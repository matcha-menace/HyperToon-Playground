using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EvetsVault
{
    [ExecuteAlways] // this script runs in edit mode to see changes to skybox live
    public class SkyboxController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SkyboxSettings skyboxSettings;
        [SerializeField] private Transform sun;
        [SerializeField] private Transform moon;
        
        [Header("Main Light")]
        [Tooltip("You can reference your own directional light in scene, " +
                 "directional light will always match the angle of the dominant celestial body that is currently in a positive angle (above horizon).")]
        [SerializeField] private Light directionalLight;
        
        [Header("Sunset Angles (degrees)")]
        [Tooltip("Sunset angle dictates when the sun is considered to be below the horizon. " +
                 "The moon will take over as main light when the sun is below this angle. " +
                 "The sunset leeway angle is used to smoothly transition between sun and moon. " +
                 "Set both angles to 90 to disable sunset transition.")]
        [SerializeField] private float sunsetThresholdAngle = 70;
        [SerializeField] private float sunsetLeewayAngle = 30;
        
        private float intensityMultiplier;
        private static readonly int SunDir = Shader.PropertyToID("_SunDir");
        private static readonly int MoonDir = Shader.PropertyToID("_MoonDir");
        private static readonly int MoonSpaceMatrix = Shader.PropertyToID("_MoonSpaceMatrix");

        private void LateUpdate()
        {
            // Sun
            Shader.SetGlobalVector(SunDir, -sun.forward);
            // Moon
            Shader.SetGlobalVector(MoonDir, -moon.forward);
            Shader.SetGlobalMatrix(MoonSpaceMatrix, new Matrix4x4(-moon.forward, 
                -moon.up, -moon.right, Vector4.zero).transpose);
            
            MatchLighting();
        }

        private void MatchLighting()
        {
            if (!directionalLight) return;
            
            // angle < 90 means below horizon
            float currentSunAngle = Vector3.Angle(Vector3.up, sun.forward);
            float t = (currentSunAngle - sunsetThresholdAngle) / sunsetLeewayAngle;

            // switch to moon as main light when sun is down
            // incorrect (sun is still lighting the scene) main light when both are down
            directionalLight.intensity = Mathf.Lerp(0.01f, 1, t);
            if (directionalLight.intensity < .2f && Vector3.Angle(Vector3.up, moon.forward) > 90)
            {
                directionalLight.transform.rotation = moon.rotation;
            }
            else
            {
                directionalLight.transform.rotation = sun.rotation;
            }
            
            if (!skyboxSettings) return;
            directionalLight.intensity *= Mathf.Lerp(1, .7f, skyboxSettings.cloudiness);
        }
    }
}
