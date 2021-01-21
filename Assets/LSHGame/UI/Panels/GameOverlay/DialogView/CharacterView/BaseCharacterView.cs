using UnityEngine;

namespace LSHGame.UI
{
    public abstract class BaseCharacterView : MonoBehaviour
    {
        public abstract bool TrySetDialog(BaseDialog baseDialog);
    }
}
