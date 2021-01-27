using DG.Tweening;
using UnityEngine;

namespace LSHGame.Util
{
    public abstract class TweenModule : MonoBehaviour, IEffectPlayer
    {
        public float Durration = 1;

        protected Tween tween;
        public Tween Tween => tween;

        public bool IsActive => tween != null && tween.IsActive();

        public void Play()
        {
            Stop();
            tween = CreateTween();
            tween.Play();
        }

        public Tween GenerateTween()
        {
            tween = CreateTween();
            return tween;
        }

        protected abstract Tween CreateTween();

        public void Stop(bool complete)
        {
            if (tween != null && tween.active)
            {
                OnStop();
                tween.Kill(complete);
            }
        }

        protected virtual void OnStop() { }

        public void Play(Bundle parameters)
        {
            Play();
        }

        public void Stop()
        {
            Stop(false);
        }
    }
}
