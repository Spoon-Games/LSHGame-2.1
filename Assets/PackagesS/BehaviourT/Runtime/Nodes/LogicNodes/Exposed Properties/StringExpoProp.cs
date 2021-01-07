using UnityEngine;

namespace BehaviourT
{
    [AddComponentMenu("LogicNodes/Exposed Properties/String Exposed Property")]
    [RequireComponent(typeof(BoxCollider2D))]
    public class StringExpoProp : ExposedProperty<string>
    {
        [SerializeField]
        [NodeEditorField(NodeEditorField.NodePlace.PortContainer,portContainer:"value")]
        private string defaultValue;

        protected override string GetDefault()
        {
            return defaultValue;
        }
    }
}
