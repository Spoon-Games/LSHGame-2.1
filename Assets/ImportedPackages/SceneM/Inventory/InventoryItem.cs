using UnityEngine;

namespace SceneM
{
    [CreateAssetMenu(fileName = "InventoryItem",menuName ="SceneM/InventoryItem",order = 0)]
    public class InventoryItem : ScriptableObject
    {
        public string ItemName;

        public PersistenceType PersistenceType;
    }
}
