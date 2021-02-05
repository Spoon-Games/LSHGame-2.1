using System;
using System.Collections.Generic;
using UnityEngine;

namespace UINavigation
{
    //[RequireComponent(typeof(UIDocument))]
    public class NavigationComponent : MonoBehaviour,IGoToNextManager
    {
        public BackStack BackStack;

        [SerializeField]
        private UINavRepository navGraph;

        public UINavRepository NavGraph
        {
            get => navGraph; set
            {
                if (value == null || value == navGraph)
                    return;

                navGraph = value;
                LoadNavGraph();
            }
        }

        public string DefaultInputController;
        public string DefaultInAnimation;
        public string DefaultOutAnimation;

        private Dictionary<string, Activity> activities = new Dictionary<string, Activity>();

        public ITaskable CurrantTask => BackStack.GetCurrant();

        #region Init
        protected virtual void Awake()
        {
            activities.Clear();
            GetChildren();

            BackStack = new BackStack();
        }

        protected void Start()
        {
            if(CurrantTask == null)
                LoadNavGraph();
        } 
        #endregion

        #region Application
        public void RunActivity(string activity)
        {
            if (activities.TryGetValue(activity, out Activity a))
                BackStack.AddTask(a);
            else
                Debug.Log("Could not run activity " + activity + " because key was not found");
        }

        public void GoToNext(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            
            if(CurrantTask is INextableTask nextable)
            {
                nextable.GoToNext(key);
            }
        }

        internal bool TryGetActivity(string name,out Activity activity)
        {
            return activities.TryGetValue(name, out activity);
        }

        #region Backstack
        public void PopBackStack()
        {
            BackStack.Pop();
        }

        public void PopBackStackTill(string activity)
        {
            if (activities.TryGetValue(activity, out Activity a))
            {
                BackStack.PopTill(a);
            }
            else
                Debug.Log("Could not pop activity " + activity + " because key was not found");
        }
        #endregion
        #endregion

        #region Helper Methods

        private void LoadNavGraph()
        {
            BackStack.Clear();
            if (NavGraph != null)
                RunActivity(NavigationGraph.SetUp(NavGraph, this));
        }

        private void GetChildren()
        {
            transform.GetChildren<Activity, PanelGroup>(a =>
             {
                 a.Parent = this;
                 a.SetVisible(false);
                 if (activities.ContainsKey(a.name))
                     Debug.Log("Dublicate name for activity: " + a.name);
                 activities[a.name] = a;
             });
        }
        #endregion
    }
}
