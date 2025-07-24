namespace ZoDream.Plugin.Spine.Models
{
    public interface IUpdatableRuntime
    {
        public bool IsEnabled { get; }


        public void Update(PhysicsMode physics);

    }

    public enum PhysicsMode
    {
        /// <summary>Physics are not updated or applied.</summary>
        None,

        /// <summary>Physics are reset to the current pose.</summary>
        Reset,

        /// <summary>Physics are updated and the pose from physics is applied.</summary>
        Update,

        /// <summary>Physics are not updated but the pose from physics is applied.</summary>
        Pose
    }
}
