using SceneM;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class Transition : BasePanel<TransitionInfo, Transition, TransitionManager>
    {
        [SerializeField]
        protected Slider slider;

        protected Animator Animator { get; private set; }

        //public bool InStart { get; private set; }

        //public bool InMiddle { get; private set; }

        //protected float Progress { get; private set; } = 0;

        private float timeMark = float.PositiveInfinity;
        private Func<float> getProgress;

        private enum State { Idle, Start, Middle, End }
        private State currentState;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void StartTransition(Func<float> getProgress)
        {

            Animator.SetTrigger("TriggerStart");
            this.getProgress = getProgress;
            currentState = State.Start;
            timeMark = panelName.StartDurration + Time.time;
        }

        private void Update()
        {
            if (currentState != State.Idle)
            {

                SetProgress(getProgress.Invoke());

                if (Time.time >= timeMark && currentState == State.Start)
                {
                    currentState = State.Middle;
                    timeMark = Time.time + panelName.MinMiddleDurration;
                    Animator.SetTrigger("TriggerMiddle");
                }

                if (currentState == State.Middle && Time.time >= timeMark && getProgress.Invoke() >= 1)
                {
                    currentState = State.End;
                    Animator.SetTrigger("TriggerEnd");
                    timeMark = Time.time + panelName.EndDurration;
                }

                if (currentState == State.End && Time.time >= timeMark)
                {
                    currentState = State.Idle;
                    Animator.SetTrigger("TriggerIdle");
                    Parent.ShowPanel(null);
                }
            }
        }

        private void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            Animator.SetFloat("Progress", progress);
            slider?.SetValueWithoutNotify(progress);
        }

        //public void StartTransition(bool automatic = true)
        //{
        //    Animator = GetComponent<Animator>();
        //    Animator.SetTrigger("Start");

        //    if (!automatic)
        //    {
        //        Animator.SetBool("IsEndAfterDurration", false);
        //    }

        //    StartTrans();
        //}

        //public void EndTransition()
        //{
        //    Animator.SetBool("IsEndAfterDurration", true);
        //}

        //public void SetProgress(float progress)
        //{
        //    Progress = Mathf.Clamp01(progress);
        //    OnSetProgress(progress);
        //}

        //private void StartTrans() {
        //    InStart = true;
        //    OnStart();
        //}

        //protected virtual void OnStart() { }

        //internal void EnterMiddle()
        //{
        //    InStart = false;
        //    InMiddle = true;
        //    OnEnterMiddle();
        //}

        //protected virtual void OnEnterMiddle() {}

        //protected virtual void OnSetProgress(float progress)
        //{
        //    if (slider != null)
        //        slider.value = progress;
        //}

        //internal void ExitMiddle()
        //{
        //    InMiddle = false;
        //    OnExitMiddle();
        //}

        //protected virtual void OnExitMiddle() { }

        //internal void End()
        //{
        //    GetComponent<Panel>().Parent.ShowPanel("");
        //    OnEnd();
        //}

        //protected virtual void OnEnd() { }
    }
}
