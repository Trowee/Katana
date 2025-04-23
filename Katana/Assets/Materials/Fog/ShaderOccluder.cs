using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Materials.Fog
{
    public enum OccluderShape { Sphere, Box }

    [DisallowMultipleComponent, RequireComponent(typeof(Renderer))]
    public class ShaderOccluder : MonoBehaviour
    {
        [SerializeField] private bool _updateAtRuntime;
        [SerializeField] private float _updateInterval = 0.5f;
        [SerializeField] private OccluderShape _shape = OccluderShape.Sphere;

        private Renderer _renderer;

        private void Start()
        {
            StartCoroutine(UpdateRoutine());
        }

        private IEnumerator UpdateRoutine()
        {
            var wait = new WaitForSecondsRealtime(_updateInterval);
            while (true)
            {
                if (_updateAtRuntime)
                    UpdateAllOccluders();
                yield return wait;
            }
        }

        private void OnEnable()
        {
            UpdateAllOccluders();
        }

        private void UpdateAllOccluders()
        {
            var parent = transform.parent;
            if (!parent) return;

            var parentRenderer = parent.GetComponent<Renderer>();
            if (!parentRenderer) return;

            var occluders = parent.GetComponentsInChildren<ShaderOccluder>();
            var spheres = new List<Vector4>();
            var boxCenters = new List<Vector4>();
            var boxExtents = new List<Vector4>();

            foreach (var o in occluders)
            {
                if (!o) continue;
                var r = o.GetComponent<Renderer>();
                if (!r) continue;

                var bounds = r.bounds;

                switch (o._shape)
                {
                    case OccluderShape.Sphere:
                        spheres.Add(new(bounds.center.x, bounds.center.y, bounds.center.z,
                            bounds.extents.magnitude));
                        break;
                    case OccluderShape.Box:
                        boxCenters.Add(bounds.center);
                        boxExtents.Add(bounds.extents);
                        break;
                }
            }

            foreach (var mat in parentRenderer.sharedMaterials)
            {
                if (!mat) continue;

                mat.SetInt("_SphereCount", spheres.Count);
                mat.SetInt("_BoxCount", boxCenters.Count);
                mat.SetVectorArray("_OccluderSpheres", spheres);
                mat.SetVectorArray("_OccluderBoxCenters", boxCenters);
                mat.SetVectorArray("_OccluderBoxExtents", boxExtents);
            }
        }
    }
}
