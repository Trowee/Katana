using Assets.Scripts.Core;
using UnityEngine;

[RequireComponent(typeof(Slice))]
public class SliceTest : MonoBehaviour, ISliceable
{
    [SerializeField] private Slice _slice;

    private void Reset()
    {
        _slice = GetComponent<Slice>();
    }

    public void GetSliced(Vector3 sliceOrigin, Vector3 sliceNormal, float sliceVelocity)
    {
        _slice.ComputeSlice(sliceNormal, sliceOrigin);
    }
}