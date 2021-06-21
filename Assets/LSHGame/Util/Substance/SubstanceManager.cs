//#define DEBUG_THIS

#if UNITY_EDITOR
using UnityEditor;
#endif
using SceneM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


namespace LSHGame.Util
{
    [CreateAssetMenu(menuName ="LSHGame/Editor/SubstanceManager")]
    public class SubstanceManager : SceneM.ScriptableSingleton<SubstanceManager>
    {
        #region Attributes
        [SerializeField]
        private SubstancePointer[] serializedPointers = new SubstancePointer[0];

        private Dictionary<TileBase, List<Substance>> tileBasedSubstances = new Dictionary<TileBase, List<Substance>>();
        private Dictionary<string, Substance> nameBasedSubstances = new Dictionary<string, Substance>();

        private static HashSet<ISubstance> substances = new HashSet<ISubstance>();


#if DEBUG_THIS
        private List<Tuple<Rect, Color>> debugRects = new List<Tuple<Rect, Color>>();
        private List<Tuple<Vector3, Color>> debugPoints = new List<Tuple<Vector3, Color>>(); 
#endif 
        #endregion

        #region Init
        protected override void Awake()
        {
            base.Awake();
            LoadSubstances();
        } 
        #endregion

        #region Retrive Substances
        public static void RetrieveSubstances(BoxCollider2D collider2D, SubstanceSet set, ISubstanceFilter filter, LayerMask layerMask, out bool touch)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            collider2D.OverlapCollider(GetContactFilter(layerMask), colliders);
            //Debug.Log("Colliders: " + colliders.Count);
            touch = colliders.Count > 0;
            //Debug.Log("Colliders: " + colliders.Count);
            //if (touch)
            // Debug.Log("Touching: " + colliders[0].name);
            RetrieveSubstances(new Rect() { size = collider2D.size, center = collider2D.offset }.LocalToWorldRect(collider2D.transform), set, filter, colliders, GetContactFilter(layerMask));
        }

        public static void RetrieveSubstances(Rect rect, SubstanceSet set, ISubstanceFilter filter, LayerMask layerMask, out bool touch, bool noTouchOnTriggers = false)
        {
            List<Collider2D> colliders = GetTouchRect(rect, GetContactFilter(layerMask));
            if (noTouchOnTriggers)
            {
                touch = false;
                foreach (var col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        touch = true;
                        break;
                    }
                }
            }
            else
                touch = colliders.Count > 0;
            //Debug.Log("Colliders: " + colliders.Count);
            RetrieveSubstances(rect, set, filter, colliders, GetContactFilter(layerMask));
        }

        private static void RetrieveSubstances(Rect rect, SubstanceSet set, ISubstanceFilter filter, List<Collider2D> colliders, ContactFilter2D cf)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<Tilemap>(out Tilemap tilemap))
                {
                    GetSubstancesFromTilemap(rect, tilemap, collider, cf, set, filter);
                }

                if (collider.TryGetComponent<Substance>(out Substance substance))
                {
                    substance.AddToSet(set, filter);
                }

                foreach (var provider in collider.GetComponents<SubstanceProvider>())
                {
                    provider.Substance?.AddToSet(set, filter);
                }

                GetSubstancesFromTag(collider.gameObject.tag, set, filter);
            }

        }
        #endregion

        #region Get Substances From
        private static void GetSubstancesFromTilemap(Rect rect, Tilemap tilemap, Collider2D collider, ContactFilter2D cf, SubstanceSet set, ISubstanceFilter filter)
        {
            if ((cf.useLayerMask && tilemap.gameObject.layer.IsOtherAllInFlag(cf.layerMask))
                || (!cf.useTriggers && collider.isTrigger))
                return;

            RectInt tileRect = new RectInt() { min = (Vector2Int)tilemap.WorldToCell(rect.min), max = (Vector2Int)tilemap.WorldToCell(rect.max) };
            tileRect.height += 1;
            tileRect.width += 1;


#if DEBUG_THIS
            bool debug = filter is PlayerSubstanceFilter f && f.ColliderType == PlayerSubstanceColliderType.Main;
            if (debug)
            {
                Instance.debugRects.Clear();
                Instance.debugPoints.Clear();

                Rect debugRect = new Rect()
                {
                    min = tilemap.CellToWorld((Vector3Int)tileRect.min),
                    max = tilemap.CellToWorld((Vector3Int)tileRect.max)
                };
                Instance.debugRects.Add(new Tuple<Rect, Color>(debugRect, Color.blue));

                Instance.debugRects.Add(new Tuple<Rect, Color>(rect, Color.green));

                List<ContactPoint2D> contacts = new List<ContactPoint2D>();
                collider.GetContacts(contacts);

                foreach (var c in contacts)
                {
                    Instance.debugPoints.Add(new Tuple<Vector3, Color>(c.point, Color.yellow));
                }

            } 
#endif

            //Debug.Log("TileRect: " + tileRect);
            foreach (var tilePos in tileRect.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile((Vector3Int)tilePos);

                //if (tile != null)
                //    Debug.Log("Try Add Tile: " + tile.name);

                if (tile != null && Instance.tileBasedSubstances.TryGetValue(tile, out List<Substance> subs))
                {
                    if (rect.Overlap(tilemap.GetRectAtPos(tilePos), out Rect overlap))
                    {

                        foreach (var s in subs)
                            s.AddToSet(set, filter);

#if DEBUG_THIS
                        if (debug)
                            Instance.debugRects.Add(new Tuple<Rect, Color>(overlap, Color.magenta)); 
#endif
                    }

                }
            }
        }

        private static void GetSubstancesFromTag(string tag, SubstanceSet set, ISubstanceFilter filter)
        {
            while (!string.IsNullOrEmpty(tag) && !Equals(tag, "Untagged"))
            {
                int startMat = tag.IndexOf("m:");
                if (startMat == -1)
                    return;

                tag = tag.Substring(startMat + 2);
                int endMat = tag.IndexOf(' ');

                string substanceName = tag;
                if (endMat != -1)
                {
                    substanceName = tag.Substring(0, endMat);
                    tag = tag.Substring(endMat + 1);
                }

                if (substanceName != "" && Instance.nameBasedSubstances.TryGetValue(substanceName, out Substance s))
                {
                    s.AddToSet(set, filter);
                }

                if (endMat == -1)
                    break;
            }
        } 
        #endregion

        #region Load Substance Pointers
        private void LoadSubstances()
        {
#if UNITY_EDITOR
            LoadSubstancePointers();
#endif

            foreach (var s in serializedPointers)
            {
                foreach (TileBase tileBase in GetTilesFormPointer(s))
                    AddTileSubsEntry(s.GetComponent<Substance>(), tileBase);
            }

            foreach (var s in serializedPointers)
            {
                if (s.TryGetComponent<SubstanceTagPointer>(out SubstanceTagPointer pointer))
                {
                    if (nameBasedSubstances.ContainsKey(s.name))
                    {
                        Debug.Log("You can not name two Substances the same. This will lead to unexpected behaviour with tags.");
                        continue;
                    }
                    nameBasedSubstances.Add(s.name, s.GetComponent<Substance>());
                }
            }
        }

        private List<TileBase> GetTilesFormPointer(SubstancePointer pointer)
        {
            var pointers = pointer.GetComponents<SubstanceTilePointer>();
            List<TileBase> tiles = new List<TileBase>();

            foreach (var p in pointers)
            {
                tiles.AddRange(p.tilesOfSubstance);
            }
            return tiles;


        }

#if UNITY_EDITOR
        [ContextMenu("Load Prefabs")]
        private void LoadSubstancePointers()
        {
            List<SubstancePointer> pointers = new List<SubstancePointer>();

            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Debug.Log("Path: " + path);

                SubstancePointer s = AssetDatabase.LoadAssetAtPath<SubstancePointer>(path);
                if (s != null)
                    pointers.Add(s);
            }
            serializedPointers = pointers.ToArray();
        }

#endif
        #endregion

        #region Helper Methods

        public void OnDrawGizmos()
        {
#if DEBUG_THIS
            foreach (var t in debugRects)
            {
                Gizmos.color = t.Item2;
                Gizmos.DrawWireCube(t.Item1.center, t.Item1.size);
            }

            foreach (var p in debugPoints)
            {
                Gizmos.color = p.Item2;
                Gizmos.DrawWireSphere(p.Item1, 0.1f);
            }
#endif
        }


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
        #endregion
    }
}
