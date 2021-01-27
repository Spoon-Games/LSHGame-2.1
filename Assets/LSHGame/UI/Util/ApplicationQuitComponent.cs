using UnityEngine;

namespace LSHGame.UI
{
    public class ApplicationQuitComponent : MonoBehaviour
    {
        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}
