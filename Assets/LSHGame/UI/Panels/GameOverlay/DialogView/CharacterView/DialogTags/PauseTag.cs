using TagInterpreterR;
using UnityEngine;

namespace LSHGame.UI
{
    [Tag(name: "Pause",tagType:TagAttribute.TagType.Single)]
    public class PauseTag : BaseTag
    {
        [TagField(isDefault:true)]
        public float Length = 1;

        private float endTimer = 0;

        public override void OnActivate()
        {
            endTimer = Time.time + Length;
        }

        public override bool OnUpdate()
        {
            return Time.time < endTimer;
        }
    }
}
