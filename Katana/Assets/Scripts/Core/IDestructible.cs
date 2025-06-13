using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IDestructible
    {
        public void GetSliced(Vector3 sliceOrigin, Vector3 sliceNormal, float sliceVelocity);
    }
}