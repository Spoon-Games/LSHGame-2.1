using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UINavigation.Editor
{
    public class SaveUINavUtility
    {
        private UINavGraphView graphView;
        private NavToolbar toolbar;

        private UINavRepository repository = null;

        private List<Edge> Edges => graphView.edges.ToList();
        private List<NavBaseNode> Nodes => graphView.nodes.ToList().Cast<NavBaseNode>().ToList();

        private SaveUINavUtility(UINavGraphView graphView,NavToolbar navToolbar)
        {
            this.graphView = graphView;
            toolbar = navToolbar;
        }

        public static SaveUINavUtility GetInstance(UINavGraphView graphView,NavToolbar navToolbar)
        {
            return new SaveUINavUtility(graphView,navToolbar);
        }

        public void Save(string path, UINavRepository repository)
        {
            bool isNewRepository = repository == null;
            if (isNewRepository)
                repository = ScriptableObject.CreateInstance<UINavRepository>();

            repository.DefaultInputController = toolbar.DefaultInputControllerField.text;
            repository.DefaultInAnimation = toolbar.DefaultInAnimationField.text;
            repository.DefaultOutAnimation = toolbar.DefaultOutAnimationField.text;

            repository.NestedNodes.Clear();
            repository.StateNodes.Clear();

            foreach (NavBaseNode node in Nodes)
            {
                List<NavOutputPortData> outputPorts = new List<NavOutputPortData>();
                foreach (Port p in node.outputContainer.Query<Port>().ToList())
                {
                    TextField t = p.Q<TextField>();
                    string name = t != null ? t.text : p.name;

                    string connected = "";

                    if (p.connections.Count() > 0)
                    {
                        Port inport = p.connections.First().input;
                        if (inport.node is NavBaseNode connectedNode)
                        {
                            connected = connectedNode.GUID;
                        }
                    }
                    outputPorts.Add(new NavOutputPortData(name, connected));
                }

                if(node is NavStateNode stateNode)
                {
                    repository.StateNodes.Add(new NavStateNodeData(node.GUID, node.GetPosition().position, outputPorts, stateNode.PanelName) {
                        InputController = stateNode.InputControllerField.text,
                        DoNotHide = stateNode.DoNotHideField.value,
                        InAnimation = stateNode.InAnimationField.text,
                        OutAnimation = stateNode.OutAnimationField.text

                    });
                }else if(node is NavNestedNode nestedNode)
                {
                    repository.NestedNodes.Add(new NavNestedNodeData(node.GUID, node.GetPosition().position, outputPorts.First(), outputPorts[1], nestedNode.Repository));
                }else if(node is NavStartNode)
                {
                    repository.StartNode = new NavStartNodeData(node.GUID, node.GetPosition().position, outputPorts.First());
                }else if(node is NavNestedEndNode)
                {
                    repository.NestedEndNode = new NavNestedEndNodeData(node.GUID, node.GetPosition().position,outputPorts.First());
                }
            }


            if (isNewRepository)
            {
                if (!AssetDatabase.IsValidFolder(Path.GetDirectoryName(path)))
                   path = "Assets/" + Path.GetFileName(path);
                Debug.Log("Is null " + (repository == null) + " " + (path == null));


                AssetDatabase.CreateAsset(repository, path);
            }
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(repository);
        }

        public void Load(UINavRepository repository)
        {

            this.repository = repository;

            toolbar.DefaultInputControllerField.SetValueWithoutNotify( repository.DefaultInputController);
            toolbar.DefaultInAnimationField.SetValueWithoutNotify( repository.DefaultInAnimation);
            toolbar.DefaultOutAnimationField.SetValueWithoutNotify( repository.DefaultOutAnimation);

            ClearGraph();
            CreateNodes();

            this.repository = null;
        }

        private void CreateNodes()
        {
            Dictionary<string, NavBaseNode> loadedNodes = new Dictionary<string, NavBaseNode>();
            List<Tuple<Port,NavOutputPortData>> dependingConnections = new List<Tuple<Port, NavOutputPortData>>();

            NavStartNode startNode = new NavStartNode(graphView, repository.StartNode.EditorPosition, repository.StartNode.Guid);
            dependingConnections.Add(new Tuple<Port, NavOutputPortData>(startNode.outputContainer.Q<Port>(), repository.StartNode.OutputPort));
            loadedNodes.Add(startNode.GUID, startNode);

            NavNestedEndNode nestedEndNode = new NavNestedEndNode(graphView, repository.NestedEndNode.EditorPosition, repository.NestedEndNode.Guid);
            dependingConnections.Add(new Tuple<Port, NavOutputPortData>(nestedEndNode.outputContainer.Q<Port>(), repository.NestedEndNode.BackPort));
            loadedNodes.Add(nestedEndNode.GUID, nestedEndNode);

            foreach(NavStateNodeData d in repository.StateNodes)
            {
                NavStateNode stateNode = new NavStateNode(graphView, d.EditorPosition, d.Guid, d.PanelName);
                stateNode.InputControllerField.SetValueWithoutNotify(d.InputController);
                stateNode.DoNotHideField.SetValueWithoutNotify(d.DoNotHide);
                stateNode.InAnimationField.SetValueWithoutNotify(d.InAnimation);
                stateNode.OutAnimationField.SetValueWithoutNotify(d.OutAnimation);

                foreach(var portData in d.CoicePorts)
                {
                    dependingConnections.Add(new Tuple<Port,NavOutputPortData>(stateNode.AddChoicePort(portData.Name), portData));
                }
                loadedNodes.Add(stateNode.GUID, stateNode);
            }

            foreach (NavNestedNodeData d in repository.NestedNodes)
            {
                NavNestedNode nestedNode = new NavNestedNode(graphView, d.EditorPosition, d.Guid,d.NestedGraph);

                Port outputPort = nestedNode.outputContainer[0].Q<Port>();
                Port backPort = nestedNode.outputContainer[1].Q<Port>();

                dependingConnections.Add(new Tuple<Port, NavOutputPortData>(outputPort, d.OutputPort));
                dependingConnections.Add(new Tuple<Port, NavOutputPortData>(backPort, d.BackPort));
                loadedNodes.Add(nestedNode.GUID, nestedNode);
            }


            foreach(var t in dependingConnections)
            {
                if (!string.IsNullOrEmpty(t.Item2.ConnectedGuid))
                {
                    Port inputPort = loadedNodes[t.Item2.ConnectedGuid].inputContainer[0].Q<Port>();
                    graphView.CreateEdge(inputPort, t.Item1);
                }
            }

        }

        private void ClearGraph()
        {
            foreach (Edge e in Edges)
            {
                graphView.RemoveElement(e);
            }

            foreach (NavBaseNode node in Nodes)
            {
                graphView.RemoveElement(node);
            }
        }
    }
}
