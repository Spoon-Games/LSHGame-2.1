#if UNITY_EDITOR
using UnityEditor;
#endif
using SceneM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    public class SubstanceManager : Singleton<SubstanceManager>
    {
        [SerializeField]
        private Substance[] serializedSubstances = new Substance[0];

        private Dictionary<TileBase, List<Substance>> tileBasedSubstances = new Dictionary<TileBase, List<Substance>>();
        private Dictionary<string, Substance> nameBasedSubstances = new Dictionary<string, Substance>();

        private static HashSet<ISubstance> substances = new HashSet<ISubstance>();

        //private List<Rect> debugRects = new List<Rect>();

        public override void Awake()
        {
            base.Awake();
            LoadSubstances();
        }

        public static void ReciveSubstanceData(BoxCollider2D collider2D, IDataReciever reciever, ISubstanceFilter filter, LayerMask layerMask, out bool touch)
        {
            List<Collider2D> colliders = new List<Collider2D>();
                collider2D.OverlapCollider(GetContactFilter(layerMask), colliders);
            //Debug.Log("Colliders: " + colliders.Count);
            touch = colliders.Count > 0;
            //Debug.Log("Colliders: " + colliders.Count);
            //if (touch)
               // Debug.Log("Touching: " + colliders[0].name);
            ReciveSubstanceData(new Rect() { size = collider2D.size, center = collider2D.offset }.LocalToWorldRect(collider2D.transform), reciever, filter, colliders, GetContactFilter(layerMask));
        }

        public static void ReciveSubstanceData(Rect rect,IDataReciever reciever, ISubstanceFilter filter, LayerMask layerMask,out bool touch,bool noTouchOnTriggers = false)
        {
            List<Collider2D> colliders = GetTouchRect(rect, GetContactFilter(layerMask));
            if (noTouchOnTriggers)
            {
                touch = false;
                foreach(var col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        touch = true;
                        break;
                    }
                }
            }else
                touch = colliders.Count > 0;
            //Debug.Log("Colliders: " + colliders.Count);
            ReciveSubstanceData(rect, reciever, filter, colliders, GetContactFilter(layerMask));
        }

        private static void ReciveSubstanceData(Rect rect, IDataReciever reciever, ISubstanceFilter filter, List<Collider2D> colliders,ContactFilter2D cf)
        {
            substances.Clear();

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<Tilemap>(out Tilemap tilemap))
                {
                    GetSubstancesFromTilemap(rect, tilemap,collider,cf, substances, filter);
                }

                foreach (var provider in collider.GetComponents<SubstanceProvider>())
                {
                    provider.Substance.AddToSet(substances, filter);
                }

                GetSubstancesFromTag(collider.gameObject.tag, substances, filter);
            }


            foreach (var substance in substances)
            {
                //Debug.Log("Substance ReciveData");
                substance.RecieveData(reciever);
            }

            substances.Clear();
        }

            private static void GetSubstancesFromTilemap(Rect rect, Tilemap tilemap,Collider2D collider, ContactFilter2D cf,HashSet<ISubstance> substances, ISubstanceFilter filter)
        {
            RectInt tileRect = new RectInt() { min = (Vector2Int)tilemap.WorldToCell(rect.min), max = (Vector2Int)tilemap.WorldToCell(rect.max) };
            tileRect.height += 1;
            tileRect.width += 1;
            //Debug.Log("TileRect: " + tileRect);
            foreach (var tilePos in tileRect.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile((Vector3Int)tilePos);

                //if (tile != null)
                //    Debug.Log("Try Add Tile: " + tile.name);

                if (tile != null && Instance.tileBasedSubstances.TryGetValue(tile, out List<Substance> subs))
                {
                    if (rect.Overlap(tilemap.GetRectAtPos(tilePos).InsetRect(0.03f), out Rect overlap)){

                        if (collider.IsTouchingRect(overlap, cf))
                        {
                            //Debug.Log("Add TileBase: " + tile.name);
                            foreach (var s in subs)
                                s.AddToSet(substances, filter);
                        }
                        //Instance.debugRects.Add(overlap);
                    }
                    
                }
            }
        }

        private static void GetSubstancesFromTag(string tag, HashSet<ISubstance> substances, ISubstanceFilter filter)
        {
            while (!string.IsNullOrEmpty(tag) && !Equals(tag,"Untagged"))
            {
                int startMat = tag.IndexOf("m:");
                if (startMat == -1)
                    return;

                tag = tag.Substring(startMat + 2);
                int endMat = tag.IndexOf(' ');

                string substanceName = tag;
                if (endMat != -1)
                {
                    substanceName = tag.Substring(0,endMat);
                    tag = tag.Substring(endMat + 1);
                }

                if (substanceName != "" && Instance.nameBasedSubstances.TryGetValue(substanceName, out Substance s))
                {
                    s.AddToSet(substances,filter);
                }

                if (endMat == -1)
                    break;
            }
        }

        private static List<Collider2D> GetTouchRect(Rect rect, ContactFilter2D contactFilter)
        {
            List<Collider2D> collider2Ds = new List<Collider2D>();
            Physics2D.OverlapBox(rect.center, rect.size, 0, contactFilter, collider2Ds);
            //Debug.Log("IsTouchingLayerRect: Center: " + ((Vector3)rect.center + transform.position) + "\n Size: " + rect.size + "\nLayers: " + layers.value + " isTouching: "+isTouching);

            return collider2Ds;
        }

        private static ContactFilter2D GetContactFilter(LayerMask layers)
        {
            return new ContactFilter2D() { useTriggers = true, layerMask = layers, useLayerMask = true };
        }

        private void LoadSubstances()
        {
#if UNITY_EDITOR
            LoadSubstancePrefabs();
#endif

            foreach (var s in serializedSubstances)
            {
                foreach (TileBase tileBase in s.tilesOfSubstance)
                    AddTileSubsEntry(s, tileBase);
            }

            foreach (var s in serializedSubstances)
            {
                if (nameBasedSubstances.ContainsKey(s.name))
                {
                    Debug.Log("You can not name two Substances the same. This will lead to unexpected behaviour with tags.");
                    continue;
                }
                nameBasedSubstances.Add(s.name, s);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Load Prefabs")]
        private void LoadSubstancePrefabs()
        {
            List<Substance> substances = new List<Substance>();
            //var paths = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Substance).ToString());
            //foreach (var path in paths)
            //{
            //    substances.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<Substance>(path));
            //}

            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Debug.Log("Path: " + path);

                Substance s = AssetDatabase.LoadAssetAtPath<Substance>(path);
                if (s != null)
                    substances.Add(s);
            }
            serializedSubstances = substances.ToArray();
        }

#endif

        //private void OnDrawGizmos()
        //{
        //    foreach(Rect r in debugRects)
        //    {
        //        Gizmos.color = Color.green;
        //        Gizmos.DrawCube(r.center, r.size);
        //    }
        //    debugRects.Clear();
        //}

        private void AddTileSubsEntry(Substance s, TileBase tileBase)
        {
            if (tileBasedSubstances.TryGetValue(tileBase, out List<Substance> subs))
            {
                if (!subs.Contains(s))
                    subs.Add(s);
            }
            else
            {
                tileBasedSubstances.Add(tileBase, new List<Substance>() { s });
            }
        }
    }
}
