// GENERATED AUTOMATICALLY FROM 'Assets/LSHGame/Util/Input/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace LSHGame.Util
{
    public class @InputController : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputController()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6aebd6ab-a31d-438e-b2e6-802b8e60e851"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""7e9300f9-cce6-4a7a-91ab-da8ea4d8211c"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""a707bf39-f731-4429-9371-8c38af26010e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""9a6ec2b4-1484-4577-a8f9-1b1a94dbd706"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""6c19b4bb-fe7b-49a7-9f6e-3b411190973c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""88ad458c-c898-4801-89df-2a10686aa5b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WallClimbHold"",
                    ""type"": ""Button"",
                    ""id"": ""bd31f9c2-62a0-4c78-b9ab-a9409ad345c0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SneekThroughPlatform"",
                    ""type"": ""Button"",
                    ""id"": ""a8752b1d-0f32-4ba3-8b63-6daf77ef38e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraLook"",
                    ""type"": ""Button"",
                    ""id"": ""3d8f97d9-2020-4ed2-a821-3477e52116b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""wasd"",
                    ""id"": ""0f069b2c-441f-4608-b24f-34f92977a155"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""febcaea4-b954-4f88-a1b0-f3d08959de47"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4c0d1c24-e2d6-4635-9c73-1f3593565399"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""15551f5e-cfab-4082-91b5-b65d8a795c40"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2614a6b6-c2d3-4c70-92fc-2cbc2524862b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""arrows"",
                    ""id"": ""517f357c-09d1-44ab-96ec-eff48f25c100"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5702cc32-9ff4-4dd9-903f-5f42603827db"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""80257b15-b81c-4305-984a-a98f343db136"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""824dbcb1-b34e-4d49-a2e8-9ed757fed9e5"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7f1d0906-9871-4526-ac22-4b2704e2cdb9"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1d0d75a3-3459-4d80-bf6c-6b67e21413a1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a3ea8e0-fd64-4980-97b7-6665354f3e95"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15f3d959-aa36-41cd-b705-f230a3cc6042"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a86802e-1f6c-487a-bcce-b647676bcb60"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f515c12-019d-4bc8-a2ea-a4cb221c52fb"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WallClimbHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e53961d7-509e-42fe-9099-c443b0d4ce2b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SneekThroughPlatform"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""954f8b51-98d0-44db-b69b-c6edb30e4d0f"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""1f8748df-3e05-4aeb-bdb7-02f906de72d1"",
            ""actions"": [
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""f9486cd4-64d2-4cdc-813f-5b6b33ddbe50"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Further"",
                    ""type"": ""Button"",
                    ""id"": ""7e1901ee-05e0-4afb-9765-28abfa513df8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""29b7abf6-6d14-41f7-be46-522a77f658d9"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a11ab0b-6159-4505-ba2f-77e29b18839d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Further"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d2596fb-0be3-4233-9e01-2a3ee09e5154"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Further"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3fe0bb69-75f6-4b21-870e-2213da928160"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Further"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
            m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
            m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
            m_Player_WallClimbHold = m_Player.FindAction("WallClimbHold", throwIfNotFound: true);
            m_Player_SneekThroughPlatform = m_Player.FindAction("SneekThroughPlatform", throwIfNotFound: true);
            m_Player_CameraLook = m_Player.FindAction("CameraLook", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_Back = m_UI.FindAction("Back", throwIfNotFound: true);
            m_UI_Further = m_UI.FindAction("Further", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Movement;
        private readonly InputAction m_Player_Jump;
        private readonly InputAction m_Player_Interact;
        private readonly InputAction m_Player_Sprint;
        private readonly InputAction m_Player_Dash;
        private readonly InputAction m_Player_WallClimbHold;
        private readonly InputAction m_Player_SneekThroughPlatform;
        private readonly InputAction m_Player_CameraLook;
        public struct PlayerActions
        {
            private @InputController m_Wrapper;
            public PlayerActions(@InputController wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Player_Movement;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputAction @Interact => m_Wrapper.m_Player_Interact;
            public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
            public InputAction @Dash => m_Wrapper.m_Player_Dash;
            public InputAction @WallClimbHold => m_Wrapper.m_Player_WallClimbHold;
            public InputAction @SneekThroughPlatform => m_Wrapper.m_Player_SneekThroughPlatform;
            public InputAction @CameraLook => m_Wrapper.m_Player_CameraLook;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Interact.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                    @Sprint.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                    @Sprint.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                    @Sprint.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                    @Dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                    @Dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                    @Dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                    @WallClimbHold.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWallClimbHold;
                    @WallClimbHold.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWallClimbHold;
                    @WallClimbHold.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWallClimbHold;
                    @SneekThroughPlatform.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSneekThroughPlatform;
                    @SneekThroughPlatform.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSneekThroughPlatform;
                    @SneekThroughPlatform.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSneekThroughPlatform;
                    @CameraLook.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraLook;
                    @CameraLook.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraLook;
                    @CameraLook.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraLook;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                    @Sprint.started += instance.OnSprint;
                    @Sprint.performed += instance.OnSprint;
                    @Sprint.canceled += instance.OnSprint;
                    @Dash.started += instance.OnDash;
                    @Dash.performed += instance.OnDash;
                    @Dash.canceled += instance.OnDash;
                    @WallClimbHold.started += instance.OnWallClimbHold;
                    @WallClimbHold.performed += instance.OnWallClimbHold;
                    @WallClimbHold.canceled += instance.OnWallClimbHold;
                    @SneekThroughPlatform.started += instance.OnSneekThroughPlatform;
                    @SneekThroughPlatform.performed += instance.OnSneekThroughPlatform;
                    @SneekThroughPlatform.canceled += instance.OnSneekThroughPlatform;
                    @CameraLook.started += instance.OnCameraLook;
                    @CameraLook.performed += instance.OnCameraLook;
                    @CameraLook.canceled += instance.OnCameraLook;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private IUIActions m_UIActionsCallbackInterface;
        private readonly InputAction m_UI_Back;
        private readonly InputAction m_UI_Further;
        public struct UIActions
        {
            private @InputController m_Wrapper;
            public UIActions(@InputController wrapper) { m_Wrapper = wrapper; }
            public InputAction @Back => m_Wrapper.m_UI_Back;
            public InputAction @Further => m_Wrapper.m_UI_Further;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void SetCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterface != null)
                {
                    @Back.started -= m_Wrapper.m_UIActionsCallbackInterface.OnBack;
                    @Back.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnBack;
                    @Back.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnBack;
                    @Further.started -= m_Wrapper.m_UIActionsCallbackInterface.OnFurther;
                    @Further.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnFurther;
                    @Further.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnFurther;
                }
                m_Wrapper.m_UIActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Back.started += instance.OnBack;
                    @Back.performed += instance.OnBack;
                    @Back.canceled += instance.OnBack;
                    @Further.started += instance.OnFurther;
                    @Further.performed += instance.OnFurther;
                    @Further.canceled += instance.OnFurther;
                }
            }
        }
        public UIActions @UI => new UIActions(this);
        private int m_KeyboardandMouseSchemeIndex = -1;
        public InputControlScheme KeyboardandMouseScheme
        {
            get
            {
                if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
                return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnSprint(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
            void OnWallClimbHold(InputAction.CallbackContext context);
            void OnSneekThroughPlatform(InputAction.CallbackContext context);
            void OnCameraLook(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnBack(InputAction.CallbackContext context);
            void OnFurther(InputAction.CallbackContext context);
        }
    }
}
