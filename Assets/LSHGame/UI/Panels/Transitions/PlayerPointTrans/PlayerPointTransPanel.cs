using DG.Tweening;
using LSHGame.PlayerN;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Image))]
    public class PlayerPointTransPanel : TransitionPanel
    {
        [SerializeField]
        private Vector2 defaultOrigin = new Vector2(0.7f, 0.6f);

        [SerializeField]
        private string objectPointTag;

        private Image image;

        private const string T_NAME = "_T";
        private const string ORIGIN_NAME = "_ORIGIN";

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        protected override void OnSwitchState(State state)
        {
            switch (state)
            {
                case State.Idle:
                    image.material.SetFloat(T_NAME, 0);
                    break;
                case State.Start:
                    image.material.SetFloat(T_NAME, 0);
                    image.material.SetVector(ORIGIN_NAME, GetOrigin());
                    image.material.DOFloat(1, T_NAME, PanelName.StartDurration);
                    break;
                case State.Middle:
                    break;
                case State.End:
                    image.material.SetVector(ORIGIN_NAME, GetOrigin());
                    image.material.DOFloat(0, T_NAME, PanelName.EndDurration);
                    break;
            }
        }

        protected virtual Vector2 GetOrigin()
        {
            var point = GameObject.FindGameObjectWithTag(objectPointTag);
            if (point != null)
            {
                Vector3 worldPos = point.transform.position;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, Camera.main.WorldToScreenPoint(worldPos), Camera.main, out Vector2 origin);

                return origin;
            }
            return defaultOrigin;
        }
    }
}