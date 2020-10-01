using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    public static class CheckpointManager
    {
        private static Vector3 currantCheckPos = Vector3.negativeInfinity;

        private static Dictionary<CheckpointInfo, Vector3> startCheckpoints = new Dictionary<CheckpointInfo, Vector3>();

        static CheckpointManager()
        {
            LevelManager.OnStartLoadingMainScene += Reset;
        }

        internal static void Reset(Func<float> o,MainSceneInfo mainSceneInfo)
        {
            currantCheckPos = Vector3.negativeInfinity;
        }

        internal static void SetDefaultStartCheckpoint(Checkpoint checkpoint)
        {
            if (!Equals(currantCheckPos,Vector3.negativeInfinity))
                Debug.LogError("Multiple default start checkpoints in the scene. This will probably result in " +
                    "unexpected behaviour.");
            currantCheckPos = checkpoint.transform.position;
        }

        internal static void RegisterStartCheckpoint(Checkpoint checkpoint,CheckpointInfo identifier)
        {
            startCheckpoints[identifier] = checkpoint.transform.position;
        }

        public static void SetStartCheckpoint(CheckpointInfo identifier,bool clearStartCheckpoints = true)
        {
            if (startCheckpoints.TryGetValue(identifier, out Vector3 pos))
            {
                currantCheckPos = pos;
            }
            else
                Debug.LogError("The checkpoint with the identifier " + identifier.name + " was not found");
            if (clearStartCheckpoints)
                startCheckpoints.Clear();
        }

        public static Vector3 GetCheckpointPos()
        {
            if (Equals(currantCheckPos, Vector3.negativeInfinity))
                throw new SceneMException("No start checkpoint was assigned. Please change that!");
            return currantCheckPos;
        }

        internal static void SetCheckpoint(Checkpoint checkpoint)
        {
            currantCheckPos = checkpoint.transform.position;
        }
    } 
}
