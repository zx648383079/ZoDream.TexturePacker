﻿using System;
using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    internal class SkeletonRoot
    {
        public Animation[] Animations { get; set; }

        public Bone[] Bones { get; set; }

        public SkeletonHeader Skeleton { get; set; }

        public Skin[] Skins { get; set; }

        public Slot[] Slots { get; set; }
        public Event[] Events { get; set; }

        public IkConstraint[] IkConstraints { get; set; }
        public TransformConstraint[] TransformConstraints { get; set; }
        public PathConstraint[] PathConstraints { get; set; }
        public PhysicsConstraint[] PhysicsConstraints { get; internal set; }

        [JsonIgnore]
        public SkeletonRuntime Runtime { get; private set; } = new();

        public int GetSlotIndex(string name)
        {
            return Array.FindIndex(Slots, x => x.Name == name);
        }
    }
}
