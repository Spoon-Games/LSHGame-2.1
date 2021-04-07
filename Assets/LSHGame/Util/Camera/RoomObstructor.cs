using DG.Tweening;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(Renderer))]
    public class RoomObstructor : ColTrigger
    {
        //[SerializeField]
        //private bool VisibleOnAwake = false;

        [SerializeField] private float easeInTime = 1;
        [SerializeField] private Ease easeIn = Ease.OutQuad;

        [SerializeField] private float easeOutTime = 1;
        [SerializeField] private Ease easeOut = Ease.InQuad;

        [Space]
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera cam;

        [SerializeField]
        private bool ConfinePlayerCamera = false;

        private Renderer ren;

        private bool _visible;
        public bool Visible { get => _visible; set => SetVisible(value); }

        protected override void Awake()
        {
            ren = GetComponent<Renderer>();
            ren.enabled = true;

            base.Awake();
        }

        private void Start()
        {
            SetVisible(base.IsTouchingLayers(),true);
        }

        protected override void OnTriggerEntered()
        {
            base.OnTriggerEntered();
            SetVisible(true);
        }

        protected override void OnTriggerExited()
        {
            base.OnTriggerExited();
            SetVisible(false);
        }

        private void SetVisible(bool visible, bool init = false)
        {
            if (init)
            {
                _visible = visible;

                Color color = ren.material.color;
                if (visible)
                    color.a = 0;
                else
                    color.a = 1;
                ren.material.color = color;
                cam?.gameObject.SetActive(visible);

                if (cam == null && ConfinePlayerCamera && visible)
                    PlayerCameraConfinerManager.Instance.SetConfiner(col);

            }
            else if (_visible != visible)
            {
                _visible = visible;
                if (visible)
                {
                    ren.material.DOFade(0, easeInTime).SetEase(easeIn);
                }
                else
                {
                    ren.material.DOFade(1, easeOutTime).SetEase(easeOut);
                }

                cam?.gameObject.SetActive(visible);

                if (cam == null && ConfinePlayerCamera)
                {
                    if (visible)
                        PlayerCameraConfinerManager.Instance.SetConfiner(col);
                    else
                        PlayerCameraConfinerManager.Instance.SetConfinerIfMatched(null, col);
                }

            }
        }
    }
}
