using TagInterpreterR;

namespace LSHGame.UI
{
    [Tag(name:"Button",tagType:TagAttribute.TagType.Single)]
    public class ButtonTag : BaseTag
    {
        [TagField(name:"Name",isDefault:true)]
        public string ButtonName;
    }
}
