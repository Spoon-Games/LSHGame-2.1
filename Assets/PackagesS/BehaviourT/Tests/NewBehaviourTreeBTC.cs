#if UNITY_EDITOR
using UnityEditor;
#endif
using BehaviourT;



public class NewBehaviourTreeBTC : BehaviourTreeComponent
{

    private const string behaviourTreePath = "Assets/PackagesS/BehaviourT/Tests/New Behaviour Tree.asset";



    protected override void Awake()
    {
        if (BehaviourTreeInstance == null)
        {
#if UNITY_EDITOR

            BehaviourTreeObjectReference = AssetDatabase.LoadAssetAtPath<BehaviourTree>(behaviourTreePath);
#endif
        }
        base.Awake();
    }
}
