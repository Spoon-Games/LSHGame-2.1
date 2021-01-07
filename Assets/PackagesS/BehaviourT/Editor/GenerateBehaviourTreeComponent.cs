using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace BehaviourT.Editor
{
    internal static class GenerateBehaviourTreeComponent
    {
        internal static void GenerateBTC(BehaviourTree behaviourTree, string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path).TrimSpace() + "BTC";
            string filePath = Path.GetDirectoryName(path).ReplaceBackslash() +'/'+ fileName;

            GenerateCode(behaviourTree, out string code, fileName, path);
            GenerateCSFile(code, filePath);
        }

        private static void GenerateCode(BehaviourTree behaviourTree, out string code, string fileName, string originPath)
        {

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("#if UNITY_EDITOR\nusing UnityEditor;\n#endif");
            builder.AppendLine("using BehaviourT;");
            builder.AppendLine("\n\n");

            GenerateRequirements(behaviourTree,builder);
            builder.AppendLine("public class " + fileName + " : BehaviourTreeComponent \n{\n");

            builder.AppendLine("private const string behaviourTreePath = \"" + originPath + "\";\n");

            GenerateParameters(behaviourTree, builder);

            builder.AppendLine("\n\n protected override void Awake() {");
            builder.AppendLine("if(BehaviourTree == null)\n{\n#if UNITY_EDITOR\n");
            builder.AppendLine("BehaviourTree = AssetDatabase.LoadAssetAtPath<BehaviourTree>(behaviourTreePath);");
            builder.AppendLine("#endif\n}");
            builder.AppendLine("base.Awake();\n}");

            builder.AppendLine("}");

            code = builder.ToString();
        }

        #region Generate Requirements
        private static void GenerateRequirements(BehaviourTree behaviourTree, StringBuilder builder)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach(var node in behaviourTree.DeserializeNodes())
            {
                 types.Add(node.GetType());
            }

            HashSet<Type> requiredTypes = new HashSet<Type>();
            foreach(var t in types)
            {
                var atts = t.GetCustomAttributes(true);
                foreach(var a in atts)
                {
                    if(a is RequireComponent requireComponent)
                    {
                        if (requireComponent.m_Type0 != null)
                            requiredTypes.Add(requireComponent.m_Type0);
                        if (requireComponent.m_Type1 != null)
                            requiredTypes.Add(requireComponent.m_Type1);
                        if (requireComponent.m_Type2 != null)
                            requiredTypes.Add(requireComponent.m_Type2);
                    }
                }
            }

            foreach(var requiredType in requiredTypes)
            {
                builder.AppendLine("[RequireComponent(typeof(" + requiredType.ToString() + "))]");
            }
        }
        #endregion

        #region Generate Parameters
        private static void GenerateParameters(BehaviourTree behaviourTree, StringBuilder builder)
        {
            foreach (var properties in behaviourTree.ExposedProperties)
            {
                var property = properties.FirstOrDefault();
                if(properties != null)
                {
                    string type = property.PropertyType.ToString();
                    string pName = property.PropertyName.UpperFirst().TrimSpace();

                    builder.AppendLine("public " + type.ToString() + " " + pName + "\n{");
                    builder.AppendLine("get => GetValue<"+type.ToString()+">(\""+property.PropertyName+"\");");
                    builder.AppendLine("set => TrySetValue(\"" + property.PropertyName + "\",value);\n}\n");
                }
            }
        }

        #endregion

        #region Generate Layers
        //private static void GenerateLayers(AnimatorController controller, StringBuilder builder)
        //{
        //    builder.AppendLine("public enum Layers { ");
        //    foreach (var layer in controller.layers)
        //    {
        //        builder.Append(layer.name.TrimSpace().ReplaceDot() + " ,");
        //    }
        //    builder.Remove(builder.Length - 2, 2);
        //    builder.AppendLine("\n}\n");
        //}
        #endregion

        #region Generate States
        //private static void GenerateStates(AnimatorController controller, StringBuilder builder)
        //{
        //    List<string> states = new List<string>();
        //    List<int> parents = new List<int>();

        //    foreach (var layer in controller.layers)
        //    {
        //        GetStates(states, layer.stateMachine, layer.name, parents, -1);
        //    }

        //    builder.AddRange(states, "public enum States {\n", s => builder.Append(s.TrimSpace().ReplaceDot().TrimBaseLayer() + " ,"),
        //        2, "}\n\n");

        //    AddRange(builder, states, "private List<int> stateHashes = new List<int>{ ",
        //        s => builder.Append(Animator.StringToHash(s) + " ,"), 2, "};\n\n");

        //    builder.AddRange(parents, "private int[] parentStates = new int[] { ", p => builder.Append(p + " ,"), 2, " };\n\n");

        //    GenerateStateAccesoire(controller, builder);
        //}

        //private static void GetStates(List<string> states, AnimatorStateMachine stateMachine, string path,
        //    List<int> parents, int parent)
        //{
        //    foreach (var state in stateMachine.states)
        //    {
        //        states.Add(path + "." + state.state.name);
        //        parents.Add(parent);
        //    }

        //    foreach (var subMachine in stateMachine.stateMachines)
        //    {
        //        states.Add(path + "." + subMachine.stateMachine.name);
        //        parents.Add(parent);
        //        GetStates(states, subMachine.stateMachine, path + "." + subMachine.stateMachine.name, parents, states.Count - 1);
        //    }
        //}
        #endregion

        #region Generate State Methods
        //private static void GenerateStateAccesoire(AnimatorController controller, StringBuilder builder)
        //{
        //    builder.AppendLine("public States CurrentState => (States) GetCurrentState(0);\n");

        //    builder.AppendLine("public States GetCurrentState(Layers layer) => (States) GetCurrentState((int)layer);\n");

        //    builder.AppendLine("public int GetCurrentState(int layer){\n" +
        //        "return stateHashes.IndexOf(animator.GetCurrentAnimatorStateInfo(layer).fullPathHash);" +
        //        "}\n");

        //    builder.AppendLine("public bool IsCurrantState(States state) => IsCurrantState(0,(int)state);\n");
        //    builder.AppendLine("public bool IsCurrantState(Layers layer,States state) => IsCurrantState((int)layer,(int)state);\n");
        //    builder.AppendLine("public bool IsCurrantState(int layer,int state) => IsParentStateOrSelf(GetCurrentState(layer),state);\n");

        //    builder.AppendLine("public bool IsParentStateOrSelf(int baseState,int parentState) {\n" +
        //        "for(int s = baseState; s != -1; s = GetParentState(s)) \n" +
        //        "if(s == parentState) \n return true;\n" +
        //        "return false;\n}\n");
        //    builder.AppendLine("public int GetParentState(int state) => parentStates[state];\n");
        //}
        #endregion

        #region HelperMethods
        private static void GenerateCSFile(string code, string path)
        {
            using (StreamWriter outfile = new StreamWriter(path+".cs"))
            {
                outfile.Write(code);
            }

            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
            Debug.Log("GeneratedCSFile: " + path);
        }

        private static string GetPath(AnimatorController controller, string name)
        {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(controller)).ReplaceBackslash() + "/" + name + ".cs";
        }

        private static void AddRange<T>(this StringBuilder builder, IEnumerable<T> range, string start, Action<T> action, int remove, string end)
        {
            builder.Append(start);
            foreach (T v in range)
            {
                action.Invoke(v);
            }
            if (range.Count() != 0)
            {
                builder.Remove(builder.Length - remove, remove);
            }
            builder.Append(end);
        }

        private static string UpperFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private static string TrimSpace(this string s)
        {
            return s.Replace(" ", "");
        }

        private static string TrimBaseLayer(this string s)
        {
            return s.Replace("BaseLayer_", "");
        }

        private static string ReplaceBackslash(this string s)
        {
            return s.Replace("\\", "/");
        }

        private static string ReplaceDot(this string s)
        {
            return s.Replace(".", "_");
        }
        #endregion
    }
}