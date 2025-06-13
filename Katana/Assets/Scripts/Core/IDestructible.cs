using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IDestructible
    {
        public void GetFractured(Vector3 fractureOrigin, float fractureForce = 0);
        public void GetSliced(Vector3 sliceOrigin, Vector3 sliceNormal, float sliceVelocity);
    }
}