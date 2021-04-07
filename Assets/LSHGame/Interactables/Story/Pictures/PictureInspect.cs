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
            {
                if (TryGetComponent<SpriteRenderer>(out SpriteRenderer s)) {

                    s.sprite = pictureLog.Picture;
                    if (TryGetComponent(out BoxCollider2D box))
                    {
                        box.size = s.bounds.size;
                        box.offset = transform.worldToLocalMatrix.MultiplyPoint(s.bounds.center);
                    }
                }
            }
        }
#endif
    } 
}
