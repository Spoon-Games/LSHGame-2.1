using UnityEngine;

namespace LSHGame.Util
{
    public class InputActionDropdownAttribute : PropertyAttribute
    {
        public string inputActionAssetProperty { get; }

        public InputActionDropdownAttribute(string inputActionAssetProperty)
        {
            this.inputActionAssetProperty = inputActionAssetProperty;
        }
    }
}
