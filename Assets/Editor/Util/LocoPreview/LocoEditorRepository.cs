using LSHGame.PlayerN;
using UnityEngine;

namespace LSHGame.Editor
{
    [CreateAssetMenu(menuName ="LSHGame/Editor/Loco Editor Repository")]
    public class LocoEditorRepository : ScriptableObject
    {
        public PlayerController playerController;

        public Color groundColor = Color.blue;
        public Color aireborneColor = Color.cyan;
        public Color climbWallColor = Color.yellow;
        public Color climbWallExhaustColor = Color.yellow * 0.3f;
        public Color climbLadderColor = Color.magenta;
        public Color dashColor = Color.cyan * 0.7f;
        public Color deadColor = Color.red;

        public float oneSecColorScalar = 0.9f;

    }
}
