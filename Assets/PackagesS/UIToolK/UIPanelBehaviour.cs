using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolK
{
    [RequireComponent(typeof(UINavigationComponent))]
    public class UIPanelBehaviour : MonoBehaviour
    {
        public VisualTreeAsset Panel;

        public UINavigationComponent Parent { get; private set; }
        protected Application Application => Parent.Application;

        protected virtual void Awake()
        {
            Parent = GetComponent<UINavigationComponent>();

            if(Panel == null)
            {
                Debug.LogError("Panel was not assigned to UIPanelBehaviour");
            }
        }

        internal void PanelChanged(VisualTreeAsset newPanel)
        {
            if(newPanel == Panel)
            {
                OnPanelChanged(Parent.GetRootElement());
            }
        }

        protected virtual void OnPanelChanged(VisualElement root) {}

        private void OnEnable()
        {
            Parent.OnPanelChanged += PanelChanged;
        }

        private void OnDisable()
        {
            Parent.OnPanelChanged -= PanelChanged;
        }
    }
}
