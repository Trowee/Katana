using ArtificeToolkit.Attributes;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    [ExecuteAlways]
    [RequireComponent(typeof(AudioSource))]
    public class BellScript : MonoBehaviour
    {
        private static readonly int RingTrigger = Animator.StringToHash("Ring");
        [SerializeField, Required] private Animator _animator;
        [SerializeField, Required] private AudioSource _ringAudioSource;

        private void Reset()
        {
            _animator = GetComponentInChildren<Animator>();
            _ringAudioSource = GetComponent<AudioSource>();
        }

        [FoldoutGroup("Test")]
        [Button]
        public void Ring()
        {
            _animator.SetTrigger(RingTrigger);
            _ringAudioSource.Play();
        }
    }
}