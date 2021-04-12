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
if(BehaviourTreeInstance == null)
{
#if UNITY_EDITOR

            BehaviourTreeObjectReference = AssetDatabase.LoadAssetAtPath<BehaviourTree>(behaviourTreePath);
#endif
}
base.Awake();
}
}
