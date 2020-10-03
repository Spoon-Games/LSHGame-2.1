using UnityEngine;

namespace UINavigation
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField]
        private string buttonName;

        private PanelManager panelManager;

        private void Awake()
        {
            panelManager = GetComponentInParent<PanelManager>();
            if(panelManager == null)
            {
                Debug.LogError("No PanelManager was found in parents");
            }
        }

        public void Activate()
        {
            if (panelManager == null)
                return;

            string s = buttonName;
            if (string.IsNullOrEmpty(s))
                s = gameObject.name;

            panelManager.ShowPanel(s);
        }
    }
}
