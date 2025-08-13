using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Evets
{
    [ExecuteAlways] // this script runs in edit mode to see changes to skybox live
    public class SkyboxController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private SkyboxSettings skyboxSettings;
        
        [Header("References")]
        // !!! important, required for skybox to function as default
        // Celestial bodies in skybox follows the rotation of these transforms
        [SerializeField] private Transform sun;
        [SerializeField] private Transform moon;
        [SerializeField] private Transform moon1;
        [SerializeField] private Transform moon2;
        
        [Header("Main Light")]
        [SerializeField] private bool isMatchingDirectionalLightWithSun = true;
        [Tooltip("You can reference your own directional light in scene, " +
                 "directional light will always match the angle of the dominant celestial body that is currently in a positive angle (above horizon).")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private float directionalLightMatchingSpeed = 10f;
        
        [Header("Sun Rotation")]
        [SerializeField] private bool isRotatingSun = true;
        [Tooltip("Day time in hours, used to calculate the rotation of the sun. " +
                 "The sun will rotate around the Y axis, while the moon will rotate around the Z axis.")]
        [SerializeField] private float dayTime = 16f; // 16 hours for a full rotation, default is 24 hours
        
        [Header("Sunset Angles (degrees)")]
        [Tooltip("Sunset angle dictates when the sun is considered to be below the horizon. " +
                 "The moon will take over as main light when the sun is below this angle. " +
                 "The sunset leeway angle is used to smoothly transition between sun and moon. " +
                 "Set both angles to 90 to disable sunset transition.")]
        [SerializeField] private float sunsetThresholdAngle = 70;
        [SerializeField] private float sunsetLeewayAngle = 30;
        
        private float intensityMultiplier;
        // shader values
        private static readonly int SunDir = Shader.PropertyToID("_SunDir");
        private static readonly int MoonDir = Shader.PropertyToID("_MoonDir");
        private static readonly int MoonSpaceMatrix = Shader.PropertyToID("_MoonSpaceMatrix");
        private static readonly int MoonDir1 = Shader.PropertyToID("_MoonDir1");
        private static readonly int MoonSpaceMatrix1 = Shader.PropertyToID("_MoonSpaceMatrix1");
        private static readonly int MoonDir2 = Shader.PropertyToID("_MoonDir2");
        private static readonly int MoonSpaceMatrix2 = Shader.PropertyToID("_MoonSpaceMatrix2");
        
        private void Update()
        {
            if (Application.isPlaying && isRotatingSun) RotateSun(360f, dayTime * 60f);
        }

        private void LateUpdate()
        {
            // update shader values for each celestial body following their respective transforms
            Shader.SetGlobalVector(SunDir, -sun.forward); // Sun
            Shader.SetGlobalVector(MoonDir, -moon.forward); // Moon
            Shader.SetGlobalMatrix(MoonSpaceMatrix, new Matrix4x4(-moon.forward, 
                -moon.up, -moon.right, Vector4.zero).transpose); // Moon
            Shader.SetGlobalVector(MoonDir1, -moon1.forward); // Moon1
            Shader.SetGlobalMatrix(MoonSpaceMatrix1, new Matrix4x4(-moon1.forward, 
                -moon1.up, -moon1.right, Vector4.zero).transpose); // Moon1
            Shader.SetGlobalVector(MoonDir2, -moon2.forward); // Moon2
            Shader.SetGlobalMatrix(MoonSpaceMatrix2, new Matrix4x4(-moon2.forward, 
                -moon2.up, -moon2.right, Vector4.zero).transpose); // Moon2
            
            // match directional light to the current dominant celestial body
            if (isMatchingDirectionalLightWithSun) MatchLighting();
        }

        private void MatchLighting()
        {
            if (!directionalLight)
            {
                Debug.LogWarning("Remember to assign a directional light in the SkyboxController component to match the lighting with the skybox's visuals");
                return;
            }
            // angle < 90 means below horizon
            float currentSunAngle = Vector3.Angle(Vector3.up, sun.forward);
            float t = (currentSunAngle - sunsetThresholdAngle) / sunsetLeewayAngle;

            // switch to moon as main light when sun is down
            directionalLight.intensity = Mathf.Lerp(0.01f, 1, t);
            var targetRotation = sun.rotation;
            
            var isMoonAbove = Vector3.Dot(moon.forward, Vector3.down) > 0;
            var isMoon1Above = Vector3.Dot(moon1.forward, Vector3.down) > 0;
            var isMoon2Above = Vector3.Dot(moon2.forward, Vector3.down) > 0;
            
            if (Vector3.Dot(sun.forward, Vector3.down) < 0)
            {
                if (isMoonAbove) targetRotation = moon.rotation;
                else if (isMoon1Above) targetRotation = moon1.rotation;
                else if (isMoon2Above) targetRotation = moon2.rotation;
            }
    
            directionalLight.transform.rotation = Quaternion.Lerp(directionalLight.transform.rotation, targetRotation,
                directionalLightMatchingSpeed * Time.deltaTime);
            
            if (!skyboxSettings) return;
            // reduce intensity of directional light based on cloudiness
            directionalLight.intensity *= Mathf.Lerp(1, .7f, skyboxSettings.cloudiness);
        }
        
        private void RotateSun(float degree, float duration)
        {
            //sun rotation
            float sunRotateSpeed = degree / duration;
            sun.localRotation *= Quaternion.AngleAxis(sunRotateSpeed * Time.deltaTime, Vector3.right);
        }
    }
}
