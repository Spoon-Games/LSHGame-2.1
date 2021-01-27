using DG.Tweening;
using UnityEngine;

namespace LSHGame.Util
{
    public class PositionShakeModule : TweenModule
    {
        public bool IsYAxis = false;

        public float Amplitude = 1;
        public float ShakesPerSecond = 1;
        public float FrequencyIncrease = 0;

        protected override Tween CreateTween()
        {
            Tween tween = IsYAxis ? transform.DOLocalMoveY(Amplitude + transform.localPosition.y, Durration) : transform.DOLocalMoveX(Amplitude + transform.localPosition.x, Durration);

            EaseFunction f = ShakeEase;
            tween.SetEase(f);
            return tween;
        }

        private float ShakeEase(float time,float durration,float overshoot,float period)
        {
            time = Mathf.Exp(time * FrequencyIncrease) * time;
            return Mathf.Sin(time * Mathf.PI * 2 * ShakesPerSecond);
        }
    }
}
