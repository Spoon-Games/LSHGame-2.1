using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UINavigation
{
    public static class NavigationGraph
    {

        public static Task SetUp(UINavRepository r, Application application, UINavigationComponent panel)
        {
            SetUpP(r, application, panel, out NavStartTask start, out NavNestedEndTask end);

            end.ReplaceThis(null, null);

            //if (start.next is NavStateTask t)
            //{
            //    Debug.Log("Debuging Graph:" + t.DebugGraph());
            //}

            return start.next;
        }

        internal static void SetUpP(UINavRepository r, Application application, UINavigationComponent panel,out NavStartTask start,out NavNestedEndTask end)
        {
            if(r == null)
            {
                Debug.LogError("UINavRepository is null of NestedNode or Component");
                start = new NavStartTask(application, new NavStartNodeData(Guid.NewGuid().ToString(), Vector2.zero, new NavOutputPortData("Out", "")));
                end = new NavNestedEndTask(application, new NavNestedEndNodeData(Guid.NewGuid().ToString(), Vector2.zero, new NavOutputPortData("Out", "")));
            }
            Dictionary<string, NavTask> registeredTasks = new Dictionary<string, NavTask>();

            start = new NavStartTask(application, r.StartNode);
            registeredTasks.Add(r.StartNode.Guid, start);

            end = new NavNestedEndTask(application, r.NestedEndNode);
            registeredTasks.Add(r.NestedEndNode.Guid, end);

            foreach (var d in r.StateNodes)
            {
                registeredTasks.Add(d.Guid, new NavStateTask(application, d, panel));
            }

            List<NavNestedTask> nestedTasks = new List<NavNestedTask>();
            foreach (var e in r.NestedNodes)
            {
                NavNestedTask t = new NavNestedTask(application,panel, e);
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
        }

        internal static bool IsBackKey(this string s)
        {
            return Equals(s, "back");
        }
    }

    internal class NavStateTask : NavTask,BackStack.IPopListener
    {
        private Dictionary<string, Task> next = new Dictionary<string, Task>();

        private NavStateNodeData data;
        private UINavigationComponent panel;

        private bool wasDebugged = false;

        public NavStateTask(Application application, NavStateNodeData data, UINavigationComponent panel) : base(application)
        {
            this.panel = panel;
            this.data = data;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            panel.ShowRealPanel(data.PanelName);
        }

        public void GoToNext(string key)
        {
            if (!IsRunning)
                return;
            if (key.IsBackKey())
            {
                Application.BackStack.Pop();
            }
            else if (next.TryGetValue(key, out Task n))
            {
                if (n == null)
                    Debug.Log("Event " + key + " is not connected to anything");
                else
                    n.Run();
            }
            else
            {
                Debug.Log("Event " + key + " was not found");
            }

        }

        public override void Init(Dictionary<string, NavTask> registeredTasks)
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

        public void ReplaceConnected(Task old,Task newTask,Task newBackTask)
        {
            foreach(var n in next.ToArray())
            {
                if (n.Value == old)
                {
                    next[n.Key] = n.Key.IsBackKey() ? newBackTask : newTask;
                }
            }
        }

        public bool OnPop()
        {
            if(next.TryGetValue("back",out Task backTask))
            {
                if(backTask != null)
                {
                    Application.BackStack.PopTill(backTask);
                    return true;
                }
            }
            return false;
        }

        internal string DebugGraph()
        {
            if (wasDebugged)
                return "";
            wasDebugged = true;

            string res = "\n\n\nNode: "+data.PanelName+"\n";

            foreach(var p in next)
            {
                if (p.Value is NavStateTask task)
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
                if (p.Value is NavStateTask task)
                    res += task.DebugGraph();
            }

            return res+"\n";
        }
    }

    internal class NavStartTask : NavTask
    {
        private NavStartNodeData data;

        internal NavTask next;

        public NavStartTask(Application application, NavStartNodeData data) : base(application)
        {
            this.data = data;
        }

        public override void Init(Dictionary<string, NavTask> registeredTasks)
        {
            next = ConnectNext(registeredTasks, data.OutputPort.ConnectedGuid, false);
        }

        internal override NavTask GetNextTask(bool isBack,NavTask _this)
        {
            return null;
        }
    }

    internal class NavNestedTask : NavTask
    {
        private NavNestedNodeData data;

        private NavTask nextOutput = null;

        private NavTask nextBack = null;

        private NavStartTask nestedStart = null;
        private NavNestedEndTask nestedEnd = null;

        public NavNestedTask(Application application, UINavigationComponent panel, NavNestedNodeData data) : base(application)
        {
            this.data = data;

            NavigationGraph.SetUpP(data.NestedGraph, application, panel, out nestedStart, out nestedEnd);
        }

        public override void Init(Dictionary<string, NavTask> registeredTasks)
        {
            nextOutput = ConnectNext(registeredTasks, data.OutputPort.ConnectedGuid, false);

            nextBack = ConnectNext(registeredTasks, data.BackPort.ConnectedGuid, true);
        }

        public void InitNested()
        {
            nestedEnd.ReplaceThis(nextOutput, nextBack);
        }

        internal override NavTask GetNextTask(bool isBack,NavTask _this)
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

    internal class NavNestedEndTask : NavTask
    {
        private NavNestedEndNodeData data;

        internal NavTask backNext = null;

        private List<NavTask> connectedTask = new List<NavTask>();

        public NavNestedEndTask(Application application, NavNestedEndNodeData data) : base(application)
        {
            this.data = data;
        }

        public override void Init(Dictionary<string, NavTask> registeredTasks)
        {
            backNext = ConnectNext(registeredTasks, data.BackPort.ConnectedGuid, true);
        }

        internal override NavTask GetNextTask(bool isBack, NavTask _this)
        {
            connectedTask.Add(_this);
            return this;
        }

        internal void ReplaceThis(NavTask newTask,NavTask newBackTask)
        {
            foreach (var ct in connectedTask)
            {
                if (ct is NavStateTask t)
                {
                    t.ReplaceConnected(this, newTask, newBackTask);
                }
            }
        }
    }

    internal abstract class NavTask : Task
    {
        public NavTask(Application application) : base(application)
        {
        }

        public abstract void Init(Dictionary<string, NavTask> registeredTasks);

        internal virtual NavTask GetNextTask(bool isBack,NavTask _this)
        {
            return this;
        }

        internal NavTask ConnectNext(Dictionary<string,NavTask> registeredTask,string guid,bool isBack)
        {
            NavTask result = null;
            if(registeredTask.TryGetValue(guid,out NavTask t))
            {
                result = t.GetNextTask(isBack, this);
            }

            return result;
        }
    }
}
