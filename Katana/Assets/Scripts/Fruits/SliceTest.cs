using Assets.Scripts.Core;
using UnityEngine;

[RequireComponent(typeof(Slice))]
public class SliceTest : MonoBehaviour, IDestructible
{
    [SerializeField] private Slice _slice;

    private void Reset()
    {
        _slice = GetComponent<Slice>();
    }

    public void GetFractured(Vector3 fractureOrigin, float fractureForce = 0)
    {
        throw new System.NotImplementedException();
    }

    public void GetSliced(Vector3 sliceOrigin, Vector3 sliceNormal, float sliceVelocity)
    {
        _slice.ComputeSlice(sliceNormal, sliceOrigin);
    }
}