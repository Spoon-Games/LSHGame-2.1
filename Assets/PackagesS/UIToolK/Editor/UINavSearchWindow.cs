using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolK.Editor
{
    public class UINavSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private UINavGraphView graphView;
        private UINavEditor editor;

        private Texture2D indentationIcon;

        internal enum NodeType { NavStateNode,NavNestedNode}

        internal void Init(UINavGraphView graphView,UINavEditor editor)
        {
            this.graphView = graphView;
            this.editor = editor;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"),0),
                //new SearchTreeGroupEntry(new GUIContent("Dialog Node"),1),
                new SearchTreeEntry(new GUIContent("Navigation State Node",indentationIcon))
                {
                    userData = NodeType.NavStateNode,
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Nested Node",indentationIcon))
                {
                    userData = NodeType.NavNestedNode,
                    level = 1
                }

            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 pos = editor.rootVisualElement.ChangeCoordinatesTo(editor.rootVisualElement.parent, context.screenMousePosition 
                - editor.position.position);
            pos = graphView.contentViewContainer.WorldToLocal(pos);

            switch ((NodeType)SearchTreeEntry.userData)
            {
                case NodeType.NavStateNode:
                    graphView.CreateNavStateNode(pos);
                    return true;
                case NodeType.NavNestedNode:
                    graphView.CreateNavNestedNode(pos);
                    return true;
                default:
                    return false ;
            }
        }
    }
}
