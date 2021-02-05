using LogicC;
using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class FallingRock : LogicDestination
    {
        [SerializeField]
        private float stallTime = 1;

        [SerializeField]
        private GameObject hitArea;

        private Animator animator;
        private EffectsController effectsController;
        private Rigidbody2D rb;
        private Vector3 startPosition;

        private bool wasActivated = false;
        private bool isFalling = false;
        private float impactThreasholdTimer = float.NegativeInfinity;

        private bool wasDestroyed = false;

        protected override void Awake()
        {
            base.Awake();
            startPosition = transform.position;
            animator = GetComponent<Animator>();
            effectsController = GetComponent<EffectsController>();
            rb = GetComponent<Rigidbody2D>();

            LevelManager.OnResetLevel += Reset;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if(!wasActivated)
            {
                wasActivated = true;
                ActivateRock();
            }
        }

        private void ActivateRock()
        {
            if(stallTime <= 0)
            {
                StartFallRock();
            }

            animator.SetBool("IsStalling", true);
            effectsController.TriggerEffect("StallingEffect");
            TimeSystem.Delay(stallTime,(t) => StartFallRock(),true);
        }

        private void StartFallRock()
        {
            animator.SetBool("IsStalling", false);
            effectsController.StopEffect("StallingEffect");
            effectsController.TriggerEffect("FallingEffect");
            rb.bodyType = RigidbodyType2D.Dynamic;
            hitArea.SetActive(true);
            isFalling = true;
            impactThreasholdTimer = Time.fixedTime + 0.2f;
        }

        private void FixedUpdate()
        {
            if(isFalling && rb.velocity.y >= 0 && Time.fixedTime > impactThreasholdTimer)
            {
                effectsController.StopEffect("FallingEffect");
                effectsController.TriggerEffect("ImpactEffect");
                rb.bodyType = RigidbodyType2D.Static;
                isFalling = false;
                hitArea.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            wasDestroyed = true;
        }

        private void Reset()
        {
            if (!wasDestroyed)
            {
                animator.SetBool("IsStalling", false);
                effectsController.StopEffect("StallingEffect");
                effectsController.StopEffect("FallingEffect");
                rb.bodyType = RigidbodyType2D.Static;
                hitArea.SetActive(false);
                wasActivated = false;
                isFalling = false;

                transform.position = startPosition;
            }
        }
    } 
}
