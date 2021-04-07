using LSHGame.Util;
using SceneM;
using TagInterpreterR;
using TMPro;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Activity))]
    public abstract class BaseCharacterView<D, C> : Singleton<C>, IActivityLifecicleCallback where D : BaseDialog where C : BaseCharacterView<D, C>
    {
        #region Attributes
        [SerializeField]
        protected Button furtherButton;

        private bool _isFurther = false;
        [SerializeField]
        protected TMP_Text furtherButtonTextField;
        [SerializeField]
        protected string defaultFurtherText = "Weiter";

        [SerializeField]
        private float typeSpeed = 0.1f;

        [SerializeField]
        private bool holdForLastStringTag = true;

        private float currentTypeSpeed;
        private float typeProgress = 0;

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

        protected TagChain TagChain { get; private set; }
        //protected BaseTag CurrentTag => TagChain.Current;

        protected bool Active { get; private set; } = false;

        private string currentWrittenText = "";
        private string currentFractionText = "";

        protected bool isDestroied = false;
        #endregion

        #region Init
        public override void Awake()
        {
            furtherButton?.onClick.AddListener(OnFurther);

            GameInput.OnFurther += OnFurther;
        }
        #endregion

        #region Update
        private void Update()
        {
            if (Active)
            {
                int protection = 0;
                while (TagChain.GetNext(skip:_isFurther) && protection < 1000) { protection++; }
                if (protection == 1000)
                    Debug.Log("Protection");
                if (TagChain.IsEnd)
                    Deactivate();
            }
        }
        #endregion

        #region TagChain Callbacks

        protected virtual void OnCreateTag(BaseTag tag)
        {
            if (tag is StringTag)
            {
                typeProgress = 0;
            }
            else if (tag is ButtonTag buttonTag)
            {
                ConsumeIsFurther();
                SetFurtherButtonText(buttonTag.ButtonName);
            }
            else if (tag is PageTag pageTag && pageTag.IsButton)
            {
                ConsumeIsFurther();
                SetFurtherButtonText(pageTag.ButtonName);
            }
            else if (tag is ActionTag actionTag)
            {
                Dialog.ActionCallback?.Invoke(actionTag.ActionName);
            }
            else if (tag is TagReference tagReference)
            {
                AddTMPTag(tagReference);
            }
        }


        protected virtual void OnActivateTag(BaseTag tag)
        {
            if (tag is SpeedTag speedTag)
            {
                currentTypeSpeed = speedTag.Value;
            }
        }

        protected virtual bool OnUpdateTag(BaseTag tag)
        {
            if (tag is StringTag stringTag)
            {
                if (currentTypeSpeed > 0)
                {
                    typeProgress += currentTypeSpeed * Time.deltaTime / ((float)stringTag.Text.Length);
                    typeProgress = Mathf.Clamp01(typeProgress);

                    if (typeProgress < 1)
                    {
                        string s = stringTag.Text.Substring(0, (int)(stringTag.Text.Length * typeProgress));
                        AddFractionText(s);
                        return true;
                    }
                }
                AddFractionText(stringTag.Text);

                if(holdForLastStringTag && TagChain.Index == TagChain.TagCount - 1)
                {
                    if (!ConsumeIsFurther())
                        return true;
                }
            }
            else if (tag is ButtonTag buttonTag)
            {
                if (!ConsumeIsFurther())
                    return true;
            }
            else if (tag is PageTag pageTag && pageTag.IsButton)
            {
                if (!ConsumeIsFurther())
                    return true;
            }

            return false;
        }
        protected virtual void OnDeactivateTag(BaseTag tag) { }
        protected virtual void OnDestroyTag(BaseTag tag, bool returnToDefault)
        {
            if (tag is StringTag stringTag)
            {
                AddFinishedText(stringTag.Text);
            }
            else if (returnToDefault && tag is SpeedTag)
            {
                currentTypeSpeed = typeSpeed;
            }
            else if (tag is ButtonTag)
            {
                SetFurtherButtonText(defaultFurtherText);
            }
            else if (tag is PageTag pageTag)
            {
                if (pageTag.IsButton)
                    SetFurtherButtonText(defaultFurtherText);
                ClearText();
            }
            else if (tag is TagReference tagReference)
            {
                AddTMPEndTag(tagReference);
            }
        }

        #endregion

        #region Livecycle
        public void Activate(D dialog)
        {
            if (!Active)
            {
                ResetView();
                Dialog = dialog;
                TagChain = Dialog.Parse();
                TagChain.OnCreate += OnCreateTag;
                TagChain.OnActivate += OnActivateTag;
                TagChain.OnUpdate += OnUpdateTag;
                TagChain.OnDeactivate += OnDeactivateTag;
                TagChain.OnDestroy += OnDestroyTag;

                Active = true;

                Activity.Parent.GoToNext(ActivityTransitionName);

                //Debug.Log("Activate");
                //Debug.Log("TagChain: " + TagChain.ToString());
            }
            else
                Debug.Log("CharacterView already active");
        }

        public void Deactivate()
        {
            if (Active)
            {
                if (TagChain != null)
                {
                    TagChain.OnCreate -= OnCreateTag;
                    TagChain.OnActivate -= OnActivateTag;
                    TagChain.OnUpdate -= OnUpdateTag;
                    TagChain.OnDeactivate -= OnDeactivateTag;
                    TagChain.OnDestroy -= OnDestroyTag;

                    //Debug.Log("Deactivate");
                }

                Active = false;

                if (!isDestroied)
                    Activity.Parent.PopBackStack();
            }
            else
                Debug.Log("CharacterView was not active");
        }

        #endregion

        #region Destroy

        protected virtual void OnDestroy()
        {
            isDestroied = true;
            if (Active)
                Deactivate();
            GameInput.OnFurther -= OnFurther;
        }

        #endregion

        #region Activity Callbacks
        public virtual void OnEnter()
        {

        }

        public virtual void OnEnterComplete()
        {
            //GetNext();
            TagChain.Start();
        }

        public virtual void OnLeave()
        {
        }

        public virtual void OnLeaveComplete()
        {
        }
        #endregion

        #region Text
        protected void AddFractionText(string fraction)
        {
            SetText(currentWrittenText + fraction);

            if (!Equals(fraction, currentFractionText))
            {
                currentFractionText = fraction;
                if (!string.IsNullOrEmpty(fraction))
                {
                    OnAddFractionChar(fraction[fraction.Length - 1]);
                }
            }
        }

        protected virtual void OnAddFractionChar(char c)
        {

        }

        protected void AddFinishedText(string finished)
        {
            currentWrittenText += finished;
            SetText(currentWrittenText);
        }

        protected void ClearText()
        {
            currentWrittenText = "";
            SetText(currentWrittenText);
        }

        private void AddTMPTag(TagReference tag)
        {
            if (IsTMPTag(tag.Name))
            {
                AddFinishedText(GetTMPString(tag));
            }
        }

        private void AddTMPEndTag(TagReference tag)
        {
            if (IsTMPTag(tag.Name))
            {
                AddFinishedText(GetTMPEndString(tag));
            }
        }

        private bool IsTMPTag(string name)
        {
            return Equals(name, "align") || Equals(name, "color") || Equals(name, "alpha") || Equals(name, "b") || Equals(name, "i")
                || Equals(name, "font") || Equals(name, "cspace") || Equals(name, "size") || Equals(name, "style") || Equals(name, "lowercase")
                || Equals(name, "uppercase") || Equals(name, "smallcaps") || Equals(name, "sprite") || Equals(name, "u") || Equals(name, "s");
        }

        private string GetTMPEndString(TagReference tag)
        {
            return "</" + tag.Name + ">";
        }

        private string GetTMPString(TagReference tag)
        {
            string s = "<" + tag.Name;
            if (tag.Attributes.TryGetValue("value", out string defalutAttr))
            {
                s += "=\"" + defalutAttr + "\"";
            }
            foreach (var a in tag.Attributes)
            {
                if (a.Key != "value")
                {
                    s += " " + a.Key + "=\"" + a.Value + "\"";
                }
            }
            s += ">";
            return s;
        }

        protected virtual void SetText(string text)
        {

        }
        #endregion

        #region Helper Methods

        private void OnFurther()
        {
            _isFurther = true;
        }

        protected bool ConsumeIsFurther()
        {
            if (_isFurther)
            {
                //Debug.Log("Consume Is Further");
                _isFurther = false;
                return true;
            }
            return false;
        }
        
        private void SetFurtherButtonText(string text)
        {
            if (!string.IsNullOrEmpty(text) && furtherButtonTextField)
                furtherButtonTextField.text = text;
        }

        protected virtual void ResetView()
        {
            currentTypeSpeed = typeSpeed;
            _isFurther = false;
            ClearText();
        }

        #endregion
    }
}
