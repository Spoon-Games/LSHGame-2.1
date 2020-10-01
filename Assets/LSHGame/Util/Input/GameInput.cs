using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LSHGame.Util
{
    public static class GameInput
    {
        public static InputController Controller { get; private set; }

        static GameInput(){
            Controller = new InputController();
            Controller.Enable();
        }

        public static ButtonControl GetBC(this InputAction inputAction)
        {
            return inputAction.controls[0] as ButtonControl;
        }
    } 
}
