using SceneM;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override void Awake()
        {
            base.Awake();
            LoadSubstances();
        }

        public static SubstanceProperty[] GetSubstances(Rect rect, ISubPropFilter filter)
        {
            HashSet<SubstanceProperty> subProps = new HashSet<SubstanceProperty>();

            List<Collider2D> colliders = GetTouchRect(rect, LayerMask.GetMask("Ground"));

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<Tilemap>(out Tilemap tilemap))
                {
                    GetSubstancesFromTilemap(rect, tilemap, subProps, filter);
                }

                foreach (var provider in collider.GetComponents<SubstanceProvider>())
                {
                    provider.Substance.GetSubProps(filter, subProps);
                }

                GetSubstancesFromTag(collider.tag, subProps, filter);
            }
            return subProps.ToArray();
        }

        private static void GetSubstancesFromTilemap(Rect rect, Tilemap tilemap, HashSet<SubstanceProperty> subProps, ISubPropFilter filter)
        {
            RectInt tileRect = new RectInt() { min = (Vector2Int)tilemap.WorldToCell(rect.min), max = (Vector2Int)tilemap.WorldToCell(rect.max) };
            foreach (var tilePos in tileRect.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile((Vector3Int)tilePos);

                if (Instance.tileBasedSubstances.TryGetValue(tile, out List<Substance> subs))
                {
                    foreach (var s in subs)
                        s.GetSubProps(filter, subProps);
                }
            }
        }

        private static void GetSubstancesFromTag(string tag, HashSet<SubstanceProperty> subProps, ISubPropFilter filter)
        {
            while (string.IsNullOrEmpty(tag))
            {
                int startMat = tag.IndexOf("m:");
                if (startMat == -1)
                    return;

                tag = tag.Substring(startMat + 3);
                int endMat = tag.IndexOf(' ');

                string substanceName = tag;
                if (endMat != -1)
                {
                    substanceName = tag.Substring(0,endMat);
                    tag = tag.Substring(endMat + 1);
                }

                if (substanceName != "" && Instance.nameBasedSubstances.TryGetValue(substanceName, out Substance s))
                {
                    s.GetSubProps(filter, subProps);
                }

                if (endMat == -1)
                    break;
            }
        }

        private static List<Collider2D> GetTouchRect(Rect rect, LayerMask layers)
        {
            List<Collider2D> collider2Ds = new List<Collider2D>();
            Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() { useTriggers = false, layerMask = layers, useLayerMask = true }, collider2Ds);
            //Debug.Log("IsTouchingLayerRect: Center: " + ((Vector3)rect.center + transform.position) + "\n Size: " + rect.size + "\nLayers: " + layers.value + " isTouching: "+isTouching);

            return collider2Ds;
        }


        private void LoadSubstances()
        {
#if UNITY_EDITOR
            List<Substance> substances = new List<Substance>();
            var paths = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Substance).ToString());
            foreach (var path in paths)
            {
                substances.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<Substance>(path));
            }
            serializedSubstances = substances.ToArray();
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
