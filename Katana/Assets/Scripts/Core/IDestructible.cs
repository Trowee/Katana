using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IDestructible
    {
        /// <summary>
        /// Fractures the object
        /// </summary>
        /// <param name="impactVelocity">Velocity that has to surpass a threshold for the fracture to happen(leave at -1 to ignore)</param>
        /// <param name="sender">GameObject that caused the fracture</param>
        public List<FragmentScript> GetFractured(
            float impactVelocity = -1,
            GameObject sender = null);

        /// <summary>
        /// Slices the object
        /// </summary>
        /// <param name="sliceOrigin">Where the slice originates from</param>
        /// <param name="sliceNormal">Normal of the slice</param>
        /// <param name="impactVelocity">Velocity that has to surpass a threshold for the slice to happen(leave at -1 to ignore)</param>
        /// <param name="sender">GameObject that caused the slice</param>
        public List<FragmentScript> GetSliced(
            Vector3 sliceOrigin,
            Vector3 sliceNormal,
            float impactVelocity = -1,
            GameObject sender = null);
    }
}
