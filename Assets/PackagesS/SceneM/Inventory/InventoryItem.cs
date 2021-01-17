using UnityEngine;

namespace SceneM
{
    [CreateAssetMenu(fileName = "InventoryItem",menuName ="SceneM/InventoryItem",order = 0)]
    public class InventoryItem : ScriptableObject
    {
        public string ItemName;

        public string ItemType;

        public PersistenceType PersistenceType;
    }

    public static class InvetoryTags
    {
        public const string SCORE_TAG = "score";
    }
}
