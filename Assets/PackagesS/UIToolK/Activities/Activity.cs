using System.Collections.Generic;
using UnityEngine;

namespace UINavigation
{
    public class Activity : MonoBehaviour, ITaskable, BackStack.IPopListener, INextableTask, IAnimActivity
    {
        #region Public Properties
        public NavigationComponent Parent { get; internal set; }

        public bool IsRunning => Parent.BackStack.IsCurrant(this);
        #endregion

        #region Navigation Data
        public string InputController { get => string.IsNullOrEmpty(_inputController) ? Parent.DefaultInputController : _inputController; set => _inputController = value; }
        private string _inputController;
        public bool DoNotHide { get; set; }
        public string InAnimation { get => string.IsNullOrEmpty(_inAnimation) ? Parent.DefaultInAnimation : _inAnimation; set => _inAnimation = value; }
        private string _inAnimation;
        public string OutAnimation { get => string.IsNullOrEmpty(_outAnimation) ? Parent.DefaultOutAnimation : _outAnimation; set => _outAnimation = value; }
        private string _outAnimation;

        protected Dictionary<string, string> nextActivities = null;
        #endregion

        #region Private Properties
        private List<IAnimationComponent> _animationComponents;
        protected List<IAnimationComponent> AnimationComponents
        {
            get
            {
                if (_animationComponents == null)
                {
                    _animationComponents = new List<IAnimationComponent>();
                    GetComponents<IAnimationComponent>(_animationComponents);
                }
                return _animationComponents;
            }
            set => _animationComponents = value;
        }

        private List<IActivityLifecicleCallback> _lifecicleCallbacks;
        private List<IActivityLifecicleCallback> LifecicleCallbacks
        {
            get
            {
                if (_lifecicleCallbacks == null)
                {
                    _lifecicleCallbacks = new List<IActivityLifecicleCallback>();
                    GetComponents<IActivityLifecicleCallback>(LifecicleCallbacks);
                }
                return _lifecicleCallbacks;
            }
            set => _lifecicleCallbacks = value;
        }

        #endregion

        #region Task
        public virtual void OnEnter()
        {
            OnGainFocus();
            LifecicleCallbacks.ForEach(c => c.OnEnter());
            AnimateIn();

        }

        public virtual void OnGainFocus()
        {
            Util.SetInputController?.Invoke(InputController);
        }

        public virtual void OnLeave()
        {
            OnLoseFocus();
            LifecicleCallbacks.ForEach(c => c.OnLeave());
            AnimateOut();
        }

        public virtual void OnLoseFocus() { }
        #endregion

        #region Navigation
        protected virtual bool OnGoToNext(string key) { return false; }

        public virtual bool OnPop()
        {
            if (nextActivities == null)
            {
                return false;
            }
            if (nextActivities.TryGetValue(Util.BackKey, out string backActivity))
            {
                Parent.PopBackStackTill(backActivity);
                return true;
            }
            return false;
        }

        public void GoToNext(string key)
        {
            if (!IsRunning)
                return;

            if (!OnGoToNext(key))
            {
                if (key.IsBackKey())
                {
                    Parent.PopBackStack();
                }
                else if (nextActivities == null)
                {
                    Debug.Log("Navigation of activity " + name + " was not initialized");
                }
                else if (nextActivities.TryGetValue(key, out string next))
                {
                    Parent.RunActivity(next);
                }
                else
                {
                    Debug.Log("Event " + key + " was not found");
                }
            }
        }
        #endregion

        #region Animations

        private void AnimateIn()
        {
            if(DoNotHide && IsVisible)
            {
                OnInAnimationComplete();
            }else
                Animate(InAnimation, true);
        }

        private void AnimateOut()
        {
            if (DoNotHide)
                OnOutAnimationComplete();
            else
                Animate(OutAnimation, false);
        }

        private void Animate(string animation, bool isIn)
        {
            if (AnimationComponents.Count == 0 || Equals(animation, "none"))
            {
                SetVisible(isIn);
                if (!animation.IsNullEmptyOrEqual("none"))
                    Debug.Log("No animation component as been assigned to activity: " + name);
                return;
            }

            foreach (var a in AnimationComponents)
            {
                if (isIn ? a.InAnimate(animation,this) : a.OutAnimate(animation,this))
                {
                    return;
                }
            }
            Debug.Log("No animation component could process the animation: " + animation + " on activity: " + name);
        }

        public virtual void OnInAnimationComplete()
        {
            LifecicleCallbacks.ForEach(c => c.OnEnterComplete());
        }

        public virtual void OnOutAnimationComplete()
        {
            LifecicleCallbacks.ForEach(c => c.OnLeaveComplete());
        }

        #endregion

        #region Helper Methods
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public virtual bool IsVisible => gameObject.activeSelf;

        internal void SetNavData(Dictionary<string, string> nextActivities)
        {
            this.nextActivities = nextActivities;
            SetVisible(DoNotHide);
        }

        #endregion
    }
}
