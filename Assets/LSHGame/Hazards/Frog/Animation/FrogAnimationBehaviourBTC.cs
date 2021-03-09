using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using BehaviourT;



[RequireComponent(typeof(UnityEngine.Animator))]
public class FrogAnimationBehaviourBTC : BehaviourTreeComponent 
{

private const string behaviourTreePath = "Assets/LSHGame/Hazards/Frog/Animation/FrogAnimationBehaviour.asset";



 protected override void Awake() {
if(BehaviourTree == null)
{
#if UNITY_EDITOR

BehaviourTree = AssetDatabase.LoadAssetAtPath<BehaviourTree>(behaviourTreePath);
#endif
}
base.Awake();
}
}
