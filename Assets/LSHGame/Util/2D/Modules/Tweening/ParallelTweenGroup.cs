using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class ParallelTweenGroup : TweenModule
    {
        
        private TweenModule[] _children;
        public TweenModule[] Children
        {
            get
            {
                if (_children == null || _children.Length == 0)
                    GetChildren();
                return _children;
            }
        }

        private void Awake()
        {
            GetChildren();
        }

        protected override Tween CreateTween()
        {
            Sequence sequence = DOTween.Sequence();
#if UNITY_EDITOR
            GetChildren();
#endif
            foreach (var c in Children)
            {
                sequence.Insert(0, c.GenerateTween());
            }
            return sequence;

        }

        private void GetChildren()
        {
            List<TweenModule> children = new List<TweenModule>();
            foreach(Transform t in transform)
            {
                GetChildren(t, children);
            }
            _children = children.ToArray();
        }

        private void GetChildren(Transform t, List<TweenModule> children)
        {
            children.AddRange(t.GetComponents<TweenModule>());
            foreach(Transform c in t)
            {
                GetChildren(c, children);
            }
        }
    }
}
