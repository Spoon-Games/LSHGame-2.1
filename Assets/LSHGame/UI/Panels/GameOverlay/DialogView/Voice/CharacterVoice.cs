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
    public class CharacterVoice : ScriptableObject
    {
        [FMODUnity.EventRef]
        public string soundFolder;
        public float probability = 1;


        public void PlayChar(char c)
        {
            c = char.ToUpper(c);
            if (c >= 64 && c < 90)
            {
                
                if(probability >= 1 || Random.value <= probability)
                    RuntimeManager.PlayOneShot(soundFolder + '/' + c);
            }
        }
    }
}
