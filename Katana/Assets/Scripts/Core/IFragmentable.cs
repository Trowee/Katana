using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IFragmentable
    {
        /// <summary>
        /// Fractures the object
        /// </summary>
        /// <param name="fragments">Fragments that are the result of the fracture</param>
        /// <param name="impactVelocity">Velocity that has to surpass a threshold for the fracture to happen</param>
        /// <param name="sender">GameObject that caused the fracture</param>
        /// <returns>Whether the object got fractured or not</returns>
        public bool GetFractured(
            out List<FragmentScript> fragments,
            float? impactVelocity = null,
            GameObject sender = null);

        /// <summary>
        /// Slices the object
        /// </summary>
        /// <param name="fragments">Fragments that are the result of the slice</param>
        /// <param name="sliceOrigin">Where the slice originates from</param>
        /// <param name="sliceNormal">Normal of the slice</param>
        /// <param name="impactVelocity">Velocity that has to surpass a threshold for the slice to happen</param>
        /// <param name="sender">GameObject that caused the slice</param>
        /// <returns>Whether the object got sliced or not</returns>
        public bool GetSliced(
            out List<FragmentScript> fragments,
            Vector3 sliceOrigin,
            Vector3 sliceNormal,
            float? impactVelocity = null,
            GameObject sender = null);
    }
}
