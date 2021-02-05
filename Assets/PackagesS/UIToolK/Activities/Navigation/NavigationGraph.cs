using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UINavigation
{
    public static class NavigationGraph
    {

        public static string SetUp(UINavRepository r,  NavigationComponent panel)
        {
            List<NavStateBuildNode> graphNodes = new List<NavStateBuildNode>();
            SetUpP(r, out NavStartBuildNode start, out NavNestedEndBuildNode end,graphNodes);

            //if (start.next is NavStateBuildNode t)
            //{
            //    Debug.Log("Debuging Graph:" + t.DebugGraph());
            //}

            foreach (var graphNode in graphNodes)
            {
                graphNode.TransformToActivity(panel);
            }

            if (r != null)
            {
                panel.DefaultInAnimation = r.DefaultInAnimation;
                panel.DefaultOutAnimation = r.DefaultOutAnimation;
                panel.DefaultInputController = r.DefaultInputController; 
            }

            if (start.next is NavStateBuildNode navStartBuildNode)
                return navStartBuildNode.data.PanelName;
            return "";
        }

        internal static void SetUpP(UINavRepository r,  out NavStartBuildNode start,out NavNestedEndBuildNode end,List<NavStateBuildNode> graphNodes)
        {
            if(r == null)
            {
                Debug.LogError("UINavRepository is null of NestedNode or Component");
                start = new NavStartBuildNode( new NavStartNodeData(Guid.NewGuid().ToString(), Vector2.zero, new NavOutputPortData("Out", "")));
                end = new NavNestedEndBuildNode( new NavNestedEndNodeData(Guid.NewGuid().ToString(), Vector2.zero, new NavOutputPortData("Out", "")));
            }
            Dictionary<string, NavBuildNode> registeredTasks = new Dictionary<string, NavBuildNode>();

            start = new NavStartBuildNode(r.StartNode);
            registeredTasks.Add(r.StartNode.Guid, start);

            end = new NavNestedEndBuildNode( r.NestedEndNode);
            registeredTasks.Add(r.NestedEndNode.Guid, end);

            foreach (var d in r.StateNodes)
            {
                registeredTasks.Add(d.Guid, new NavStateBuildNode( d));
            }

            List<NavNestedBuildNode> nestedTasks = new List<NavNestedBuildNode>();
            foreach (var e in r.NestedNodes)
            {
                NavNestedBuildNode t = new NavNestedBuildNode(e,graphNodes);
                registeredTasks.Add(e.Guid, t);
                nestedTasks.Add(t);
            }

            foreach (var t in registeredTasks)
            {
                t.Value.Init(registeredTasks);
            }

            foreach(var nestedTask in nestedTasks)
            {
                nestedTask.InitNested();
            }

            graphNodes.AddRange(registeredTasks.Values.Where(node => node is NavStateBuildNode).Cast<NavStateBuildNode>());
        }
    }

    internal class NavStateBuildNode : NavBuildNode
    {
        private Dictionary<string, NavBuildNode> next = new Dictionary<string, NavBuildNode>();

        internal NavStateNodeData data;

        private bool wasDebugged = false;

        public NavStateBuildNode( NavStateNodeData data) 
        {
            this.data = data;
        }


        public override void Init(Dictionary<string, NavBuildNode> registeredTasks)
        {
            next.Clear();
            foreach (var p in data.CoicePorts)
            {
                if (!string.IsNullOrEmpty(p.Name))
                {
                    next.Add(p.Name, ConnectNext(registeredTasks, p.ConnectedGuid, p.Name.IsBackKey()));
                }
            }
        }

        public void ReplaceConnected(NavBuildNode old, NavBuildNode newTask, NavBuildNode newBackTask)
        {
            foreach(var n in next.ToArray())
            {
                if (n.Value == old)
                {
                    next[n.Key] = n.Key.IsBackKey() ? newBackTask : newTask;
                }
            }
        }

        internal void TransformToActivity(NavigationComponent applicationComponent)
        {
            if (applicationComponent.TryGetActivity(data.PanelName, out Activity activity))
            {
                activity.InputController = data.InputController;
                activity.InAnimation = data.InAnimation;
                activity.OutAnimation = data.OutAnimation;
                activity.DoNotHide = data.DoNotHide;

                Dictionary<string, string> nextActivities = new Dictionary<string, string>();
                foreach(var n in next)
                {
                    if (n.Value is NavStateBuildNode navStateBuildNode)
                        nextActivities.Add(n.Key, navStateBuildNode.data.PanelName);
                }
                activity.SetNavData(nextActivities);
            }
            else
                Debug.Log("Activity " + data.PanelName + " was not found");
        }

        internal string DebugGraph()
        {
            if (wasDebugged)
                return "";
            wasDebugged = true;

            string res = "\n\n\nNode: "+data.PanelName+"\n";

            foreach(var p in next)
            {
                if (p.Value is NavStateBuildNode task)
                    res += " -> " + p.Key + ": " + task.data.PanelName+"\n";
                else if(p.Value == null)
                {
                    res += " -> " + p.Key + ": null\n";
                }
                else
                {
                    res += " -> " + p.Key + ": Error, wrong type " + p.Value.GetType().ToString() + "\n";
                }
            }

            foreach (var p in next)
            {
                if (p.Value is NavStateBuildNode task)
                    res += task.DebugGraph();
            }

            return res+"\n";
        }
    }

    internal class NavStartBuildNode : NavBuildNode
    {
        private NavStartNodeData data;

        internal NavBuildNode next;

        public NavStartBuildNode( NavStartNodeData data)
        {
            this.data = data;
        }

        public override void Init(Dictionary<string, NavBuildNode> registeredTasks)
        {
            next = ConnectNext(registeredTasks, data.OutputPort.ConnectedGuid, false);
        }

        internal override NavBuildNode GetNextTask(bool isBack,NavBuildNode _this)
        {
            return null;
        }
    }

    internal class NavNestedBuildNode : NavBuildNode
    {
        private NavNestedNodeData data;

        private NavBuildNode nextOutput = null;

        private NavBuildNode nextBack = null;

        private NavStartBuildNode nestedStart = null;
        private NavNestedEndBuildNode nestedEnd = null;

        public NavNestedBuildNode(NavNestedNodeData data,List<NavStateBuildNode> graphNodes)
        {
            this.data = data;

            NavigationGraph.SetUpP(data.NestedGraph, out nestedStart, out nestedEnd,graphNodes);
        }

        public override void Init(Dictionary<string, NavBuildNode> registeredTasks)
        {
            nextOutput = ConnectNext(registeredTasks, data.OutputPort.ConnectedGuid, false);

            nextBack = ConnectNext(registeredTasks, data.BackPort.ConnectedGuid, true);
        }

        public void InitNested()
        {
            nestedEnd.ReplaceThis(nextOutput, nextBack);
        }

        internal override NavBuildNode GetNextTask(bool isBack,NavBuildNode _this)
        {
            if (!isBack)
            {
                return nestedStart.next;
            }
            else
            {
                return nestedEnd.backNext;
            }
        }
    }

    internal class NavNestedEndBuildNode : NavBuildNode
    {
        private NavNestedEndNodeData data;

        internal NavBuildNode backNext = null;

        private List<NavBuildNode> connectedTask = new List<NavBuildNode>();

        public NavNestedEndBuildNode( NavNestedEndNodeData data)
        {
            this.data = data;
        }

        public override void Init(Dictionary<string, NavBuildNode> registeredTasks)
        {
            backNext = ConnectNext(registeredTasks, data.BackPort.ConnectedGuid, true);
        }

        internal override NavBuildNode GetNextTask(bool isBack, NavBuildNode _this)
        {
            connectedTask.Add(_this);
            return this;
        }

        internal void ReplaceThis(NavBuildNode newTask,NavBuildNode newBackTask)
        {
            foreach (var ct in connectedTask)
            {
                if (ct is NavStateBuildNode t)
                {
                    t.ReplaceConnected(this, newTask, newBackTask);
                }
            }
        }
    }

    internal abstract class NavBuildNode
    {
        public abstract void Init(Dictionary<string, NavBuildNode> registeredTasks);

        internal virtual NavBuildNode GetNextTask(bool isBack,NavBuildNode _this)
        {
            return this;
        }

        internal NavBuildNode ConnectNext(Dictionary<string,NavBuildNode> registeredTask,string guid,bool isBack)
        {
            NavBuildNode result = null;
            if(registeredTask.TryGetValue(guid,out NavBuildNode t))
            {
                result = t.GetNextTask(isBack, this);
            }

            return result;
        }
    }
}
