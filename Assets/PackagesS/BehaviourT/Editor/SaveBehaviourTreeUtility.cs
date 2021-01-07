using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourT.Editor
{
    public class SaveBehaviourTreeUtility
    {
        private TreeGraphView graphView;

        private BehaviourTree repository = null;

        private List<Edge> Edges => graphView.edges.ToList();
        private List<NodeView> Nodes => graphView.nodes.ToList().Cast<NodeView>().ToList();

        private SaveBehaviourTreeUtility(TreeGraphView graphView)
        {
            this.graphView = graphView;
        }

        public static SaveBehaviourTreeUtility GetInstance(TreeGraphView graphView)
        {
            return new SaveBehaviourTreeUtility(graphView);
        }

        public void Save( BehaviourTree repository,bool generateScript)
        {
            bool isNewRepository = repository == null;
            if (isNewRepository)
                repository = ScriptableObject.CreateInstance<BehaviourTree>();

            List<Node> nodeData = new List<Node>();

            foreach (NodeView node in Nodes)
            {
                nodeData.Add(node.Serialize());
            }

            List<DataExNode> dataExNodes = new List<DataExNode>();
            foreach (var groupView in graphView.graphElements.ToList().Where(e => e is GroupView).Cast<GroupView>())
            {
                dataExNodes.Add(groupView.Serialize());
            }

            string path = AssetDatabase.GetAssetPath(repository);
            if (isNewRepository)
            {
                if (!AssetDatabase.IsValidFolder(Path.GetDirectoryName(path)))
                    path = "Assets/" + Path.GetFileName(path);
                Debug.Log("Is null " + (repository == null) + " " + (path == null));

                AssetDatabase.CreateAsset(repository, path);
            }

            repository.SerializeNodes(nodeData.ToArray());
            repository.SerializeDataExNodes(dataExNodes.ToArray());
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(repository);

            if (generateScript)
                GenerateBehaviourTreeComponent.GenerateBTC(repository, path);
        }

        public void Load(BehaviourTree repository)
        {

            this.repository = repository;

            ClearGraph();
            CreateNodes();

            this.repository = null;
        }

        private void CreateNodes()
        {
            foreach(Node node in repository.DeserializeNodes())
            {
                graphView.AddNodeView(node.editorPos, node);
            }

            var nodes = Nodes;
            foreach(var n in nodes)
            {
                n.Deserialize(nodes,graphView);
            }

            foreach(DataExNode dataExNode in repository.DeserializeDataExNodes())
            {
                if(dataExNode is GroupDataExNode groupData)
                {
                    graphView.AddGroup(groupData.GroupName, groupData.GroupPosition).Deserialize(groupData, nodes);
                }
            }

        }

        private void ClearGraph()
        {
            foreach (var e in graphView.graphElements.ToList())
                graphView.RemoveElement(e);
        }
    }
}
