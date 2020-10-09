using UnityEngine;

namespace UINavigation
{
    //[RequireComponent(typeof(UIDocument))]
    public class UINavigationComponent : PanelManager
    {
        [SerializeField]
        private UINavRepository navigationGraph;

        public Application Application { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Application = new Application();

            if (navigationGraph != null)
                NavigationGraph.SetUp(navigationGraph, Application, this)?.Run();
            //document = GetComponent<UIDocument>();
        }

        protected override void Start()
        {
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

        //internal void SetAsset(VisualTreeAsset asset)
        //{
        //    //document.visualTreeAsset = asset;

        //    //OnPanelChanged?.Invoke(asset);

        //    //SetUpEvents();
        //}

        //public VisualElement GetRootElement()
        //{
        //    return null;
        //    //return document.rootVisualElement;
        //}

        //private void SetUpEvents()
        //{
        //    //VisualElement root = GetRootElement();

        //    //Application.BackStack.OnBeforPopListener = null;

        //    //foreach (GlobalUIEvent e in globalUIEvents)
        //    //{
        //    //    if (e.panel == null || Equals(e.panel, document.visualTreeAsset))
        //    //    {
        //    //        if (string.IsNullOrEmpty(e.eventName))
        //    //            continue;

        //    //        if (e.eventName.IsBackKey())
        //    //        {
        //    //            Application.BackStack.OnBeforPopListener += () =>
        //    //            {
        //    //                e.action.Invoke();
        //    //            };
        //    //        }
        //    //        else
        //    //        {
        //    //            var buttons = root.Query<Button>(name: e.eventName).ToList();
        //    //            foreach (var b in buttons)
        //    //            {
        //    //                b.RegisterCallback<ClickEvent>(evt => e.action.Invoke());
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

        public void PopBackStack()
        {
            Application.BackStack.Pop();
            //Debug.Log("PopBackStack");
        }
    }

    //[System.Serializable]
    //public class GlobalUIEvent
    //{
    //    public string eventName;

    //    public VisualTreeAsset panel;

    //    public UnityEvent action = new UnityEvent();
    //}
}
