using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IDestructible
    {
        /// <summary>
        /// Fractures the object
        /// </summary>
        /// <param name="fractureOrigin">Where the fracture originates from</param>
        /// <param name="fractureForce">Explosion force of the fracture</param>
        /// <param name="impactVelocity">Velocity that has to surpass a threshold for the fracture to happen(leave at -1 to ignore)</param>
        /// <param name="sender">GameObject that caused the fracture</param>
        public void GetFractured(
            Vector3? fractureOrigin = null,
            float fractureForce = 0,
            float impactVelocity = -1,
            GameObject sender = null);

        /// <summary>
        /// Slices the object
        /// </summary>
        /// <param name="sliceOrigin">Where the slice originates from</param>
        /// <param name="sliceNormal">Normal of the slice</param>
        /// <param name="impactVelocity">Velocity that has to surpass a threshold for the slice to happen(leave at -1 to ignore)</param>
        /// <param name="sender">GameObject that caused the slice</param>
        public void GetSliced(
            Vector3 sliceOrigin,
            Vector3 sliceNormal,
            float impactVelocity = -1,
            GameObject sender = null);
    }
}
