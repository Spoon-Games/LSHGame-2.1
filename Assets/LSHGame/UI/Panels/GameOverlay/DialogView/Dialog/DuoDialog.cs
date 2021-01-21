using FMODUnity;
using System.Collections;
using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/DuoDialog")]
    public class DuoDialog : BaseDialog
    {
        public LineData[] Lines;

        private IEnumerator enumerator;

        public override void Reset()
        {
            enumerator = Lines.GetEnumerator();
            enumerator.Reset();
        }

        public bool GetNext(out LineData line)
        {
            bool hasNext = enumerator.MoveNext();
            line = hasNext?(LineData)enumerator.Current:null;
            return hasNext;
        }
    }

    [System.Serializable]
    public class LineData
    {
        public Sprite image;
        [EventRef]
        public string sound;
        public string name;
        public bool isRight;
        [Multiline]
        public string text;
    }
}
