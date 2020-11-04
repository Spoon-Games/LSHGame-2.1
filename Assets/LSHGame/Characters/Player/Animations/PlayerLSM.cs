using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif



[RequireComponent(typeof(Animator))]
public class PlayerLSM : MonoBehaviour 
{

private const string animatorPath = "Assets/LSHGame/Characters/Player/Animations/PlayerLSM.cs";

private Animator animator;
public Animator Animator => animator;

private const int IsTouchingLadderHash = 1379395005;
public bool IsTouchingLadder {
get => animator.GetBool(IsTouchingLadderHash);
set => animator.SetBool(IsTouchingLadderHash,value); 
}

private const int IsGroundedHash = 507951781;
public bool IsGrounded {
get => animator.GetBool(IsGroundedHash);
set => animator.SetBool(IsGroundedHash,value); 
}

private const int HorizontalSpeedHash = -1118621987;
public float HorizontalSpeed {
get => animator.GetFloat(HorizontalSpeedHash);
set => animator.SetFloat(HorizontalSpeedHash,value); 
}

private const int VerticalSpeedHash = -1148172834;
public float VerticalSpeed {
get => animator.GetFloat(VerticalSpeedHash);
set => animator.SetFloat(VerticalSpeedHash,value); 
}

private const int IsTouchingClimbWallHash = 1250088830;
public bool IsTouchingClimbWall {
get => animator.GetBool(IsTouchingClimbWallHash);
set => animator.SetBool(IsTouchingClimbWallHash,value); 
}

private const int IsDashHash = 1040174394;
public bool IsDash {
get => animator.GetBool(IsDashHash);
set => animator.SetBool(IsDashHash,value); 
}

private const int IsDeathHash = 569220492;
public bool IsDeath {
get => animator.GetBool(IsDeathHash);
set => animator.SetBool(IsDeathHash,value); 
}

public enum Layers { 
BaseLayer
}

public enum States {
ClimbingLadder ,Deathd ,Locomotion ,ClimbingWall ,Aireborne ,Aireborne_Landing ,Aireborne_Jump ,Dash ,Dash_Dash}

private List<int> stateHashes = new List<int>{ 203955424 ,1081322403 ,-1269438207 ,1203022505 ,795248834 ,712193975 ,2070461148 ,304293996 ,-646947155};

private int[] parentStates = new int[] { -1 ,-1 ,-1 ,-1 ,-1 ,4 ,4 ,-1 ,7 };

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
