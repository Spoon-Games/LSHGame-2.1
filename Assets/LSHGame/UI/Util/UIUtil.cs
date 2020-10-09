using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;

namespace LSHGame.UI
{
    public static class UIUtil
    {
        public static TweenerCore<string, string, StringOptions> DOTypeWritePerSpeed(this TMP_Text textField, string target, float typeSpeed, string from = "")
        {
            return DOTypeWrite(textField, target, ((float)target.Length) * typeSpeed, from);
        }

        public static TweenerCore<string,string,StringOptions> DOTypeWrite(this TMP_Text textField,string target,float durration,string from = "")
        {
            textField.text = from;
            return DOTween.To(() => textField.text, (string t) => textField.SetText(t),target, durration);
        }
    }
}
