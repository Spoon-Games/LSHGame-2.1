using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/Dialog")]
    public class Dialog : BaseDialog
    {
        [SerializeField]
        public string speakerName;

        [Multiline]
        public string Text;

        [SerializeField]
        public Sprite speakerSprite;

        [SerializeField]
        public bool isLeft = true;

        private string parsingText = "";

        public override void Reset()
        {
            parsingText = Text;
        }

        public bool GetNext(out string next)
        {
            if (parsingText.Equals(""))
            {
                next = "";
                return false;
            }
      
            int split = parsingText.IndexOf("\n\n");
            if(split == -1)
            {
                next = parsingText;
                parsingText = "";
                return true;
            }

            next = parsingText.Substring(0, split);
            parsingText = parsingText.Substring(split + 2);

            return true;
        }

        public override void Show()
        {
            Debug.Log("Show is not implemented");
        }
    }
}
