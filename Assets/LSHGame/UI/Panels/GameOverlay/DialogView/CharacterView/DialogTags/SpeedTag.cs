using TagInterpreterR;

namespace LSHGame.UI
{
    [Tag(name: "Speed", tagType: TagAttribute.TagType.Area)]
    public class SpeedTag : BaseTag
    {
        [TagField(name: "Value", isDefault: true)]
        public float Value;

    }
}
