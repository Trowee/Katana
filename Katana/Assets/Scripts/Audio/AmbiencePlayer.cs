using System.Collections;
using Assets.Scripts.Core;
using AudioManager;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AmbiencePlayer : MonoBehaviour
    {
        [SerializeField] private AudioItem _ambienceItem;

        private void Reset()
        {
            _ambienceItem.SourceType = SourceType.Object;
            _ambienceItem.AssignTargetAtRuntime = true;
        }

        private IEnumerator Start()
        {
            _ambienceItem.Target = gameObject;

            // Play once with a shorter fade
            var originalFadeInTime = _ambienceItem.FadeInTime;
            _ambienceItem.FadeInTime = 1;
            yield return Play();

            // Restore the original fade time
            _ambienceItem.FadeInTime = originalFadeInTime;

            while (true)
            {
                yield return Play();
            }
        }

        private IEnumerator Play()
        {
            var item = GameManager.AudioManager.Play(_ambienceItem);
            var fadeOutTime = _ambienceItem.FadeOutTime;
            var fadeOutScale = _ambienceItem.FadeOutScale;
            var fadeOutScaleWithPitch = _ambienceItem.FadeOutScaleWithPitch;

            int FadeOutSample() =>
                item.Source.clip.samples -
                Mathf.RoundToInt(item.Source.clip.frequency *
                                 (fadeOutTime / (fadeOutScale ? Time.timeScale : 1) /
                                  (fadeOutScaleWithPitch ? item.Pitch : 1) +
                                  0.05f));

            yield return new WaitUntil(() => item.Source.timeSamples >= FadeOutSample());
        }
    }
}
