using System.Collections;
using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/Picture Log")]
    public class PictureLog : BaseDialog
    {
        public Sprite Picture;

        public string[] Lines = new string[1] { "" };
        [FMODUnity.EventRef]
        public string Sound;

        private IEnumerator enumerator;

        public override void Reset()
        {
            enumerator = Lines.GetEnumerator();
            enumerator.Reset();
        }

        public bool GetNext(out string line)
        {
            bool hasNext = enumerator.MoveNext();
            line = hasNext ? (string)enumerator.Current : null;
            return hasNext;
        }

        public override void Show()
        {
            PictureView.Instance.ShowDialog(this);
        }
    }
}
