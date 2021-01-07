using SceneM;
using UnityEngine;

namespace LSHGame
{
    public class VanishingPlatform : MonoBehaviour
    {
        [SerializeField]
        private Transform spriteTransform;

        [SerializeField]
        private Transform destructableTransform;

        [SerializeField]
        private Transform destructablePartsParent;

        private void Awake()
        {
            LevelManager.OnResetLevel += Reset;
        }

        private void Reset()
        {
            spriteTransform.gameObject.SetActive(true);
            destructableTransform.gameObject.SetActive(false);
            
            foreach(Transform child in destructablePartsParent)
            {
                child.localPosition = Vector3.zero;
                child.localRotation = Quaternion.identity;
            }
        }

        public void Vanish()
        {
            spriteTransform.gameObject.SetActive(false);
            destructableTransform.gameObject.SetActive(true);
        }
    }

}