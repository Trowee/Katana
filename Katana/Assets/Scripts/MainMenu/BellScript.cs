using ArtificeToolkit.Attributes;
using Assets.Scripts.Audio;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    [ExecuteAlways]
    public class BellScript : MonoBehaviour
    {
        private static readonly int RingTrigger = Animator.StringToHash("Ring");
        [SerializeField, Required] private Animator _animator;
        [SerializeField] private AudioItem _ringAudioItem;

        private void Reset()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        [FoldoutGroup("Test")]
        [Button]
        public void Ring()
        {
            _animator.SetTrigger(RingTrigger);
            GameManager.AudioManager.Play(_ringAudioItem);
        }
    }
}