using System.Collections.Generic;
using UnityEngine;

namespace UINavigation
{
    public abstract class BasePanelManager<T,P,M> : MonoBehaviour where P : BasePanel<T,P,M> where M : BasePanelManager<T, P, M>
    {
        [SerializeField]
        protected T startPanel;

        protected Dictionary<T, P> panels = new Dictionary<T, P>();

        public virtual P CurrentPanel => currentPanel;

        protected P currentPanel;

        protected virtual void Awake()
        {
            panels.Clear();
            GetChildren();
            //Debug.Log("Awake "+name);
        }

        protected virtual void Start()
        {
            ShowPanel(startPanel);
        }

        public C ShowPanelGetComponent<C>(T panelName) where C : MonoBehaviour
        {
            ShowPanel(panelName);
            return currentPanel?.GetComponent<C>();
        }

        public virtual P ShowPanel(T panelName)
        {
            currentPanel?.SetVisible(false);

            if (panelName == null)
                currentPanel = null;
            else
            {
                panels.TryGetValue(panelName, out currentPanel);
                currentPanel?.SetVisible(true);
            }

            return currentPanel;
        }

        public void ShowPanelByP(P panel)
        {
            ShowPanel(panel.PanelName);
        }

        private void OnDestroy()
        {
            panels.Clear();
        }

        private void GetChildren()
        {
            transform.GetChildren<P, PanelGroup>(p =>
            {
                if (p.PanelName == null)
                {
                    Debug.Log("No PanelName was asigned to: " + p.name);
                }
                else
                {
                    p.Parent = (M)this;
                    if (panels.ContainsKey(p.PanelName))
                        Debug.Log("Duplicate PanelName: " + p.PanelName + " for panel: " + p.name);
                    panels[p.PanelName] = p;
                    p.SetVisible(false);
                }
            });
            
        }
    }

    public abstract class BasePanel<T,P,M> : MonoBehaviour where P : BasePanel<T,P,M> where M : BasePanelManager<T,P,M>
    {
        public T PanelName;

        public M Parent { get; internal set; }

        public void Show()
        {
            Parent.ShowPanel(PanelName);
        }

        internal virtual void SetVisible(bool visible)
        {
            if (gameObject != null)
                gameObject.SetActive(visible);
            else
                Debug.Log("GameObject is null "+PanelName);
        }
    }
}
