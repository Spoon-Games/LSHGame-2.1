using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    public static class CheckpointManager
    {
        private static Vector3 currentCheckPos = Vector3.negativeInfinity;

        private static Dictionary<CheckpointInfo, Checkpoint> startCheckpoints = new Dictionary<CheckpointInfo, Checkpoint>();

        private static int currentOrder = 0;

        #region Init
        static CheckpointManager()
        {
            LevelManager.OnStartLoadingMainScene += Reset;
        }

        internal static void Reset(Func<float> o, TransitionInfo transition)
        {
            currentCheckPos = Vector3.negativeInfinity;
        }

        internal static void SetDefaultStartCheckpoint(Checkpoint checkpoint)
        {
            if (!Equals(currentCheckPos, Vector3.negativeInfinity))
                Debug.LogError("Multiple default start checkpoints in the scene. This will probably result in " +
                    "unexpected behaviour.");
            currentCheckPos = checkpoint.transform.position;
        }

        internal static void RegisterStartCheckpoint(Checkpoint checkpoint, CheckpointInfo identifier)
        {
            startCheckpoints[identifier] = checkpoint;
        }

        public static void SetStartCheckpoint(CheckpointInfo identifier, bool clearStartCheckpoints = true)
        {
            if (startCheckpoints.TryGetValue(identifier, out Checkpoint checkpoint))
            {
                SetCheckpoint(checkpoint);
            }
            else
                Debug.LogError("The checkpoint with the identifier " + identifier.name + " was not found");
            if (clearStartCheckpoints)
                startCheckpoints.Clear();
        } 
        #endregion

        public static Vector3 GetCheckpointPos()
        {
            if (Equals(currentCheckPos, Vector3.negativeInfinity))
                throw new SceneMException("No start checkpoint was assigned. Please change that!");
            return currentCheckPos;
        }

        internal static bool SetCheckpoint(Checkpoint checkpoint)
        {
            if(checkpoint.Order >= currentOrder)
            {
                currentCheckPos = checkpoint.transform.position;
                currentOrder = checkpoint.Order;
                return true;
            }
            return false;
        }
    } 
}
