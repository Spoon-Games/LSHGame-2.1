using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using BehaviourT;



public class NewBehaviourTreeBTC : BehaviourTreeComponent 
{

private const string behaviourTreePath = "Assets/PackagesS/BehaviourT/Tests/New Behaviour Tree.asset";



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
