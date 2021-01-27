using UnityEngine;

namespace UINavigation
{
    //[RequireComponent(typeof(UIDocument))]
    public class UINavigationComponent : PanelManager
    {
        public static UINavigationComponent Instance;

        [SerializeField]
        private UINavRepository navGraph;

        public UINavRepository NavGraph { get => navGraph; set
            {
                if (value == null || value == navGraph)
                    return;

                navGraph = value;
                LoadNavGraph();
            }
        }

        public Application Application { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (Instance == null)
                Instance = this;
            //document = GetComponent<UIDocument>();
        }

        protected override void Start()
        {
            if (Application == null)
                LoadNavGraph();
        }

        public override Panel ShowPanel(string panelName)
        {
            //Debug.Log("ShowPanel: " + panelName);
            if (string.IsNullOrEmpty(panelName))
                return base.CurrentPanel;

            if (Application.Currant is NavStateTask navState)
            {
                navState.GoToNext(panelName);
            }
            return base.currentPanel;
        }

        internal void ShowRealPanel(string panelName)
        {
            //Debug.Log("ShowRealPanel: " + panelName);
            base.ShowPanel(panelName);
        }

        private void LoadNavGraph()
        {
            Application = new Application();
            if (NavGraph != null)
                NavigationGraph.SetUp(NavGraph, Application, this)?.Run();
        }


        public void PopBackStack()
        {
            Application.BackStack.Pop();
            //Debug.Log("PopBackStack");
        }
    }
}
