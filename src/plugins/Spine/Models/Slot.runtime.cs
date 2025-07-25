using System.Collections.Generic;

namespace ZoDream.Plugin.Spine.Models
{
    public class SlotRuntime(SpineSkeletonController controller, Slot slot)
    {
        public int SequenceIndex { get; set; } = -1;
        public AttachmentBase Attachment {  get; set; }
        public Bone Bone { get; internal set; }
        public IList<float> Deform { get; private set; } = [];

    }
}
