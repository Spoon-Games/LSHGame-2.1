namespace UINavigation
{
    public class PanelManager : BasePanelManager<string, Panel, PanelManager>, IGoToNextManager
    {
        public void GoToNext(string key)
        {
            ShowPanel(key);
        }
    }
}
