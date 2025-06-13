using System.Collections;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class FruitFragmentScript : MonoBehaviour
{
    public Rigidbody Rigidbody;
    private Collider _collider;

    [SerializeField] private float _lifetime = 10;

    [FoldoutGroup("Disappear Animation")]
    [SerializeField] private float _disappearTime = 1;
    [FoldoutGroup("Disappear Animation")]
    [SerializeField] private AnimationCurve _disappearCurve;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void CopySettings(FruitFragmentScript original)
    {
        gameObject.layer = original.gameObject.layer;
        _lifetime = original._lifetime;
        _disappearTime = original._disappearTime;
        _disappearCurve = original._disappearCurve;
    }

    public void GetDestroyed() => StartCoroutine(DestroyRoutine());

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(_lifetime);
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
    }
}
