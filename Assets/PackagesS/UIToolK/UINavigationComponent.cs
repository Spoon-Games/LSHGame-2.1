using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UIToolK
{
    //[RequireComponent(typeof(UIDocument))]
    public class UINavigationComponent : MonoBehaviour
    {
        [SerializeField]
        private UINavRepository navigationGraph;

        [SerializeField]
        private List<GlobalUIEvent> globalUIEvents = new List<GlobalUIEvent>();

        //private UIDocument document;

        public Application Application { get; private set; }

        internal event Action<VisualTreeAsset> OnPanelChanged;

        private void Awake()
        {
            //document = GetComponent<UIDocument>();

            //Application = new Application();

            //if (navigationGraph != null)
            //    NavigationGraph.SetUp(navigationGraph, Application, this)?.Run();
        }

        internal void SetAsset(VisualTreeAsset asset)
        {
            //document.visualTreeAsset = asset;

            OnPanelChanged?.Invoke(asset);

            SetUpEvents();
        }

        public VisualElement GetRootElement()
        {
            return null;
            //return document.rootVisualElement;
        }

        private void SetUpEvents()
        {
            //VisualElement root = GetRootElement();

            //Application.BackStack.OnBeforPopListener = null;

            //foreach (GlobalUIEvent e in globalUIEvents)
            //{
            //    if (e.panel == null || Equals(e.panel, document.visualTreeAsset))
            //    {
            //        if (string.IsNullOrEmpty(e.eventName))
            //            continue;

            //        if (e.eventName.IsBackKey())
            //        {
            //            Application.BackStack.OnBeforPopListener += () =>
            //            {
            //                e.action.Invoke();
            //            };
            //        }
            //        else
            //        {
            //            var buttons = root.Query<Button>(name: e.eventName).ToList();
            //            foreach (var b in buttons)
            //            {
            //                b.RegisterCallback<ClickEvent>(evt => e.action.Invoke());
            //            }
            //        }
            //    }
            //}
        }

        public void PopBackStack()
        {
            Application.BackStack.Pop();
            Debug.Log("PopBackStack");
        }
    }

    [System.Serializable]
    public class GlobalUIEvent
    {
        public string eventName;

        public VisualTreeAsset panel;

        public UnityEvent action = new UnityEvent();
    }
}
