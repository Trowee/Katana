using UnityEngine;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using Assets.Scripts.Core;
using System.Collections.Generic;

namespace Assets.Scripts.Colosseum
{
    [RequireComponent(typeof(Fracture))]
    public class ColosseumRock : MonoBehaviour
    {
        [SerializeField, Required] Fracture _fracture;
        [SerializeField, Required] FragmentScript _fragmentSettings;
        [SerializeField] float _explosionForce = 500;

        private void Reset()
        {
            _fracture = gameObject.GetOrAddComponent<Fracture>();
        }

        private void Awake()
        {
            ColosseumSceneManager.Player.OnPerformedAction += Explode;
        }

        private void Explode(PlayerAction playerAction)
        {
            ColosseumSceneManager.Player.OnPerformedAction -= Explode;
            var fragments = _fracture.ComputeFracture();

            List<FragmentScript> frags = new();
            fragments.ForEach(fragment =>
            {
                var frag = fragment.AddComponent<FragmentScript>();
                frag.CopySettings(_fragmentSettings);
                frag.GetDestroyed();
                fragment.GetComponent<Rigidbody>().AddExplosionForce(
                    _explosionForce, transform.position, 10, 0, ForceMode.Impulse);
                frags.Add(frag);
            });

            Destroy(fragments[0].transform.parent.gameObject, _fragmentSettings.Lifetime + 1);
            Destroy(gameObject);
        }
    }
}