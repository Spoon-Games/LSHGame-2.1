using LSHGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PictureInspect : ClickableSpeeker
    {
        protected override void Awake()
        {
            base.Awake();
            if (base.dialog is PictureLog pictureLog)
                GetComponent<SpriteRenderer>().sprite = pictureLog.Picture;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (base.dialog is PictureLog pictureLog)
                GetComponent<SpriteRenderer>().sprite = pictureLog.Picture;
        }
#endif
    } 
}
