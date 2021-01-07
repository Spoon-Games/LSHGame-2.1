using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace LogicStateM.Editor
{
    internal static class LSMCodeGenerator
    {
        internal static void GenerateLSM(AnimatorController controller)
        {

            GenerateCode(controller, out string code, out string path);
            GenerateCSFile(code,path);
        }

        private static void GenerateCode(AnimatorController controller,out string code,out string path)
        {
            string name = controller.name.TrimSpace() + "LSM";
            path = GetPath(controller, name);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("#if UNITY_EDITOR\nusing UnityEditor;\nusing UnityEditor.Animations;\n#endif");
            builder.AppendLine("\n\n");
            builder.AppendLine("[RequireComponent(typeof(Animator))]");
            builder.AppendLine("public class " + name + " : MonoBehaviour \n{\n");

            builder.AppendLine("private const string animatorPath = \"" + AssetDatabase.GetAssetPath(controller) + "\";\n");
            builder.AppendLine("private Animator animator;");
            builder.AppendLine("public Animator Animator => animator;\n");

            GenerateParameters(controller,builder);
            GenerateLayers(controller, builder);
            GenerateStates(controller, builder);

            builder.AppendLine("\n\n private void Awake() {");
            builder.AppendLine("animator = GetComponent<Animator>();\n");
            builder.AppendLine("if(animator.runtimeAnimatorController == null)\n{\n#if UNITY_EDITOR\n");
            builder.AppendLine("animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);");
            builder.AppendLine("#endif\n}\n}");

            builder.AppendLine("}");

            code = builder.ToString();
        }

        #region Generate Parameters
        private static void GenerateParameters(AnimatorController controller, StringBuilder builder)
        {
            foreach (var parameter in controller.parameters)
            {
                string type = GetParameterType(parameter);
                string pName = parameter.name.UpperFirst().TrimSpace();
                string pHashName = parameter.name.TrimSpace() + "Hash";

                builder.AppendLine("private const int " + pHashName + " = " + Animator.StringToHash(parameter.name) + ";");
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    builder.AppendLine("public void " + pName + "() => animator.SetTrigger(" + pHashName + ");\n");
                }
                else
                {
                    string methodBody = GetParameterMethodBody(parameter);

                    builder.AppendLine("public " + type + " " + pName + " {\nget => animator.Get" + methodBody +
                        "(" + pHashName + ");\nset => animator.Set" + methodBody + "(" + pHashName + ",value); \n}\n");
                }
            }
        }

        private static string GetParameterType(AnimatorControllerParameter parameter)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    return "float";
                case AnimatorControllerParameterType.Int:
                    return "int";
                case AnimatorControllerParameterType.Bool:
                    return "bool";
                case AnimatorControllerParameterType.Trigger:
                    return "bool";
                default:
                    return "";
            }
        }

        private static string GetParameterMethodBody(AnimatorControllerParameter parameter)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    return "Float";
                case AnimatorControllerParameterType.Int:
                    return "Integer";
                case AnimatorControllerParameterType.Bool:
                    return "Bool";
                default:
                    return "";
            }
        }
        #endregion

        #region Generate Layers
        private static void GenerateLayers(AnimatorController controller, StringBuilder builder)
        {
            builder.AppendLine("public enum Layers { ");
            foreach (var layer in controller.layers)
            {
                builder.Append(layer.name.TrimSpace().ReplaceDot() + " ,");
            }
            builder.Remove(builder.Length - 2, 2);
            builder.AppendLine("\n}\n");
        }
        #endregion

        #region Generate States
        private static void GenerateStates(AnimatorController controller, StringBuilder builder)
        {
            List<string> states = new List<string>();
            List<int> parents = new List<int>();

            foreach (var layer in controller.layers)
            {
                GetStates(states, layer.stateMachine, layer.name,parents,-1);
            }

            builder.AddRange(states, "public enum States {\n", s => builder.Append(s.TrimSpace().ReplaceDot().TrimBaseLayer() + " ,"),
                2, "}\n\n");

            AddRange(builder, states, "private List<int> stateHashes = new List<int>{ ",
                s => builder.Append(Animator.StringToHash(s) + " ,"), 2, "};\n\n");

            builder.AddRange(parents, "private int[] parentStates = new int[] { ", p => builder.Append(p + " ,"), 2, " };\n\n");

            GenerateStateAccesoire(controller, builder);
        }

        private static void GetStates(List<string> states, AnimatorStateMachine stateMachine, string path,
            List<int> parents,int parent)
        {
            foreach (var state in stateMachine.states)
            {
                states.Add(path + "." + state.state.name);
                parents.Add(parent);
            }

            foreach (var subMachine in stateMachine.stateMachines)
            {
                states.Add(path + "." + subMachine.stateMachine.name);
                parents.Add(parent);
                GetStates(states, subMachine.stateMachine, path + "." + subMachine.stateMachine.name,parents,states.Count-1);
            }
        }
        #endregion

        #region Generate State Methods
        private static void GenerateStateAccesoire(AnimatorController controller,StringBuilder builder)
        {
            builder.AppendLine("public States CurrentState => (States) GetCurrentState(0);\n");

            builder.AppendLine("public States GetCurrentState(Layers layer) => (States) GetCurrentState((int)layer);\n");

            builder.AppendLine("public int GetCurrentState(int layer){\n" +
                "return stateHashes.IndexOf(animator.GetCurrentAnimatorStateInfo(layer).fullPathHash);" +
                "}\n");

            builder.AppendLine("public bool IsCurrantState(States state) => IsCurrantState(0,(int)state);\n");
            builder.AppendLine("public bool IsCurrantState(Layers layer,States state) => IsCurrantState((int)layer,(int)state);\n");
            builder.AppendLine("public bool IsCurrantState(int layer,int state) => IsParentStateOrSelf(GetCurrentState(layer),state);\n");

            builder.AppendLine("public bool IsParentStateOrSelf(int baseState,int parentState) {\n" +
                "for(int s = baseState; s != -1; s = GetParentState(s)) \n" +
                "if(s == parentState) \n return true;\n" +
                "return false;\n}\n");
            builder.AppendLine("public int GetParentState(int state) => parentStates[state];\n");
        }
        #endregion

        #region HelperMethods
        private static void GenerateCSFile(string code, string path)
        {
            using (StreamWriter outfile = new StreamWriter(path))
            {
                outfile.Write(code);
            }

            AssetDatabase.ImportAsset(path);
            Debug.Log("GeneratedCSFile: " + path);
        }

        private static string GetPath(AnimatorController controller, string name)
        {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(controller)).ReplaceBackslash() + "/" + name + ".cs";
        }

        private static void AddRange<T>(this StringBuilder builder,IEnumerable<T> range,string start, Action<T> action, int remove,string end)
        {
            builder.Append(start);
            foreach(T v in range)
            {
                action.Invoke(v);
            }
            if(range.Count() != 0) {
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
            return s.Replace("BaseLayer_","");
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