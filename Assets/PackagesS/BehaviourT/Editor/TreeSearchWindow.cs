using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourT.Editor
{
    public class TreeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private TreeGraphView graphView;
        private BehaviourTreeEditorr editor;

        private Texture2D indentationIcon;

        private List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>();

        internal enum NodeType { DialogNode, StartNode }

        internal void Init(TreeGraphView graphView, BehaviourTreeEditorr editor)
        {
            this.graphView = graphView;
            this.editor = editor;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();

            GetAllConnectionTypes();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return searchTree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 pos = editor.rootVisualElement.ChangeCoordinatesTo(editor.rootVisualElement.parent, context.screenMousePosition
                - editor.position.position);
            pos = graphView.contentViewContainer.WorldToLocal(pos);

            if (searchTreeEntry.userData is Type type)
            {
                if (type.IsSubclassOf(typeof(BehaviourT.Node)))
                {
                    graphView.AddNodeView(pos, type);
                    return true;
                }else if (type.Equals(typeof(GroupDataExNode)))
                {
                    graphView.AddGroup(pos);
                    return true;
                }

            }

            return false;
        }

        private void GetAllConnectionTypes()
        {
            searchTree = new List<SearchTreeEntry>();
            searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Behaviour Tree Nodes"), 0));

            List<Type> AssTypes = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssTypes.AddRange(item.GetTypes());
            }

            var querry1 = from type in AssTypes
                          where type.IsSubclassOf(typeof(BehaviourT.Node)) && !type.IsAbstract && !type.Equals(typeof(RootTask))
                          select new { type, attribute = type.GetCustomAttribute<AddComponentMenu>() };

            var querry2 = from q in querry1
                          select new { q.type, path = q.attribute != null ? q.attribute.componentMenu : "New Scripts/" + q.type.Name };
            querry2 = querry2.OrderBy(q => q.path);

            string[] lastMenus = new string[0];

            foreach (var q in querry2)
            {
                string[] menus = q.path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                int lastCommonIndex = -1;
                for (int j = 0; j < menus.Length && j < lastMenus.Length; j++)
                {
                    if (Equals(menus[j], lastMenus[j]))
                        lastCommonIndex = j;
                }

                int i = lastCommonIndex;
                if (i < menus.Length - 1 && (i < lastMenus.Length - 1 || lastMenus.Length == 0))
                    i++;

                lastMenus = menus;
                for (; i < menus.Length - 1;)
                {
                    searchTree.Add(new SearchTreeGroupEntry(new GUIContent(menus[i]), i + 1));
                    i++;
                }
                searchTree.Add(new SearchTreeEntry(new GUIContent(menus[i], indentationIcon))
                {
                    userData = q.type,
                    level = i + 1
                });
            }

            searchTree.Add(new SearchTreeEntry(new GUIContent("Group", indentationIcon))
            {
                userData = typeof(GroupDataExNode),
                level = 1
            });
        }
    }
}
