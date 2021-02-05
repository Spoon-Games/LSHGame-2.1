using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UINavigation.Editor
{
    public class NavStateNode : NavBaseNode
    {
        internal string PanelName { get; private set; }

        internal TextField InputControllerField;
        internal Toggle DoNotHideField;
        internal TextField InAnimationField;
        internal TextField OutAnimationField;
        //internal string PanelName { get; private set; }

        public NavStateNode(UINavGraphView graphView, Vector2 position, string guid, string panelName)
            : base(graphView, "NavState", position, guid)
        {
            AddChoicePortButton();
            CreateMainContainer(panelName);

            AddPort("In", Direction.Input, Port.Capacity.Multi);
        }

        public NavStateNode(UINavGraphView graphView, Vector2 position) : this(graphView, position, null, "") { }

        private void CreateMainContainer(string panelName)
        {
            PanelName = panelName;

            //ObjectField visualTreeField = new ObjectField("Source Asset") { objectType = typeof(VisualTreeAsset),value = panelName};
            //visualTreeField.RegisterValueChangedCallback(evt =>
            //{
            //    if (evt.newValue is VisualTreeAsset v)
            //    {
            //        PanelName = v;
            //    }
            //});
            TextField panelNameField = new TextField("Panel Name") { value = panelName };
            panelNameField.RegisterValueChangedCallback(s => PanelName = s.newValue);
            mainContainer.Add(panelNameField);

            InputControllerField = new TextField("Input Controller");
            mainContainer.Add(InputControllerField);

            DoNotHideField = new Toggle("Do not hide");
            mainContainer.Add(DoNotHideField);

            InAnimationField = new TextField("In Animation");
            mainContainer.Add(InAnimationField);

            OutAnimationField = new TextField("Out Animation");
            mainContainer.Add(OutAnimationField);




            mainContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
        }
    }

    public class NavStartNode : NavBaseNode
    {
        public NavStartNode(UINavGraphView graphView, Vector2 position, string guid = null)
            : base(graphView, "START", position, guid)
        {
            AddPort("Out", Direction.Output, Port.Capacity.Single);
            capabilities &= ~Capabilities.Deletable;
        }
    }

    public class NavNestedEndNode : NavBaseNode
    {
        public NavNestedEndNode(UINavGraphView graphView, Vector2 position, string guid = null)
            : base(graphView, "Nested End", position, guid)
        {
            AddPort("End", Direction.Input, Port.Capacity.Multi);
            AddPort("Back (From Parent)", Direction.Output, Port.Capacity.Single);
            capabilities &= ~Capabilities.Deletable;
        }
    }

    public class NavNestedNode : NavBaseNode
    {
        internal UINavRepository Repository { get; private set; }

        public NavNestedNode(UINavGraphView graphView, Vector2 position, string guid, UINavRepository repository)
            : base(graphView, "NavNested", position, guid)
        {
            CreateMainContainer(repository);

            AddPort("In", Direction.Input, Port.Capacity.Multi);
            AddPort("Out", Direction.Output, Port.Capacity.Single);
            AddPort("Back", Direction.Output, Port.Capacity.Single);
        }

        public NavNestedNode(UINavGraphView graphView, Vector2 position) : this(graphView, position, null, null) { }

        private void CreateMainContainer(UINavRepository repository)
        {
            Repository = repository;

            ObjectField repositoryField = new ObjectField("UI Navigation Graph") { objectType = typeof(UINavRepository), value = repository };
            repositoryField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is UINavRepository v)
                {
                    Repository = v;
                }
            });

            mainContainer.Add(repositoryField);
            mainContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
        }
    }
}
