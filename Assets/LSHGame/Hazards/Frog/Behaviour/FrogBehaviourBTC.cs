using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using BehaviourT;



[RequireComponent(typeof(UnityEngine.Rigidbody2D))]
public class FrogBehaviourBTC : BehaviourTreeComponent 
{

private const string behaviourTreePath = "Assets/LSHGame/Hazards/Frog/Behaviour/FrogBehaviour.asset";



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
