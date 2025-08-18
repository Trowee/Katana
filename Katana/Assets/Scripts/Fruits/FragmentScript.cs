using System.Collections;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using SadnessMonday.BetterPhysics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BetterRigidbody))]
[RequireComponent(typeof(Collider))]
public class FragmentScript : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public BetterRigidbody BetterRigidbody;
    private Collider _collider;

    public float Lifetime = 10;

    [FoldoutGroup("Disappear Animation")]
    [SerializeField] private float _disappearTime = 1;
    [FoldoutGroup("Disappear Animation")]
    [SerializeField] private AnimationCurve _disappearCurve;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        BetterRigidbody = GetComponent<BetterRigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void CopySettings(FragmentScript original)
    {
        gameObject.layer = original.gameObject.layer;
        Lifetime = original.Lifetime;
        _disappearTime = original._disappearTime;
        _disappearCurve = original._disappearCurve;

        Rigidbody.interpolation = original.Rigidbody.interpolation;
        Rigidbody.collisionDetectionMode = original.Rigidbody.collisionDetectionMode;
        BetterRigidbody.PhysicsLayer = original.BetterRigidbody.PhysicsLayer;
    }

    public void GetDestroyed() => StartCoroutine(DestroyRoutine());

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(Lifetime);
        _collider.enabled = false;
        Rigidbody.useGravity = false;

        var originalScale = transform.localScale;
        float lerpPos = 0;
        while (lerpPos < 1)
        {
            var t = _disappearCurve.Evaluate(Misc.Tween(ref lerpPos, _disappearTime));
            transform.localScale = Vector3.LerpUnclamped(originalScale, Vector3.zero, t);
            yield return null;
        }

        // Parent should get destroyed so no need to cleanup here
    }
}
