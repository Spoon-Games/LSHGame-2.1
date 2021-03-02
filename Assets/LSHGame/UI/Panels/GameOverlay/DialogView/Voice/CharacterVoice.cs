using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMOD.Studio;
using FMODUnity;
using SceneM;
using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Voice/Character Voice")]
    public class CharacterVoice : BaseVoice
    {
        [FMODUnity.EventRef]
        public string soundFolder;

        public float readSpeed = 1;
        public float probability = 1;

        private string readText = "";

        private Tween readTween;

        public override void Play(string text)
        {
            Stop();

            readTween = DoCharacterRead(text);
        }

        private TweenerCore<string, string, StringOptions> DoCharacterRead(string target)
        {
            readText = "";
            return DOTween.To(() => readText,
                (string t) => {
                    if(!Equals(t,readText))
                        PlayLastLetter(readText);
                    readText = t;
                    },
                target, ((float)target.Length) * readSpeed)
                .From("", false).SetEase(Ease.Linear);
        }

        private void PlayLastLetter(string s)
        {
            if (s.Length == 0)
                return;

            char c = s[s.Length - 1];
            c = char.ToUpper(c);
            if (c >= 64 && c < 90)
            {
                
                if(probability >= 1 || Random.value <= probability)
                    RuntimeManager.PlayOneShot(soundFolder + '/' + c);
            }
        }

        public override void Stop()
        {
            if (readTween != null && readTween.active)
                readTween.Kill();
        }
    }
}
