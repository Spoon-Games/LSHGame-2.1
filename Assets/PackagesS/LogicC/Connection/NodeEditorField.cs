using System;
using UnityEngine;

namespace LogicC
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeEditorField : Attribute
    {
        public enum NodePlace { TitleContainer,MainContainer,PortContainer}

        public readonly NodePlace nodePlace;
        public readonly string label;
        public readonly bool hideLabel = false;
        public readonly string portContainer;
        public readonly float minWidth = -1;
        public readonly bool editable = true;
        //public readonly bool showLabel;

        public NodeEditorField(NodePlace nodePlace = NodePlace.MainContainer,string label = "",bool hideLabel = false,string portContainer = "",float minWidth = -1,bool editable = true)
        {
            this.nodePlace = nodePlace;
            this.label = label;
            this.hideLabel = hideLabel;
            this.portContainer = portContainer;
            this.minWidth = minWidth;
            this.editable = editable;
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

    //[AttributeUsage(AttributeTargets.Field)]
    //public class PreviewPort : Attribute
    //{

    //}
}
