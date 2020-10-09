#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    [DisallowMultipleComponent]
    public class Substance : BaseSubstance
    {
        public List<TileBase> tilesOfSubstance = new List<TileBase>();

        
#if UNITY_EDITOR
        [MenuItem("Assets/Create/LSHGame/Substance")]
        private static void CreateSubstancePrefab()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            GameObject go = new GameObject();
            go.AddComponent<Substance>();

            int i = 1;
            while (AssetDatabase.GetMainAssetTypeAtPath(path+"/Substance "+i+".prefab") != null){
                i++;
            }
            PrefabUtility.SaveAsPrefabAsset(go, path + "/Substance " + i + ".prefab");
            DestroyImmediate(go);
        }

        [MenuItem("Assets/SearchPrefabs")]
        private static void SearchPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Debug.Log("Path: " + path);

                Substance s = AssetDatabase.LoadAssetAtPath<Substance>(path);
                if (s != null)
                    Debug.Log("Substance: " + path);
                //foreach (var o in objects)
                //{
                //    //Debug.Log(o);
                //    if (o is Substance substance)
                //    {
                //        Debug.Log("Substance: " + path);
                //    }
                //}
            }
        }
#endif
    }

    public abstract class BaseSubstance : MonoBehaviour, ISubstance
    {
        private List<SubstanceProperty> m_substanceProperties;

        private List<SubstanceProperty> SubstanceProperties
        {
            get
            {
                if (m_substanceProperties == null)
                {
                    m_substanceProperties = new List<SubstanceProperty>();

                    m_substanceProperties.AddRange(GetComponents<SubstanceProperty>());
                }
                return m_substanceProperties;
            }
        }

        private List<SubSubstance> m_childSubstances;
        private List<SubSubstance> ChildSubstances { get
            {
                if(m_childSubstances == null)
                {
                    m_childSubstances = new List<SubSubstance>();
                    foreach(Transform child in transform)
                    {
                        if(child.TryGetComponent<SubSubstance>(out SubSubstance sc))
                        {
                            m_childSubstances.Add(sc);
                        }
                    }
                }
                return m_childSubstances;
            } }

        public virtual void AddToSet(HashSet<ISubstance> set, ISubstanceFilter filter)
        {
            if(filter.IsValidSubstance(this,out bool searchChildren))
            {
                if (!set.Contains(this))
                    set.Add(this);
            }

            if (searchChildren)
            {
                foreach(var c in ChildSubstances)
                {
                    c.AddToSet(set, filter);
                }
            }

        }

        public virtual void RecieveData(IDataReciever dataReciever)
        {
            foreach (var prop in SubstanceProperties)
            {
                prop.RecieveData(dataReciever);
            }
        }
    }

    public interface ISubstance
    {
        void AddToSet(HashSet<ISubstance> set, ISubstanceFilter filter);

        void RecieveData(IDataReciever reciever);
    }

    public interface ISubstanceFilter {
        bool IsValidSubstance(ISubstance substance, out bool searchChildren);
    }
}
