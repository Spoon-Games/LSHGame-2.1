using System.Collections;
using TagInterpreterR;
using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/Picture Log")]
    public class PictureLog : BaseDialog
    {
        public Sprite Picture;

        [Multiline(lines: 20,order = 5)]
        public string Text;
        [FMODUnity.EventRef]
        public string OpeningSound;

        protected override string GetData()
        {
            return Text;
        }

        public override void Show()
        {
            PictureView.Instance.Activate(this);
        }
    }
}
