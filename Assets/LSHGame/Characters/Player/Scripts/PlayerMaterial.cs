using LSHGame.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [CreateAssetMenu(fileName = "PlayerMaterial",menuName = "LSHGame/PlayerMaterial")]
    public class PlayerMaterial : ScriptableObject
    {
        [Header("Stats")]

        public DefaultableProperty<AnimationCurve> RunAccelCurve;

        public DefaultableProperty<AnimationCurve> RunDeaccelCurve;

        public DefaultableProperty<AnimationCurve> RunAccelAirborneCurve;

        public DefaultableProperty<AnimationCurve> RunDeaccelAirborneCurve;

        [Header("Environment")]
        public DefaultableProperty<BounceSettings> BounceSettings;
        public DefaultableProperty<Vector2> PushingVelocity;
        public DefaultableProperty<float> Gravity;
        public DefaultableProperty<float> FallDamping;

        [Header("Jump")]
        public DefaultableProperty<float> JumpSpeed;
        public DefaultableProperty<float> JumpSpeedCutter;

        [Header("Climbing Ladder")]
        [SerializeField]
        public DefaultableProperty<float> ClimbingLadderSpeed;

        [Header("Climbing Wall")]
        public DefaultableProperty<float> ClimbingWallSlideSpeed;
        public DefaultableProperty<float> ClimbingWallExhaustSlideSpeed;
        public DefaultableProperty<float> ClimbWallExhaustDurration;
        public DefaultableProperty<Vector2> ClimbingWallJumpVelocity;

        [Header("Dash")]
        public DefaultableProperty<float> DashDurration;
        public DefaultableProperty<float> DashSpeed;
        public DefaultableProperty<float> DashRecoverDurration;
    } 

    [System.Serializable]
    public class BounceSettings
    {
        public float BounceSpeed;
        public float Rotation = 0;
        public bool AddGameObjectRotation = true;
        public bool ConstantHeight = true;
    }

    internal class PlayerStats
    {
        public AnimationCurve RunAccelCurve => GetValue(m=>m.RunAccelCurve);
        public AnimationCurve RunDeaccelCurve => GetValue(m=>m.RunDeaccelCurve);
        public AnimationCurve RunAccelAirborneCurve => GetValue(m => m.RunAccelAirborneCurve);
        public AnimationCurve RunDeaccelAirborneCurve => GetValue(m => m.RunDeaccelAirborneCurve);

        public BounceSettings BounceSettings => GetValue(m => m.BounceSettings);
        public Vector2 PushingVelocity => GetValue(m => m.PushingVelocity);
        public float Gravity => GetValue(m => m.Gravity);
        public float FallDamping => GetValue(m => m.FallDamping);

        public float JumpSpeed => GetValue(m => m.JumpSpeed);
        public float JumpSpeedCutter => GetValue(m => m.JumpSpeedCutter);

        public float ClimbingLadderSpeed => GetValue(m => m.ClimbingLadderSpeed);

        public float ClimbingWallSlideSpeed => GetValue(m => m.ClimbingWallSlideSpeed);
        public float ClimbingWallExhaustSlideSpeed => GetValue(m => m.ClimbingWallExhaustSlideSpeed);
        public float ClimbWallExhaustDurration => GetValue(m => m.ClimbWallExhaustDurration);
        public Vector2 ClimbingWallJumpVelocity => GetValue(m => m.ClimbingWallJumpVelocity);

        public float DashDurration => GetValue(m => m.DashDurration);
        public float DashSpeed => GetValue(m => m.DashSpeed);
        public float DashRecoverDurration => GetValue(m => m.DashRecoverDurration);

        private PlayerMaterial defaultMaterial;

        private List<PlayerMaterial> materials = new List<PlayerMaterial>();

        public PlayerStats(PlayerMaterial defaultMaterial)
        {
            this.defaultMaterial = defaultMaterial;
            //Update(null);
        }

        public void AddMaterial(PlayerMaterial material)
        {
            materials.Insert(0,material);
            //Update(material);
        }

        public void RemoveMaterial(PlayerMaterial material)
        {
            materials.Remove(material);
            //Update(material);
        }

        //private void Update(PlayerMaterial change)
        //{
        //    UpdateValue(ref RunAccelCurve, m => m.RunAccelCurve,change);
        //    UpdateValue(ref RunDeaccelCurve, m => m.RunDeaccelCurve, change);
        //    UpdateValue(ref RunAccelAirborneCurve, m => m.RunAccelAirborneCurve, change);
        //    UpdateValue(ref RunDeaccelAirborneCurve, m => m.RunDeaccelAirborneCurve, change);
        //    UpdateValue(ref BounceSettings, m => m.BounceSettings, change);
        //    UpdateValue(ref PushingVelocity, m => m.PushingVelocity, change);
        //    UpdateValue(ref Gravity, m => m.Gravity, change);
        //    UpdateValue(ref FallDamping, m => m.FallDamping, change);
        //    UpdateValue(ref JumpSpeed, m => m.JumpSpeed, change);
        //    UpdateValue(ref JumpSpeedCutter, m => m.JumpSpeedCutter, change);
        //    UpdateValue(ref ClimbingLadderSpeed, m => m.ClimbingLadderSpeed, change);
        //    UpdateValue(ref ClimbingWallSlideSpeed, m => m.ClimbingWallSlideSpeed, change);
        //    UpdateValue(ref ClimbingWallExhaustSlideSpeed, m => m.ClimbingWallExhaustSlideSpeed, change);
        //    UpdateValue(ref ClimbWallExhaustDurration, m => m.ClimbWallExhaustDurration, change);
        //    UpdateValue(ref ClimbingWallJumpVelocity, m => m.ClimbingWallJumpVelocity, change);
        //    UpdateValue(ref DashDurration, m => m.DashDurration, change);
        //    UpdateValue(ref DashSpeed, m => m.DashSpeed, change);
        //    UpdateValue(ref DashRecoverDurration, m => m.DashRecoverDurration, change);
        //}

        //private void UpdateValue<T>(ref T value,Func<PlayerMaterial,DefaultableProperty<T>> func,PlayerMaterial change)
        //{
        //    if (change != null && func.Invoke(change).isDefault)
        //        return;
        //    value = func.Invoke(defaultMaterial).value;
        //    foreach (var m in materials)
        //    {
        //        DefaultableProperty<T> p = func.Invoke(m);
        //        if (!p.isDefault)
        //        {
        //            value = p.value;
        //            return;
        //        }
        //    }
        //}

        private T GetValue<T>(Func<PlayerMaterial, DefaultableProperty<T>> func)
        {
            T value = func.Invoke(defaultMaterial).value;
            foreach (var m in materials)
            {
                DefaultableProperty<T> p = func.Invoke(m);
                if (!p.isDefault)
                {
                    value = p.value;
                    return value;
                }
            }
            return value;
        }

    }
}
