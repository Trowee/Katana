using System.Collections;
using UnityEditor;
using UnityEngine;

namespace SmoothSceneCamera
{
    public class SmoothSceneCamera : MonoBehaviour
    {
        private static float _lastFrameTime;
        private static float _zoomDuration;
        
        
        [MenuItem("Tools/Zoom Out Scene Camera (Z+1)")]
        public static void ZoomOutSceneCamera()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (!sceneView) return; 

            var currentDistance = sceneView.cameraDistance;
            sceneView.LookAt(sceneView.pivot, sceneView.rotation, currentDistance + 1f,
                             sceneView.camera.orthographic, true);
        }

        private static IEnumerator ZoomRoutine()
        {
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var deltaTime = (float)(EditorApplication.timeSinceStartup - _lastFrameTime);
                lerpPos += deltaTime / _zoomDuration;
                lerpPos = Mathf.Clamp01(lerpPos);
                
                
                
                _lastFrameTime = (float)EditorApplication.timeSinceStartup;
                yield return null;
            }
            
            yield return null;
        }
    }
}
