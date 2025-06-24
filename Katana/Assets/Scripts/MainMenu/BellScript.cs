using ArtificeToolkit.Attributes;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    [ExecuteAlways]
    public class BellScript : MonoBehaviour
    {
        private static readonly int RingTrigger = Animator.StringToHash("Ring");
        [SerializeField, Required] private Animator _animator;

        private void Reset()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        [FoldoutGroup("Test")]
        [Button]
        public void Ring()
        {
            _animator.SetTrigger(RingTrigger);
        }
    }
}