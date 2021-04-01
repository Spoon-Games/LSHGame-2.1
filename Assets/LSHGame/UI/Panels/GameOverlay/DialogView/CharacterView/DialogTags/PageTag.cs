using TagInterpreterR;

namespace LSHGame.UI
{
    [Tag(name: "Page", tagType: TagAttribute.TagType.Single)]
    public class PageTag : BaseTag
    {
        [TagField(name: "ButtonName")]
        public string ButtonName;

        [TagField(name: "IsButton")]
        public bool IsButton = true;

    }
}
