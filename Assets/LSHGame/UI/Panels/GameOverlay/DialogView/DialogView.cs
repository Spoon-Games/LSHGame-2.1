using SceneM;
using UnityEngine;

namespace LSHGame.UI
{
    public class DialogView : Singleton<DialogView>
    {
        [SerializeField]
        public BaseCharacterView[] characterViews;

        public void SetDialog(BaseDialog dialog)
        {
            Debug.Log("ShowDialog: " + dialog);
            foreach(var view in characterViews)
            {
                if (view.TrySetDialog(dialog))
                    return;
            }
        }
    }
}
