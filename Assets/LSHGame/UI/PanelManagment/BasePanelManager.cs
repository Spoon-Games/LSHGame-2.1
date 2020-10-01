using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.UI
{
    public abstract class BasePanelManager<T,P,M> : MonoBehaviour where P : BasePanel<T,P,M> where M : BasePanelManager<T, P, M>
    {
        [SerializeField]
        protected T startPanel;

        protected Dictionary<T, P> panels = new Dictionary<T, P>();

        private P currentPanel;

        protected virtual void Awake()
        {
            panels.Clear();
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<P>(out P p))
                {
                    p.Parent = (M)this;
                    panels[p.panelName] = p;
                    p.SetVisible(false);
                }
            }

            ShowPanel(startPanel);
        }

        public C ShowPanelGetComponent<C>(T panelName) where C : MonoBehaviour
        {
            ShowPanel(panelName);
            return currentPanel?.GetComponent<C>();
        }

        public P ShowPanel(T panelName)
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
    }

    public abstract class BasePanel<T,P,M> : MonoBehaviour where P : BasePanel<T,P,M> where M : BasePanelManager<T,P,M>
    {
        [SerializeField]
        internal T panelName;

        public M Parent { get; internal set; }

        public void Show()
        {
            Parent.ShowPanel(panelName);
        }

        internal virtual void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
