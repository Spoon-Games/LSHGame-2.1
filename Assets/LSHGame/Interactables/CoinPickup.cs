using SceneM;
using UnityEngine;

namespace LSHGame.General.Environment
{
    public class CoinPickup : DataPersistBehaviour
    {

        [SerializeField] private AudioClip coinPickUpSFX;
        [SerializeField] private InventoryItem inventoryItem;

        private bool destroied = false;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            Inventory.Add(inventoryItem);
            AudioSource.PlayClipAtPoint(coinPickUpSFX, Camera.main.transform.position);
            destroied = true;
            //gameObject.SetActive(false);
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }

        public override void LoadData(SceneM.Data data)
        {
            if(data is Data<bool> storedData && storedData.value)
            {
                gameObject.SetActive(false);
                //gameObject.SetActive(false);
            }
        }

        public override SceneM.Data SaveData()
        {
            return new Data<bool>(destroied);
        }
    } 
}
