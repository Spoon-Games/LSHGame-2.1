using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif



[RequireComponent(typeof(Animator))]
public class PlayerLSM : MonoBehaviour
{

    private const string animatorPath = "Assets/LSHGame/Characters/Player/Animations/Player.controller";

    private Animator animator;
    public Animator Animator => animator;

    private const int HorizontalSpeedHash = -1118621987;
    public float HorizontalSpeed
    {
        get => animator.GetFloat(HorizontalSpeedHash);
        set => animator.SetFloat(HorizontalSpeedHash, value);
    }

    private const int VerticalSpeedHash = -1148172834;
    public float VerticalSpeed
    {
        get => animator.GetFloat(VerticalSpeedHash);
        set => animator.SetFloat(VerticalSpeedHash, value);
    }

    private const int SLocomotionHash = -366921587;
    public bool SLocomotion
    {
        get => animator.GetBool(SLocomotionHash);
        set => animator.SetBool(SLocomotionHash, value);
    }

    private const int SAireborneHash = -2058483014;
    public bool SAireborne
    {
        get => animator.GetBool(SAireborneHash);
        set => animator.SetBool(SAireborneHash, value);
    }

    private const int SClimbingWallHash = 559333848;
    public bool SClimbingWall
    {
        get => animator.GetBool(SClimbingWallHash);
        set => animator.SetBool(SClimbingWallHash, value);
    }

    private const int SClimbinLadderHash = 1327845802;
    public bool SClimbinLadder
    {
        get => animator.GetBool(SClimbinLadderHash);
        set => animator.SetBool(SClimbinLadderHash, value);
    }

    private const int SDashHash = -1551241436;
    public bool SDash
    {
        get => animator.GetBool(SDashHash);
        set => animator.SetBool(SDashHash, value);
    }

    private const int SDeathHash = -612575539;
    public bool SDeath
    {
        get => animator.GetBool(SDeathHash);
        set => animator.SetBool(SDeathHash, value);
    }

    private const int StateChangedHash = -2071402574;
    public void StateChanged() => animator.SetTrigger(StateChangedHash);

    private const int VerticalPositionHash = 1430360787;
    public float VerticalPosition
    {
        get => animator.GetFloat(VerticalPositionHash);
        set => animator.SetFloat(VerticalPositionHash, value);
    }

    public enum Layers
    {
        BaseLayer
    }

    public enum States
    {
        ClimbingLadder, Deathd, Locomotion, ClimbingWall, Aireborne, Aireborne_Landing, Aireborne_Jump, Dash, Dash_Dash
    }

    private List<int> stateHashes = new List<int> { 203955424, 1081322403, -1269438207, 1203022505, 795248834, 712193975, 2070461148, 304293996, -646947155 };

    private int[] parentStates = new int[] { -1, -1, -1, -1, -1, 4, 4, -1, 7 };

    public States CurrentState => (States)GetCurrentState(0);

    public States GetCurrentState(Layers layer) => (States)GetCurrentState((int)layer);

    public int GetCurrentState(int layer)
    {
        return stateHashes.IndexOf(animator.GetCurrentAnimatorStateInfo(layer).fullPathHash);
    }

    public bool IsCurrantState(States state) => IsCurrantState(0, (int)state);

    public bool IsCurrantState(Layers layer, States state) => IsCurrantState((int)layer, (int)state);

    public bool IsCurrantState(int layer, int state) => IsParentStateOrSelf(GetCurrentState(layer), state);

    public bool IsParentStateOrSelf(int baseState, int parentState)
    {
        for (int s = baseState; s != -1; s = GetParentState(s))
            if (s == parentState)
                return true;
        return false;
    }

    public int GetParentState(int state) => parentStates[state];



    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator.runtimeAnimatorController == null)
        {
#if UNITY_EDITOR

            animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);
#endif
        }
    }
}
