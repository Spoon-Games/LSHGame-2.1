using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    [CreateAssetMenu(fileName = "Prefab Tile",menuName ="Tiles/Prefab Tile",order = 11)]
    public class PrefabTile : Tile
    {

        [SerializeField]
        private GameObject[] prefabs = new GameObject[0];

        [SerializeField]
        private Sprite previewSprite;

        [SerializeField]
        public Vector2 pivot = Vector2.zero;

        private Sprite previewInstance;
        // This refreshes itself and other RoadTiles that are orthogonally and diagonally adjacent
        public override void RefreshTile(Vector3Int location, ITilemap tilemap)
        {
            base.RefreshTile(location, tilemap);
        }
        // This determines which sprite is used based on the RoadTiles that are adjacent to it and rotates it to fit the other tiles.
        // As the rotation is determined by the RoadTile, the TileFlags.OverrideTransform is set for the tile.
        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            //Debug.Log("GetTileData: " + tileName);

            tileData.sprite = previewInstance;
            tileData.color = Color.clear;
            var m = tileData.transform;
            tileData.transform = Matrix4x4.TRS((Vector3)pivot,Quaternion.identity,Vector3.one) * m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Grid;
        }

        internal GameObject GetPrefab()
        {
            if (prefabs.Length == 0)
            {
                Debug.Log("Prfab of PrefabTile " + name + " was not assigned");
                return null;
            }
            else if (prefabs.Length == 1)
                return prefabs[0];
            else
                return prefabs[Random.Range(0, prefabs.Length - 1)];
        }

        private void OnEnable()
        {
            previewInstance = GetSprite();
        }

        private void OnValidate()
        {
            previewInstance = GetSprite();
        }

        private Sprite GetSprite()
        {
            if(previewSprite != null)
            {
                return previewSprite;
            }
            else if (prefabs.Length != 0 && prefabs[0] != null && prefabs[0].TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
            {
                return renderer.sprite;
            }
            else
            {
                Texture2D tex = new Texture2D(100,100);
               
                
                return Sprite.Create(tex, new Rect(Vector2.zero,new Vector2(100,100)), Vector2.one / 2);
            }
        }

        //private Sprite FormateSprite(Sprite sprite)
        //{
        //    float pixelPerUnit = sprite.pixelsPerUnit * Mathf.Max(1, sprite.rect.size.x / unitSize.x, sprite.rect.size.y / unitSize.y);
        //    return Sprite.Create(sprite.texture, new Rect(Vector2.zero, sprite.rect.size), Vector2.one / 2, pixelPerUnit);
        //}

//#if UNITY_EDITOR
//        // The following is a helper that adds a menu item to create a RoadTile Asset
//        [MenuItem("Assets/Create/LSHGame/PrefabTile")]
//        public static void CreateRoadTile()
//        {
//            string path = EditorUtility.SaveFilePanelInProject("Save Prefab Tile", "New Prefab Tile", "Asset", "Save Prefab Tile", "Assets");
//            if (path == "")
//                return;
//            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PrefabTile>(), path);
//        }
//#endif
    }
}