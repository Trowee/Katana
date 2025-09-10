using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using AudioManager;
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

        private void Start()
        {
            if (!Application.isPlaying) return;
            GameManager.AudioManager.GetOrCreateItem(_ringAudioItem);
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
