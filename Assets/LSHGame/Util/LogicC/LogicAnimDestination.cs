using LogicC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    [AddComponentMenu("LogicC/Boolean/LogicAnimDestiantion")]
    [RequireComponent(typeof(Animator))]
    public class LogicAnimDestination : LogicDestination
    {
        [SerializeField]
        [NodeEditorField]
        private string boolName;

        private Animator animator;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
        }

        protected override void InputPortUpdate()
        {
            base.InputPortUpdate();

            animator.SetBool(boolName, inputPort.GetFirst());
        }
    }

}