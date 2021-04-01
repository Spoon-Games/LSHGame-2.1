using System;
using TagInterpreterR;
using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/DuoDialog")]
    public class DuoDialog : BaseDialog
    {
        [Multiline(lines: 20,order = 5)]
        public string Data;

        public Person PersonRight;
        public Person PersonLeft;

        [Header("Reference Data")]
        public string[] Actions;

        protected override string GetData()
        {
            return Data;
        }

        public override void Show()
        {
            DuoCharacterView.Instance.Activate(this);
        }
    }
}
