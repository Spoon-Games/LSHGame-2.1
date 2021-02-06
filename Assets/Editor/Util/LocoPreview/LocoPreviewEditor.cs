using LSHGame.PlayerN;
using LSHGame.Util;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace LSHGame.Editor
{
    #region LocoEditorWindow
    public class LocoPreviewEditor : EditorWindow
    {
        private static bool isEnabled = false;
        private static LocoPreview instance;

        [MenuItem("Window/Util/Loco Preview Editor")]
        public static void GetWindow()
        {
            GetWindow<LocoPreviewEditor>("Loco Preview Editor");
        }

        [InitializeOnLoadMethod]
        private static void InitConnectionRegistry()
        {
            EnableEditor();
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

        }

        internal static void OnToolbarGUI()
        {
            GUILayout.Space(20);

            GUILayout.Space(8);

            var tex = EditorGUIUtility.IconContent(@"UnityEditor.SceneView").image;
            bool e = GUILayout.Toggle(isEnabled, new GUIContent("Loco Preview", tex, "Toggle Loco Preview"), ToolbarStyles.commandButtonStyle);
            if (e && !isEnabled)
                EnableEditor();
            if (!e && isEnabled)
                DisableEditor();

            //GUILayout.FlexibleSpace();
        }

        internal static void EnableEditor()
        {
            if (instance == null)
                instance = new LocoPreview();
            if (!isEnabled)
            {
                isEnabled = true;
                instance.OnEnable();
            }
        }


        private static void DisableEditor()
        {
            if (isEnabled)
            {
                isEnabled = false;
                instance?.OnDisable();
            }
        }
    } 
    #endregion

    public class LocoPreview
    {
        private const float deltaT = 0.02f;

        private LocoEditorRepository data;

        #region Player
        private PlayerStateMachine stateMachine;
        private PlayerStats stats;
        private SubstanceSet substanceSet;

        private PlayerController playerController;
        private PlayerColliders playerColliders;

        private Rect mainColliderRect;
        private bool isTouchingClimbWallLeft;
        #endregion

        private bool isAcitve = false;
        private bool isPlaying = false;

        private Vector3 worldPosition;
        private Vector2 prevVelocity;
        private float prevGravity;
        private AnimationCurve prevXVelocityCurve;
        private bool prevIsDeaccel;

        private float prevOpDurration;

        private bool prevOneSec = false;

        private readonly InputContainer input = new InputContainer();

        #region Init

        internal void OnEnable()
        {
            data = Resources.Load<LocoEditorRepository>("LocoEditorRepository");
            if (data == null)
            {
                Debug.Log("No LocoEditorRepository found");
                return;
            }

            stateMachine = new PlayerStateMachine(null);
            substanceSet = new SubstanceSet();
            if (data.playerController == null)
            {
                Debug.Log("No Player prefab assignet to LocoEditorRepository");
                return;
            }
            playerController = data.playerController;
            playerColliders = playerController.GetComponentInChildren<PlayerColliders>();

            SceneView.duringSceneGui += OnSceneGUI;

        }

        internal void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        #endregion

        #region Update
        void OnSceneGUI(SceneView sceneview)
        {
            if (ProcessEvents() && isAcitve)
            {
                input.Update();
                if(!isPlaying)
                    GetWorldPosition();

                stats = playerColliders.GetStatePreview(worldPosition, substanceSet, stateMachine, playerController, out mainColliderRect, out isTouchingClimbWallLeft);

                CheckInput();

                stateMachine.UpdateState();

                GetPrevValues();
                SelectColor();
                DrawCollider();
                DrawMovement();
            }
        }
        #endregion

        #region GetPrevValues
        private void GetNonePlayValues()
        {
            //if (isPlaying)
            //    return;
            switch (stateMachine.State)
            {
                case PlayerStates.Aireborne:
                case PlayerStates.ClimbLadder:
                case PlayerStates.ClimbLadderTop:
                case PlayerStates.Locomotion:
                    prevVelocity.x = input.MouseWheel * input.XLooking * stats.RunAccelCurve.Evaluate(1000);
                    break;
            }
        }

        private void GetPrevValues()
        {
            ResetPrev();
            GetNonePlayValues();
            switch (stateMachine.State)
            {
                case PlayerStates.Locomotion:
                    GetPrevRun(false);
                    GetPrevJump();
                    break;
                case PlayerStates.Aireborne:
                    GetPrevRun(true);
                    prevGravity = stats.Gravity;
                    GetPrevJump();
                    break;
                case PlayerStates.ClimbWall:
                    prevOpDurration = stats.ClimbingWallExhaustDurration;
                    prevVelocity.y = stats.ClimbingWallSlideSpeed * input.Movement.y;
                    GetPrevJump();
                    break;
                case PlayerStates.ClimbWallExhaust:
                    prevVelocity.y = -stats.ClimbingWallExhaustSlideSpeed;
                    GetPrevJump();
                    break;
                case PlayerStates.ClimbLadder:
                    GetPrevRun(false);
                    prevVelocity.y = stats.ClimbingLadderSpeed * input.Movement.y;
                    GetPrevJump();
                    break;
                case PlayerStates.ClimbLadderTop:
                    GetPrevRun(false);
                    prevVelocity.y = Mathf.Min(0, stats.ClimbingLadderSpeed * input.Movement.y);
                    GetPrevJump();
                    break;
                case PlayerStates.Dash:
                    prevVelocity.x = stats.DashSpeed * input.XLooking;
                    prevOpDurration = stats.DashDurration;
                    break;
                case PlayerStates.Death:
                    break;
                case PlayerStates.Crouching:
                    break;
            }
        }

        private void GetPrevJump()
        {
            if (input.Space)
            {
                switch (stateMachine.State)
                {
                    case PlayerStates.Aireborne:
                        if (stats.IsJumpableInAir)
                            prevVelocity.y = stats.JumpSpeed;
                        break;
                    case PlayerStates.ClimbLadder:
                    case PlayerStates.ClimbLadderTop:
                    case PlayerStates.Locomotion:
                        GetPrevRun(true);
                        prevVelocity.y = stats.JumpSpeed;
                        prevGravity = stats.Gravity;
                        break;
                    case PlayerStates.ClimbWall:
                        GetPrevRun(true);
                        prevVelocity = stats.ClimbingWallJumpVelocity * new Vector2(input.Movement.x, 1);
                        prevGravity = stats.Gravity;
                        break;
                    case PlayerStates.ClimbWallExhaust:
                        GetPrevRun(true);
                        prevVelocity = stats.ClimbingWallJumpVelocity * new Vector2(isTouchingClimbWallLeft ? 11 : -1, 1);
                        prevGravity = stats.Gravity;
                        break;
                }
            }
        }

        private void GetPrevRun(bool isAireborne)
        {
            if (!input.A && !input.D)
            {
                prevIsDeaccel = true;
                prevXVelocityCurve = isAireborne ? stats.RunDeaccelAirborneCurve : stats.RunDeaccelCurve;
            }
            else
            {
                prevIsDeaccel = false;
                prevXVelocityCurve = isAireborne ? stats.RunAccelAirborneCurve : stats.RunAccelCurve;
                prevOneSec = true;
            }
        }

        private void ResetPrev()
        {
            if(!isPlaying)
                prevVelocity = Vector2.zero;
            prevGravity = 0;
            prevXVelocityCurve = null;
            prevOpDurration = 5;
            prevOneSec = false;
        }
        #endregion

        #region Draw
        private void SelectColor()
        {
            switch (stateMachine.State)
            {
                case PlayerStates.Locomotion:
                    Handles.color = data.groundColor;
                    break;
                case PlayerStates.Aireborne:
                    Handles.color = data.aireborneColor;
                    break;
                case PlayerStates.ClimbWall:
                    Handles.color = data.climbWallColor;
                    break;
                case PlayerStates.ClimbWallExhaust:
                    Handles.color = data.climbWallExhaustColor;
                    break;
                case PlayerStates.ClimbLadder:
                    Handles.color = data.climbLadderColor;
                    break;
                case PlayerStates.ClimbLadderTop:
                    Handles.color = data.groundColor;
                    break;
                case PlayerStates.Dash:
                    Handles.color = data.dashColor;
                    break;
                case PlayerStates.Death:
                    Handles.color = data.deadColor;
                    break;
                case PlayerStates.Crouching:
                    Handles.color = data.groundColor;
                    break;
                default:
                    break;
            }
        }

        private void DrawCollider()
        {
            Handles.DrawWireCube(mainColliderRect.center, mainColliderRect.size);
        }

        private void DrawMovement()
        {
            Vector2 lastPoint = worldPosition;
            Vector2 point = worldPosition;
            Vector2 velocity = prevVelocity;
            float startT = GetStartT();

            bool isOneSecColor = false;

            for (float t = deltaT; t < prevOpDurration; t += deltaT)
            {
                velocity.x = GetXVelocity(startT, t, out bool finished);
                velocity.y = GetYVelocity( t, out bool finished2);

                point = lastPoint + velocity * deltaT;
                Handles.DrawLine(lastPoint, point);
                lastPoint = point;

                if (finished && finished2)
                {
                    if (t <= 0.5 && prevOneSec)
                    {
                        if (!isOneSecColor)
                        {
                            isOneSecColor = true;
                            Color color = Handles.color * data.oneSecColorScalar;
                            color.a = 1;
                            Handles.color = color;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private float GetXVelocity(float startT, float t, out bool finished)
        {
            finished = false;
            if (prevXVelocityCurve == null)
                return prevVelocity.x;

            if (prevIsDeaccel)
            {
                float v = prevXVelocityCurve.Evaluate(startT + t) * Mathf.Sign(prevVelocity.x);
                finished = v.Approximately(0, 0.1f);
                return v;
            }
            else
            {
                float v = prevXVelocityCurve.Evaluate(startT + t) * Mathf.Sign(input.Movement.x);
                finished = Mathf.Abs(v) >= prevXVelocityCurve.Evaluate(1000) - 0.2f;
                return v;
            }
        }

        private float GetYVelocity(float t, out bool finished)
        {
            float v = prevVelocity.y - prevGravity * t * 100;
            finished = v <= -prevVelocity.y;
            return v;
        }

        private float GetStartT()
        {
            float startT = 0;

            if (prevXVelocityCurve != null)
            {
                if (prevIsDeaccel)
                    startT = prevXVelocityCurve.GetTimeOfValue(Mathf.Abs(prevVelocity.x), deltaT, prevIsDeaccel);
                else
                    startT = prevXVelocityCurve.GetTimeOfValue(prevVelocity.x * Mathf.Sign(input.Movement.x), deltaT, prevIsDeaccel);
            }
            return startT;
        }
        #endregion

        #region Check

        private void CheckInput()
        {
            stateMachine.IsTouchingClimbWall &= input.Shift;
            stateMachine.IsClimbWallExhausted = input.Control;
            stateMachine.IsDash = input.RightClick;
        }

        #endregion

        #region Process Events
        private bool ProcessEvents()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (isAcitve)
                    {
                        if (e.button == 0)
                        {
                            isPlaying = !isPlaying;
                            e.Use();
                        }
                        else if (e.button == 1)
                            input.RightClick = true;
                    }
                    if (e.button == 2)
                    {
                        isAcitve = !isAcitve;
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (isAcitve)
                    {
                        if (e.button == 0 || e.button == 2)
                            e.Use();
                    }
                    if (e.button == 1)
                        input.RightClick = false;
                    break;
                case EventType.MouseDrag:
                    if (isAcitve && !input.RightClick)
                        e.Use();
                    break;
                case EventType.ScrollWheel:
                    if (isAcitve && !input.RightClick)
                    {
                        input.MouseWheel -= e.delta.y * 0.1f;
                        e.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (isAcitve)
                    {
                        switch (e.keyCode)
                        {
                            case KeyCode.A: input.A = true; input.XLooking = -1; break;
                            case KeyCode.W: input.W = true; break;
                            case KeyCode.D: input.D = true; input.XLooking = 1; break;
                            case KeyCode.S: input.S = true; break;
                            case KeyCode.Space: input.Space = true; break;
                            case KeyCode.RightShift:
                            case KeyCode.LeftShift: input.Shift = true; break;
                            case KeyCode.RightControl:
                            case KeyCode.LeftControl: input.Control = true; break;
                            case KeyCode.RightAlt:
                            case KeyCode.LeftAlt: input.Alt = true; break;
                        }
                        e.Use();
                    }
                    break;
                case EventType.KeyUp:
                    switch (e.keyCode)
                    {
                        case KeyCode.A: input.A = false; break;
                        case KeyCode.W: input.W = false; break;
                        case KeyCode.D: input.D = false; break;
                        case KeyCode.S: input.S = false; break;
                        case KeyCode.Space: input.Space = false; break;
                        case KeyCode.RightShift:
                        case KeyCode.LeftShift: input.Shift = false; break;
                        case KeyCode.RightControl:
                        case KeyCode.LeftControl: input.Control = false; break;
                        case KeyCode.RightAlt:
                        case KeyCode.LeftAlt: input.Alt = false; break;
                    }
                    if (isAcitve)
                        e.Use();
                    break;
                case EventType.Repaint:
                    return true;
            }
            return false;
        }
        #endregion

        #region Helper Methods

        private void GetWorldPosition()
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Camera.current.pixelHeight - mousePos.y;

            worldPosition = Camera.current.ScreenToWorldPoint(mousePos);
            worldPosition.z = 0;
        }


        private class InputContainer
        {
            public bool W;
            public bool A;
            public bool S;
            public bool D;
            public bool Shift;
            public bool Space;
            public bool Control;
            public bool Alt;
            public bool RightClick;

            private float _mouseWheel;
            public float MouseWheel { get => _mouseWheel; set => _mouseWheel = Mathf.Clamp01(value); }

            public Vector2 Movement;
            public int XLooking = 1;

            public void Update()
            {
                Movement = new Vector2((D ? 1 : 0) + (A ? -1 : 0), (W ? 1 : 0) + (S ? -1 : 0));
            }
        }

        #endregion

    }

    internal static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageLeft,
                fixedWidth = 90,
            };
        }
    }
}
