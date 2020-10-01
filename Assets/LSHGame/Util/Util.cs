using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }
}
