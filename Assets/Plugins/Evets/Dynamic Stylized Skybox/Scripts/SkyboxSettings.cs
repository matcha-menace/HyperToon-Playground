using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

namespace Evets
{
    public class SkyboxSettings : ScriptableObject
    {
        private const int Resolution = 128;

        // sky dynamic gradient
        [SerializeField] private Gradient nightDayGradient;
        [SerializeField] private Gradient horizonZenithGradient;
        [SerializeField] private Gradient sunHaloGradient;
        [SerializeField] private Gradient sunColorGradient;
        [SerializeField] private Gradient cloudColorGradient;
        [SerializeField, Range(0f, 4f)] private float skyExposure = 1f;
        
        // sun
        [Range(0f, 1f), SerializeField] private float sunRadius = 0.12f;
        [Range(1f, 20f), SerializeField] private float sunIntensity = 4;
        [SerializeField, Range(2, 100)] private float sunHaloStrength = 24;
        [SerializeField, Range(1, 200)] private float sunEdgeFalloff = 1;
        [SerializeField, Range(0f, 1f)] private float sunCoreSharpness = 0f;
        [SerializeField] private bool customizeSunColors;
        [SerializeField] private bool sunTexture = false;
        [SerializeField] private Cubemap sunCubeMap;
        [Range(0f, 1f), SerializeField] private float sunTextureStrength = .8f;
        [SerializeField] private bool synthwaveSun = false;
        [Range(0f, 1f), SerializeField] private float synthSunBottom = .5f;
        [SerializeField] private float synthSunLines = 64;
        // moon
        [Range(0, 3), SerializeField] private int moonCount = 1;
        [SerializeField] private Cubemap moonTexture;
        [SerializeField] private bool moonTurnOn = true;
        [Range(0f, 1f), SerializeField] private float moonRadius = .3f;
        [Range(0.01f, 1f), SerializeField] private float moonEdgeStrength = .4f;
        [Range(-16, 0), SerializeField] private float moonExposure = 0;
        [Range(0, .9f), SerializeField] private float moonDarkside = 0;
        
        [SerializeField] private Cubemap moonTexture1;
        [SerializeField] private bool moonTurnOn1 = true;
        [Range(0f, 1f), SerializeField] private float moonRadius1 = .3f;
        [Range(0.01f, 1f), SerializeField] private float moonEdgeStrength1 = .4f;
        [Range(-16, 0), SerializeField] private float moonExposure1 = 0;
        [Range(0, .9f), SerializeField] private float moonDarkside1 = 0;
        
        [SerializeField] private Cubemap moonTexture2;
        [SerializeField] private bool moonTurnOn2 = true;
        [Range(0f, 1f), SerializeField] private float moonRadius2 = .3f;
        [Range(0.01f, 1f), SerializeField] private float moonEdgeStrength2 = .4f;
        [Range(-16, 0), SerializeField] private float moonExposure2 = 0;
        [Range(0, .9f), SerializeField] private float moonDarkside2 = 0;
        // stars
        [SerializeField] private Cubemap starCubeMap;
        [Range(0f, .1f), SerializeField] private float starSpeed = .005f;
        [Range(-16, 16), SerializeField] private int starExposure = 0;
        [Range(1f, 5f), SerializeField] private float starPower = 1.8f;
        [Range(-90, 90), SerializeField] private int starLatitude = -8;
        // clouds
        [SerializeField] private bool cloudTurnOn = true;
        [Range(0.2f, 1f), SerializeField] private float cloudAlpha = 0.6f;
        [SerializeField] private Cubemap cloudCubeMap;
        [SerializeField] private Cubemap cloudBackCubeMap;
        [Range(0f, .1f), SerializeField] private float cloudSpeed = .01f;
        [Range(0f, 1f), SerializeField] public float cloudiness = 0f;

        private void OnValidate()
        {
            // refresh shader values on validation
            if (RenderSettings.skybox.shader.name != "Evets/Skybox")
            {
                Debug.Log("Evets Skybox: make sure to use correct material for skybox!");
                return;
            }
            SetSkyboxValues();
        }

        private void SetSkyboxValues()
        {
            RenderSettings.skybox.SetFloat("_SkyExposure", skyExposure);
            RenderSettings.skybox.SetFloat("_SunRadius", sunRadius);
            RenderSettings.skybox.SetFloat("_SunIntensity", sunIntensity);
            RenderSettings.skybox.SetFloat("_SunHaloStrength", sunHaloStrength);
            RenderSettings.skybox.SetFloat("_SunEdgeFalloff", sunEdgeFalloff);
            RenderSettings.skybox.SetFloat("_SunCoreSharpness", sunCoreSharpness);
            RenderSettings.skybox.SetFloat("_SunColorCustomize", customizeSunColors ? 1 : 0);
            RenderSettings.skybox.SetFloat("_SunTextureOn", sunTexture ? 1 : 0);
            RenderSettings.skybox.SetTexture("_SunCubeMap", sunCubeMap);
            RenderSettings.skybox.SetFloat("_SunTextureStrength", sunTextureStrength);
            RenderSettings.skybox.SetFloat("_SynthSun", synthwaveSun ? 1 : 0);
            RenderSettings.skybox.SetFloat("_SynthSunBottom", synthSunBottom);
            RenderSettings.skybox.SetFloat("_SynthSunLines", synthSunLines);
            
            moonTurnOn = moonCount > 0;
            moonTurnOn1 = moonCount > 1;
            moonTurnOn2 = moonCount > 2;
            
            if (moonCount < 1) RenderSettings.skybox.DisableKeyword("MOON_ON");
            else RenderSettings.skybox.EnableKeyword("MOON_ON");
            
            RenderSettings.skybox.SetFloat("_MoonOn", moonTurnOn ? 1 : 0);
            RenderSettings.skybox.SetFloat("_MoonRadius", moonRadius);
            RenderSettings.skybox.SetFloat("_MoonEdgeStrength", moonEdgeStrength);
            RenderSettings.skybox.SetFloat("_MoonExposure", moonExposure);
            RenderSettings.skybox.SetFloat("_MoonDarkside", moonDarkside);
            RenderSettings.skybox.SetTexture("_MoonCubeMap", moonTexture);
            
            RenderSettings.skybox.SetFloat("_MoonOn1", moonTurnOn1 ? 1 : 0);
            RenderSettings.skybox.SetFloat("_MoonRadius1", moonRadius1);
            RenderSettings.skybox.SetFloat("_MoonEdgeStrength1", moonEdgeStrength1);
            RenderSettings.skybox.SetFloat("_MoonExposure1", moonExposure1);
            RenderSettings.skybox.SetFloat("_MoonDarkside1", moonDarkside1);
            RenderSettings.skybox.SetTexture("_MoonCubeMap1", moonTexture1);
            
            RenderSettings.skybox.SetFloat("_MoonOn2", moonTurnOn2 ? 1 : 0);
            RenderSettings.skybox.SetFloat("_MoonRadius2", moonRadius2);
            RenderSettings.skybox.SetFloat("_MoonEdgeStrength2", moonEdgeStrength2);
            RenderSettings.skybox.SetFloat("_MoonExposure2", moonExposure2);
            RenderSettings.skybox.SetFloat("_MoonDarkside2", moonDarkside2);
            RenderSettings.skybox.SetTexture("_MoonCubeMap2", moonTexture2);
            
            RenderSettings.skybox.SetTexture("_StarCubeMap", starCubeMap);
            RenderSettings.skybox.SetFloat("_StarSpeed", starSpeed * .1f);
            RenderSettings.skybox.SetFloat("_StarExposure", starExposure);
            RenderSettings.skybox.SetFloat("_StarPower", starPower);
            RenderSettings.skybox.SetFloat("_StarLatitude", starLatitude);
            
            RenderSettings.skybox.SetTexture("_CloudCubeMap", cloudCubeMap);
            RenderSettings.skybox.SetFloat("_CloudSpeed", cloudSpeed * .1f);
            RenderSettings.skybox.SetFloat("_CloudOn", cloudTurnOn ? 1 : 0);
            RenderSettings.skybox.SetFloat("_CloudAlpha", cloudAlpha);
            RenderSettings.skybox.SetTexture("_CloudBackCubeMap", cloudBackCubeMap);
            RenderSettings.skybox.SetFloat("_Cloudiness", cloudiness);
        }

        /// <summary>
        /// Parse hex string to color
        /// </summary>
        /// <param name="hex">hex string</param>
        /// <returns>color</returns>
        private Color GetColorFromHex(string hex)
        {
            Color col = Color.black;
            if (ColorUtility.TryParseHtmlString("#" + hex + "FF", out col))
            {
                return col;
            }

            return col;
        }

        /// <summary>
        /// Save single gradient to texture
        /// </summary>
        /// <param name="gradient">gradient</param>
        /// <param name="textureName">name of saved texture</param>
        private void SaveGradient(Gradient gradient, string textureName)
        {
#if UNITY_EDITOR
            Texture2D tex = new Texture2D(Resolution, 1);
            for (int x = 0; x < Resolution; x++)
            {
                tex.SetPixel(x, 0, gradient.Evaluate(x / (float)Resolution));
            }

            tex.Apply();

            byte[] data = tex.EncodeToPNG();
            MonoScript ms = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(ms);
            string scriptDir = Path.GetDirectoryName(scriptPath);
            string parentDir = Directory.GetParent(scriptDir)?.FullName;
            string relativeParentDir = parentDir.Replace(Application.dataPath, "Assets");
            
            string path = $"{relativeParentDir}/Textures/SkyGradients/{textureName}.png";
            File.WriteAllBytes(path, data);

            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// Reset and updates all gradients
        /// </summary>
        [ContextMenu("Reset Gradients")]
        public void ResetGradients()
        {
            // default alpha Keys
            GradientAlphaKey[] defaulAlphas = new GradientAlphaKey[2];
            defaulAlphas[0].alpha = 1f;
            defaulAlphas[1].alpha = 1f;
            defaulAlphas[0].time = 0f;
            defaulAlphas[1].time = 1f;

            GradientColorKey[] defaultSunZenithColors = new GradientColorKey[4];
            defaultSunZenithColors[0].color = GetColorFromHex("0D0E17");
            defaultSunZenithColors[1].color = GetColorFromHex("13161D");
            defaultSunZenithColors[2].color = GetColorFromHex("61B0D8");
            defaultSunZenithColors[3].color = GetColorFromHex("4A87C8");
            defaultSunZenithColors[0].time = 0f;
            defaultSunZenithColors[1].time = .347f;
            defaultSunZenithColors[2].time = .721f;
            defaultSunZenithColors[3].time = 1f;
            nightDayGradient.SetKeys(defaultSunZenithColors, defaulAlphas);

            GradientColorKey[] horizonZenithColors = new GradientColorKey[6];
            horizonZenithColors[0].color = GetColorFromHex("15151A");
            horizonZenithColors[1].color = GetColorFromHex("284167");
            horizonZenithColors[2].color = GetColorFromHex("FE6E00");
            horizonZenithColors[3].color = GetColorFromHex("CF3663");
            horizonZenithColors[4].color = GetColorFromHex("A8D5EC");
            horizonZenithColors[5].color = GetColorFromHex("A7C7EA");
            horizonZenithColors[0].time = 0f;
            horizonZenithColors[1].time = .318f;
            horizonZenithColors[2].time = .515f;
            horizonZenithColors[3].time = .622f;
            horizonZenithColors[4].time = .675f;
            horizonZenithColors[5].time = 1f;
            horizonZenithGradient.SetKeys(horizonZenithColors, defaulAlphas);

            GradientColorKey[] defaultSunHaloColors = new GradientColorKey[5];
            defaultSunHaloColors[0].color = GetColorFromHex("000000");
            defaultSunHaloColors[1].color = GetColorFromHex("000000");
            defaultSunHaloColors[2].color = GetColorFromHex("FD9E13");
            defaultSunHaloColors[3].color = GetColorFromHex("ABE2E7");
            defaultSunHaloColors[4].color = GetColorFromHex("C7D9D8");
            defaultSunHaloColors[0].time = 0f;
            defaultSunHaloColors[1].time = .174f;
            defaultSunHaloColors[2].time = .526f;
            defaultSunHaloColors[3].time = .659f;
            defaultSunHaloColors[4].time = 1f;
            sunHaloGradient.SetKeys(defaultSunHaloColors, defaulAlphas);
            
            GradientColorKey[] defaultSunColors = new GradientColorKey[5];
            defaultSunColors[0].color = GetColorFromHex("000000");
            defaultSunColors[1].color = GetColorFromHex("000000");
            defaultSunColors[2].color = GetColorFromHex("FD5E13");
            defaultSunColors[3].color = GetColorFromHex("FAA4A4");
            defaultSunColors[4].color = GetColorFromHex("FFFFFF");
            defaultSunColors[0].time = 0f;
            defaultSunColors[1].time = .174f;
            defaultSunColors[2].time = .447f;
            defaultSunColors[3].time = .653f;
            defaultSunColors[4].time = 1f;
            sunColorGradient.SetKeys(defaultSunColors, defaulAlphas);
            
            GradientColorKey[] defaultCloudColors = new GradientColorKey[7];
            defaultCloudColors[0].color = GetColorFromHex("24262E");
            defaultCloudColors[1].color = GetColorFromHex("2B2B2B");
            defaultCloudColors[2].color = GetColorFromHex("352929");
            defaultCloudColors[3].color = GetColorFromHex("EE2A42");
            defaultCloudColors[4].color = GetColorFromHex("F5D952");
            defaultCloudColors[5].color = GetColorFromHex("FFFFFF");
            defaultCloudColors[6].color = GetColorFromHex("FFFFFF");
            defaultCloudColors[0].time = 0f;
            defaultCloudColors[1].time = .338f;
            defaultCloudColors[2].time = .397f;
            defaultCloudColors[3].time = .531f;
            defaultCloudColors[4].time = .614f;
            defaultCloudColors[5].time = .709f;
            defaultCloudColors[6].time = 1f;
            cloudColorGradient.SetKeys(defaultCloudColors, defaulAlphas);

            UpdateGradients();
        }

        /// <summary>
        /// Save all gradients to texture
        /// </summary>
        [ContextMenu("Update Gradients")]
        public void UpdateGradients()
        {
            SaveGradient(nightDayGradient, "NightDayGradient");
            SaveGradient(horizonZenithGradient, "HorizonZenithGradient");
            SaveGradient(sunHaloGradient, "SunHaloGradient");
            SaveGradient(cloudColorGradient, "CloudColorGradient");
            SaveGradient(sunColorGradient, "SunColorGradient");
        }
    }
}
