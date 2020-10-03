using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/Dialog")]
    public class Dialog : BaseDialog
    {
        [Multiline]
        public string Text;

        private string parsingText = "";

        public void Reset()
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
    }
}
