using System;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteParallax : ParallaxLayer
    {
        [SerializeField]
        private Rect orthographicBounds;

        [SerializeField]
        private float maxCameraOrthographicSize = 5;

        [SerializeField]
        private float maxAspectRatio = 16 / 9;

        protected override void Start()
        {
            base.Start();
            GetSpirteSize();
        }

        private void GetSpirteSize()
        {
            Vector2 cameraSize = GetCameraSize();
            float multiplier = GetMultiplier();
            Vector2 center = orthographicBounds.center;
            Vector2 rectSize = orthographicBounds.size / 2;

            Vector2 spriteSize = rectSize + cameraSize - multiplier * rectSize;

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if(renderer.sprite != null)
            {
                Vector2 realSpriteSize = renderer.size / 2;
                Vector2 factor = spriteSize / realSpriteSize;
                float scale = Mathf.Max(1, factor.x, factor.y, transform.localScale.x, transform.localScale.y);
                transform.localScale = new Vector3(scale, scale, 1);
            }

            Vector2 newStartPos = ((Vector2) startCameraPos - center) * multiplier + center;
            this.startPos = new Vector3(newStartPos.x, newStartPos.y, this.startPos.z);
            transform.position = this.startPos;
        }

        private Vector2 GetCameraSize()
        {
            return new Vector2(maxAspectRatio * maxCameraOrthographicSize, maxCameraOrthographicSize);
        }

#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    try
        //    {
        //        if (Camera.main.transform != null)
        //        {
        //            Start();
        //        }
        //    }
        //    catch (Exception) { }
        //}

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(orthographicBounds.center, orthographicBounds.size);

            //Gizmos.DrawWireCube(Camera.main.transform.position, GetCameraSize(Camera.main) * 2 + new Vector2(0.1f,0.1f));
        }
#endif
    }
}
