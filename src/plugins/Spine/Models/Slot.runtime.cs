using System.Collections.Generic;

namespace ZoDream.Plugin.Spine.Models
{
    public class SlotRuntime(SpineSkeletonController controller, Slot slot)
    {
        public AttachmentBase Attachment {  get; set; }
        public Bone Bone { get; internal set; }
        public IList<float> Deform { get; private set; } = [];

        public void Update(SkeletonRoot root)
        {
            if (root.Runtime.Skin.TryGet<AttachmentBase>(slot.Index, slot.Attachment, out var res))
            {
                Attachment = res;
            }
        }
    }
}
