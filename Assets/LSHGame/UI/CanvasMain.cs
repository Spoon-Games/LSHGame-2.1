using LSHGame.Util;
using SceneM;

namespace LSHGame.UI
{
    public class CanvasMain : Singleton<CanvasMain>
    {
        private PanelManager panelManager;

        public override void Awake()
        {
            base.Awake();

            panelManager = GetComponent<PanelManager>();
        }

        public void ShowPanel(string panelName)
        {
            panelManager.ShowPanel(panelName);
        }
    } 
}
