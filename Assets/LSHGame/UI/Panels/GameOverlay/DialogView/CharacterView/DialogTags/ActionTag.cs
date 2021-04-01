using TagInterpreterR;

namespace LSHGame.UI
{
    [Tag(name:"Action",tagType:TagAttribute.TagType.Single)]
    public class ActionTag : BaseTag
    {
        [TagField(name: "Name", isDefault: true)]
        public string ActionName = "Action";
    }
}
