using LSHGame.Util;
using SceneM;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Activity))]
    public abstract class BaseCharacterView<D,C> : Singleton<C>, IActivityLifecicleCallback where D : BaseDialog where C : BaseCharacterView<D,C>
    {
        [SerializeField]
        protected Button furtherButton;

        protected D Dialog { get; private set; }

        private Activity _activity;
        protected Activity Activity
        {
            get
            {
                if (_activity == null)
                    _activity = GetComponent<Activity>();
                return _activity;
            }
        }

        protected virtual string ActivityTransitionName => "Start" + name;

        public override void Awake()
        {
            furtherButton?.onClick.AddListener(GetNext);

            GameInput.OnFurther += GetNext;
        }

        public void ShowDialog(D dialog)
        {
            Dialog = dialog;
            ResetView();
            Activity.Parent.GoToNext(ActivityTransitionName);
        }

        protected virtual void ResetView()
        {
            Dialog.Reset();
        }

        protected virtual void GetNext()
        {

        }


        public void End()
        {
            Activity.Parent.PopBackStack();
        }



        protected virtual void OnDestroy()
        {
            GameInput.OnFurther -= GetNext;
        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnEnterComplete()
        {
            GetNext();
        }

        public virtual void OnLeave()
        {
        }

        public virtual void OnLeaveComplete()
        {
        }
    }
}
