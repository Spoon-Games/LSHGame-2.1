using SceneM;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.General.Environment
{
    public class CoinPickup : DataPersistBehaviour
    {
        [SerializeField]
        private InventoryItem inventoryItem;

        [SerializeField]
        private UnityEvent onActivateEvent;

        private bool activated = false;

        public void Activate()
        {
            if (!activated)
            {
                onActivateEvent?.Invoke();
                Inventory.Add(inventoryItem);
                activated = true;
                Destroy(gameObject);
            }
        }

        public override void LoadData(SceneM.Data data)
        {
            if(data is Data<bool> storedData && storedData.value)
            {
                activated = true;
                Destroy(gameObject);
                //gameObject.SetActive(false);
            }
        }

        public override SceneM.Data SaveData()
        {
            return new Data<bool>(activated);
        }
    } 
}
