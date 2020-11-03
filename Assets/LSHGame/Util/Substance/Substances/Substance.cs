#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace LSHGame.Util
{
    [DisallowMultipleComponent]
    public class Substance : FilterableSubstance 
    {
        internal List<TileBase> GetTilesFormPointer()
        {
            var pointers = GetComponents<SubstanceTilePointer>();
            List<TileBase> tiles = new List<TileBase>();

            foreach(var p in pointers)
            {
                tiles.AddRange(p.tilesOfSubstance);
            }
            return tiles;

           
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/LSHGame/Substance")]
        private static void CreateSubstancePrefab()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            GameObject go = new GameObject();
            go.AddComponent<Substance>();
            go.AddComponent<SubstanceTilePointer>();

            int i = 1;
            while (AssetDatabase.GetMainAssetTypeAtPath(path + "/Substance " + i + ".prefab") != null)
            {
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

    [DisallowMultipleComponent]
    public abstract class BaseSubstance : MonoBehaviour, ISubstance
    {
        [NonSerialized]
        private SubstanceProperty[] m_substanceProperties = null;
        protected SubstanceProperty[] SubstanceProperties
        {
            get
            {
                if (m_substanceProperties == null)
                {
                    m_substanceProperties = GetComponents<SubstanceProperty>();
                }
                return m_substanceProperties;
            }
        }

        private ISubstance[] m_childSubstances;
        protected ISubstance[] ChildSubstances
        {
            get
            {
                if (m_childSubstances == null)
                {
                    List<ISubstance> tmp = new List<ISubstance>();
                    foreach (Transform child in transform)
                    {
                        if (child.TryGetComponent<ISubstance>(out ISubstance sc))
                        {
                            tmp.Add(sc);
                        }
                    }

                    foreach(var provider in GetComponents<SubstanceProvider>())
                    {
                        tmp.Add(provider.Substance);
                    }
                    m_childSubstances = tmp.ToArray();
                }
                return m_childSubstances;
            }
        }

        public virtual void RecieveData(IDataReciever dataReciever)
        {
            //Debug.Log("Recieve Data Props: " + SubstanceProperties.Length);
            foreach (var prop in SubstanceProperties)
            {
                prop.RecieveData(dataReciever);
            }
        }

        public abstract void AddToSet(SubstanceSet set, ISubstanceFilter filter);
    }

    [DisallowMultipleComponent]
    public abstract class FilterableSubstance : BaseSubstance
    {
        private List<ISubstanceSpecifier> m_substanceSpecifier;
        protected List<ISubstanceSpecifier> SubstanceSpecifier
        {
            get
            {
                if (m_substanceSpecifier == null)
                {
                    m_substanceSpecifier = new List<ISubstanceSpecifier>();
                    GetComponents<ISubstanceSpecifier>(m_substanceSpecifier);
                }
                return m_substanceSpecifier;
            }
        }

        public override void AddToSet(SubstanceSet set, ISubstanceFilter filter)
        {
            if (set.Contains(this))
                return;
            if (SubstanceSpecifier.Count == 0)
            {
                AddToSetHelper(set, filter);
                return;
            }
            foreach (var specifier in SubstanceSpecifier)
            {
                if (filter.IsValidSubstance(specifier))
                {
                    AddToSetHelper(set, filter);
                    return;
                }
            }
        }

        private void AddToSetHelper(SubstanceSet set, ISubstanceFilter filter)
        {
            set.Add(this);

            foreach (var c in ChildSubstances)
            {
                c.AddToSet(set, filter);
            }
        }
    }
    public interface ISubstanceSpecifier { }

    public interface ISubstance
    {
        void AddToSet(SubstanceSet set, ISubstanceFilter filter);

        void RecieveData(IDataReciever reciever);
    }

    public interface ISubstanceFilter
    {
        bool IsValidSubstance(ISubstanceSpecifier specifier);
    }
}
