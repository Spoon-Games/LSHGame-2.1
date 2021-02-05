using SceneM;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.General.Environment
{
    public class CoinPickup : DataPersistBehaviour
    {
        [SerializeField] private InventoryItem inventoryItem;
        [SerializeField]
        private UnityEvent onPickUp;

        private bool destroied = false;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!destroied)
            {
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
    } 
}
