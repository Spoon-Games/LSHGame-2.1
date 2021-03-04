using LSHGame.PlayerN;
using SceneM;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame
{
    [RequireComponent(typeof(PlayerFollower))]
    public class CoinPickup : DataPersistBehaviour
    {
        [SerializeField] private InventoryItem inventoryItem;
        [SerializeField]
        private UnityEvent onPickUp;

        private PlayerFollower followComponent;

        private bool destroied = false;

        private bool active = false;
        private Vector3 startPosition;

        private void Awake()
        {
            followComponent = GetComponent<PlayerFollower>();
            LevelManager.OnResetLevel += OnReset;
            startPosition = transform.position;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            Activate();
        }

        private void Activate()
        {
            if (!destroied)
            {
                active = true;
                followComponent.Active = true;
                TryPickup();
            }
        }

        private void Update()
        {
            TryPickup();
        }

        private void TryPickup()
        {
            if(!destroied && active && Player.Instance.IsSaveGround)
            {
                followComponent.Active = false;
                Inventory.Add(inventoryItem);
                destroied = true;
                Destroy(gameObject);
                onPickUp?.Invoke();
            }
        }

        public override void LoadData(SceneM.Data data)
        {
            if(data is Data<bool> storedData && storedData.value)
            {
                Destroy(gameObject);
                //gameObject.SetActive(false);
            }
        }

        public override SceneM.Data SaveData()
        {
            return new Data<bool>(destroied);
        }

        private void OnDestroy()
        {
            LevelManager.OnResetLevel -= OnReset;
        }

        private void OnReset()
        {
            if (!destroied)
            {
                followComponent.Active = false;
                transform.position = startPosition;
            }
        }
    } 
}
