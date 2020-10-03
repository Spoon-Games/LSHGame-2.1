using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UINavigation
{
    [CreateAssetMenu(fileName = "UI Navigation Graph",menuName ="UI Navigation Graph")]
    public class UINavRepository : ScriptableObject
    {
        public NavStartNodeData StartNode = new NavStartNodeData(Guid.NewGuid().ToString(),Vector2.zero,new NavOutputPortData("Out",""));
        public NavNestedEndNodeData NestedEndNode = new NavNestedEndNodeData(Guid.NewGuid().ToString(),new Vector2(0,200),new NavOutputPortData("Back",""));

        public List<NavStateNodeData> StateNodes = new List<NavStateNodeData>();
        public List<NavNestedNodeData> NestedNodes = new List<NavNestedNodeData>();

    }

    [System.Serializable]
    public class NavStateNodeData : NavNodeData
    {
        public List<NavOutputPortData> CoicePorts;

        public string PanelName;

        public NavStateNodeData(string guid, Vector2 editorPosition, List<NavOutputPortData> coicePorts, string panelName) : base(guid, editorPosition)
        {
            CoicePorts = coicePorts;
            this.PanelName = panelName;
        }
    }

    [System.Serializable]
    public class NavNestedNodeData : NavNodeData
    {
        public NavOutputPortData OutputPort;
        public NavOutputPortData BackPort;

        public UINavRepository NestedGraph;

        public NavNestedNodeData(string guid, Vector2 editorPosition, NavOutputPortData outputPort, NavOutputPortData backPort, UINavRepository nestedGraph)
            : base(guid, editorPosition)
        {
            OutputPort = outputPort;
            BackPort = backPort;
            NestedGraph = nestedGraph;
        }
    }

    [System.Serializable]
    public class NavStartNodeData : NavNodeData
    {
        public NavOutputPortData OutputPort;

        public NavStartNodeData(string guid, Vector2 editorPosition, NavOutputPortData outputPort) : base(guid, editorPosition)
        {
            OutputPort = outputPort;
        }
    }

    [System.Serializable]
    public class NavNestedEndNodeData : NavNodeData
    {
        public NavOutputPortData BackPort;

        public NavNestedEndNodeData(string guid, Vector2 editorPosition, NavOutputPortData backPort) : base(guid, editorPosition)
        {
            BackPort = backPort;
        }
    }



    public abstract class NavNodeData
    {
        public string Guid;

        public Vector2 EditorPosition;

        protected NavNodeData(string guid, Vector2 editorPosition)
        {
            Guid = guid;
            EditorPosition = editorPosition;
        }
    }

    [System.Serializable]
    public class NavOutputPortData
    {
        public string Name;

        public string ConnectedGuid;

        public NavOutputPortData(string name, string connectedGuid)
        {
            Name = name;
            ConnectedGuid = connectedGuid;
        }
    }

}