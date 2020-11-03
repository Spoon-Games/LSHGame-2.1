using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace LSHGame.Util
{
    public class LSHGameException : Exception
    {
        public LSHGameException(string msg) : base(msg) { }
    }

    public class Bundle
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public void Put<T>(string name,T value)
        {
            values.Add(name, value);
        }

        public bool TryGet<T>(string name,out T value)
        {
            if(values.TryGetValue(name,out object v))
            {
                if(v is T result)
                {
                    value = result;
                    return true;
                }
            }
            value = default;
            return false;
        }
    }

    public static class GameUtil
    {
        public static List<T> FindAllOfTypeInScene<T>()
        {
            List<T> result = new List<T>();
            Transform[] roots = SceneManager.GetActiveScene().GetRootGameObjects().Select(g => g.transform).ToArray();
            foreach (Transform child in roots)
            {
                AddChildrenTransform<T>(result, child);
            }
            return result;
        }

        private static void AddChildrenTransform<T>(List<T> result,Transform parent)
        {
            Component[] components = parent.GetComponents<Component>();
            foreach (var c in components)
                if (c is T r)
                    result.Add(r);
            foreach (Transform child in parent)
                AddChildrenTransform<T>(result, child);
        }

        public static float EvaluateValueByStep(this AnimationCurve curve,float value,float timeStep,bool descending = false,float accuracy = 15)
        {
            //bool stz = value < 0;
            //float stzf = stz ? 1 : -1;
            float result;
            if (value >= curve.Evaluate(0) ^ descending)
            {
                if (value >= curve.keys[curve.length - 1].value ^ descending)
                    return curve.keys[curve.length - 1].value;

                float t = 0;
                int protection = 0;
                while ((!descending && curve.Evaluate(t) < value) || (descending && curve.Evaluate(t) > value))
                {
                    t += timeStep;
                    protection++;
                    if (protection > 1000)
                    {
                        //Debug.Log("Protection");
                        Debug.Log("Protection t: " + t + "\nInput value: " + value + "\nEvaluate T: " + curve.Evaluate(t) + " V: " + (curve.Evaluate(t) > value) + " descending: " + descending);
                        return curve.Evaluate(t);
                    }
                }

                float partStep = timeStep;
                for (int i = 0; i < accuracy; i++)
                {
                    partStep = partStep / 2;
                    if (value < curve.Evaluate(t - partStep) ^ descending)
                    {
                        t -= partStep;
                    }
                }

                result = curve.Evaluate(t + timeStep);
            }
            else
            {
                if (value <= curve.keys[0].value ^ descending)
                    return curve.keys[0].value;

                float t = 0;
                float protection = 0;
                while ((!descending && curve.Evaluate(t) > value) || (descending && curve.Evaluate(t) < value))
                {
                    t -= timeStep;
                    protection++;
                    if (protection > 1000)
                    {
                        //Debug.Log("Protection");
                        Debug.Log("Protection t: " + t + "\nInput value: " + value + "\nEvaluate T: " + curve.Evaluate(t) + " V: " + (curve.Evaluate(t) > value) + " descending: " + descending);
                        return curve.Evaluate(t);
                    }
                }

                t += timeStep;

                float partStep = timeStep;
                for (int i = 0; i < accuracy; i++)
                {
                    partStep = partStep / 2;
                    if (value < curve.Evaluate(t - partStep) ^ descending)
                    {
                        t -= partStep;
                    }
                }
                //Debug.Log("t: " + t + "\nInput value: " + value +  "\nresult: " + result+"\nEvaluate T: "+curve.Evaluate(t));

                result = curve.Evaluate(t+timeStep);
            }

            //Debug.Log("Input: "+value+" result: "+result+" desending: "+descending+"D: " + System.Math.Round((result - value) / timeStep, 2));

            return result;
        }

        public static bool IsLayer(this LayerMask layermask,int layer)
        {
            return layermask == (layermask | (1 << layer));
            //return layermask == (layermask | (1 << layer));
        }

        public static bool IsOtherAllInFlag(this int flag,int otherFlag)
        {
            return (flag & otherFlag) == otherFlag;
        }

        public static bool Approximately(this Vector2 a,Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }

        public static bool Approximately(this Vector2 a,Vector2 b,float accuracy)
        {
            return a.x.Approximately(b.x, accuracy) && b.y.Approximately(b.y, accuracy);
        }

        public static bool Approximately(this float a,float b,float accuracy)
        {
            return Mathf.Abs(a - b) <= accuracy;
        }

        public static Rect LocalToWorldRect(this Rect rect,Transform transform)
        {
            Rect r = new Rect() { size = rect.size * AbsVector2(transform.lossyScale) };
            r.center = rect.center * transform.lossyScale + (Vector2)transform.position;
            return r;
        }

        public static Vector2 AbsVector2(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        //public static Rect LocalToWorldRect(this Rect rect,Matrix4x4 localToWorld)
        //{
        //    return new Rect() { min = localToWorld.MultiplyPoint(rect.min), max = localToWorld.MultiplyPoint(rect.max) };
        //}

        public static bool Overlap(this Rect a, Rect b,out Rect overlap)
        {
            overlap = new Rect()
            {
                min = new Vector2(Mathf.Max(a.min.x, b.min.x), Mathf.Max(a.min.y, b.min.y)),
                max = new Vector2(Mathf.Min(a.max.x, b.max.x), Mathf.Min(a.max.y, b.max.y))
            };
            return overlap.width >= 0 && overlap.height >= 0;
        }

        public static bool IsTouchingRect(this Collider2D collider, Rect rect, ContactFilter2D cf)
        {
            List<Collider2D> collider2Ds = new List<Collider2D>();
            Physics2D.OverlapBox(rect.center, rect.size, 0, cf, collider2Ds);
            return collider2Ds.Contains(collider);
        }

        public static Rect GetRectAtPos(this Tilemap tilemap,Vector2Int pos)
        {
            return new Rect()
            {
                min = tilemap.CellToWorld((Vector3Int)pos),
                max = tilemap.CellToWorld((Vector3Int)(pos + Vector2Int.one))
            };
        }

        public static Rect InsetRect(this Rect rect, float inset)
        {
            rect.min += Vector2.one * inset;
            rect.max -= Vector2.one * inset;
            return rect;
        }

        public static Rect GetGlobalRectOfBox(this BoxCollider2D boxCollider)
        {
            return new Rect() { size = boxCollider.size, center = boxCollider.offset }.LocalToWorldRect(boxCollider.transform);
        }
    }
}
