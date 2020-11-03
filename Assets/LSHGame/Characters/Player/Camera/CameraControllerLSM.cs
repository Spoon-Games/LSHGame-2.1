using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif



[RequireComponent(typeof(Animator))]
public class CameraControllerLSM : MonoBehaviour 
{

private const string animatorPath = "Assets/LSHGame/Characters/Player/Camera/CameraControllerLSM.cs";

private Animator animator;
public Animator Animator => animator;

private const int LookDownHash = -1797170654;
public bool LookDown {
get => animator.GetBool(LookDownHash);
set => animator.SetBool(LookDownHash,value); 
}

public enum Layers { 
BaseLayer
}

public enum States {
PlayerAnimatorDefault ,LookDown}

private List<int> stateHashes = new List<int>{ 1725647286 ,1768528329};

private int[] parentStates = new int[] { -1 ,-1 };

public States CurrentState => (States) GetCurrentState(0);

public States GetCurrentState(Layers layer) => (States) GetCurrentState((int)layer);

public int GetCurrentState(int layer){
return stateHashes.IndexOf(animator.GetCurrentAnimatorStateInfo(layer).fullPathHash);}

public bool IsCurrantState(States state) => IsCurrantState(0,(int)state);

public bool IsCurrantState(Layers layer,States state) => IsCurrantState((int)layer,(int)state);

public bool IsCurrantState(int layer,int state) => IsParentStateOrSelf(GetCurrentState(layer),state);

public bool IsParentStateOrSelf(int baseState,int parentState) {
for(int s = baseState; s != -1; s = GetParentState(s)) 
if(s == parentState) 
 return true;
return false;
}

public int GetParentState(int state) => parentStates[state];



 private void Awake() {
animator = GetComponent<Animator>();

if(animator.runtimeAnimatorController == null)
{
#if UNITY_EDITOR

animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);
#endif
}
}
}
