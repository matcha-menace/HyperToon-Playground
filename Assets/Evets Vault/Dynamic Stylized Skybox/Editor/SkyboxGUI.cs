using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EvetsVault
{
    public class SkyboxGUI
    {
        public static class Info {
            private const string Version = "1.0.0";
            private const string ReleaseDate = "05.30.2025";
            private const string Message = "by evets.";
    
            public static readonly string FullInfo = $"{Message} version: {Version} ({ReleaseDate})";
        }
        
        public static void DrawLogo()
        {
            var temp = ScriptableObject.CreateInstance<SkyboxSettings>();
            MonoScript script = MonoScript.FromScriptableObject(temp);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            Object.DestroyImmediate(temp);
            string scriptDir = Path.GetDirectoryName(scriptPath);
            string parentDir = Directory.GetParent(scriptDir)?.FullName;
            string relativeParentDir = parentDir.Replace(Application.dataPath, "Assets");
            string path = $"{relativeParentDir}\\Editor\\Banner\\skybox_banner.png";
            int index = path.IndexOf("Assets");
            string relativePath = index >= 0 ? path.Substring(index) : path;
            Texture2D banner = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

            if (banner != null)
            {
                float bannerWidth = EditorGUIUtility.currentViewWidth - 40f; // Adjust if needed
                float bannerHeight = banner.height * (bannerWidth / banner.width);
                Rect rect = GUILayoutUtility.GetRect(bannerWidth, bannerHeight, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(rect, banner, ScaleMode.ScaleToFit);
                GUILayout.Space(10);
            }
            else
            {
                EditorGUILayout.HelpBox("Banner not found:\n" + path, MessageType.Warning);
            }
            
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 18;
            titleStyle.fontStyle = FontStyle.BoldAndItalic;
            titleStyle.richText = true;
            
            GUIStyle subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 11;
            subtitleStyle.fontStyle = FontStyle.Italic;
            subtitleStyle.normal.textColor = Color.gray;
            
            GUIStyle messageStyle = new GUIStyle();
            messageStyle.fontSize = 12;
            messageStyle.fontStyle = FontStyle.Bold;
            messageStyle.normal.textColor = new Color(.7f, .7f, .7f);
            
            string coloredTitle = "<color=#EFEFEF>Eve's</color> " +
                                  "<color=#7ad8f4>S</color><color=#7acaf4>k</color><color=#7abcf4>y</color>" +
                                  "<color=#7ab1f4>b</color><color=#7aaaf4>o</color><color=#7a98f4>x</color>";
            EditorGUILayout.LabelField(coloredTitle, titleStyle);
            EditorGUILayout.LabelField(Info.FullInfo, subtitleStyle);
            EditorGUILayout.LabelField("Please refer to README for starter guide and detailed documentation." +
                                       "\nContact: evets.dev@gmail.com", messageStyle);
            
            EditorGUILayout.Space(20);
            Rect r = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(1));
            EditorGUI.DrawRect(r, new Color(0.3f, 0.3f, 0.3f));
            EditorGUILayout.Space();
        }
    }
    
    [CustomEditor(typeof(SkyboxSettings))]
    public class SkyboxEditorInspector : Editor
    {
        private SerializedProperty sunRadius;
        private SerializedProperty sunIntensity;
        private SerializedProperty synthwaveSun;
        private SerializedProperty synthSunBottom;
        private SerializedProperty synthSunLines;

        private SerializedProperty moonTurnOn;
        private SerializedProperty moonRadius;
        private SerializedProperty moonEdgeStrength;
        private SerializedProperty moonExposure;
        private SerializedProperty moonDarkside;
        private SerializedProperty moonTexture;

        private SerializedProperty cloudTurnOn;
        private SerializedProperty cloudCubeMap;
        private SerializedProperty cloudSpeed;
        private SerializedProperty cloudBackCubeMap;
        private SerializedProperty cloudiness;

        private SerializedProperty starCubeMap;
        private SerializedProperty starSpeed;
        private SerializedProperty starExposure;
        private SerializedProperty starPower;
        private SerializedProperty starLatitude;

        private SerializedProperty nightDayGradient;
        private SerializedProperty horizonZenithGradient;
        private SerializedProperty sunHaloGradient;
        private SerializedProperty cloudColorGradient;

        private void OnEnable()
        {
            sunRadius = serializedObject.FindProperty("sunRadius");
            sunIntensity = serializedObject.FindProperty("sunIntensity");
            synthwaveSun = serializedObject.FindProperty("synthwaveSun");
            synthSunBottom = serializedObject.FindProperty("synthSunBottom");
            synthSunLines = serializedObject.FindProperty("synthSunLines");

            moonTurnOn = serializedObject.FindProperty("moonTurnOn");
            moonRadius = serializedObject.FindProperty("moonRadius");
            moonEdgeStrength = serializedObject.FindProperty("moonEdgeStrength");
            moonExposure = serializedObject.FindProperty("moonExposure");
            moonDarkside = serializedObject.FindProperty("moonDarkside");
            moonTexture = serializedObject.FindProperty("moonTexture");

            cloudTurnOn = serializedObject.FindProperty("cloudTurnOn");
            cloudCubeMap = serializedObject.FindProperty("cloudCubeMap");
            cloudSpeed = serializedObject.FindProperty("cloudSpeed");
            cloudBackCubeMap = serializedObject.FindProperty("cloudBackCubeMap");
            cloudiness = serializedObject.FindProperty("cloudiness");

            starCubeMap = serializedObject.FindProperty("starCubeMap");
            starSpeed = serializedObject.FindProperty("starSpeed");
            starExposure = serializedObject.FindProperty("starExposure");
            starPower = serializedObject.FindProperty("starPower");
            starLatitude = serializedObject.FindProperty("starLatitude");

            nightDayGradient = serializedObject.FindProperty("nightDayGradient");
            horizonZenithGradient = serializedObject.FindProperty("horizonZenithGradient");
            sunHaloGradient = serializedObject.FindProperty("sunHaloGradient");
            cloudColorGradient = serializedObject.FindProperty("cloudColorGradient");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SkyboxGUI.DrawLogo();
            
            // --- GRADIENTS ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gradient", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Gradient dictates the sky colors from Night to Day." +
                                    "\nReset Gradient: Reset all gradients to a preset default color scheme." +
                                    "\nSave Gradient: Auto-generate a 128x1 gradient texture to be saved under /Textures/SkyGradients."
                                    , MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(nightDayGradient);
            EditorGUILayout.PropertyField(horizonZenithGradient);
            EditorGUILayout.PropertyField(sunHaloGradient);
            EditorGUILayout.PropertyField(cloudColorGradient);

            EditorGUILayout.Space();
            SkyboxSettings s = (SkyboxSettings)target;

            if (GUILayout.Button("Reset Gradients"))
                s.ResetGradients();

            if (GUILayout.Button("Update Gradients"))
                s.UpdateGradients();

            // --- SUN ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sun", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Controls sun visuals and intensity." +
                                    "\nBy checking \"Synthwave Sun\", a stylized effect will be applied when the sun is at a low angle.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(sunRadius);
            EditorGUILayout.PropertyField(sunIntensity);
            EditorGUILayout.PropertyField(synthwaveSun);
            if (synthwaveSun.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(synthSunBottom);
                EditorGUILayout.PropertyField(synthSunLines);
                EditorGUI.indentLevel--;
            }

            // --- MOON ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Moon", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Toggles moon visuals and sets appearance details like exposure and edge strength." +
                                    "\nYou can apply customized moon texture by changing the cubemap.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(moonTurnOn);
            if (moonTurnOn.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(moonTexture);
                EditorGUILayout.PropertyField(moonRadius);
                EditorGUILayout.PropertyField(moonEdgeStrength);
                EditorGUILayout.PropertyField(moonExposure);
                EditorGUILayout.PropertyField(moonDarkside);
                EditorGUI.indentLevel--;
            }
            
            // --- CLOUDS ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clouds", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Cloud using 2 layers of cubemaps. Adjust speed and density here." +
                                    "\nEach layer can be adjusted separately with your custom cubemaps.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(cloudTurnOn);
            if (cloudTurnOn.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(cloudCubeMap);
                EditorGUILayout.PropertyField(cloudBackCubeMap);
                EditorGUILayout.PropertyField(cloudiness);
                EditorGUILayout.PropertyField(cloudSpeed);
                EditorGUI.indentLevel--;
            
            }

            // --- STARS ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stars", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configure starfield rotation and brightness settings. Stars will only show up when sun is at low or negative angles." +
                                    "\nAlternatively you can use your own custom cubemap for stars.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(starCubeMap);
            EditorGUILayout.PropertyField(starSpeed);
            EditorGUILayout.PropertyField(starExposure);
            EditorGUILayout.PropertyField(starPower);
            EditorGUILayout.PropertyField(starLatitude);

            serializedObject.ApplyModifiedProperties();
        }
    }
    
    public class CustomSkyboxShaderGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 12;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = new Color(.7f, .7f, .7f);
            
            EditorGUI.indentLevel = 0;
            GUILayout.Space(20);
            GUIContent warningIcon = EditorGUIUtility.IconContent("console.warnicon.sml");
            GUILayout.Label(warningIcon, GUILayout.Width(20), GUILayout.Height(20));
            GUILayout.Label(
                "This material is controlled by the Evets_SkyboxSettings ScriptableObject.\nPlease make changes there.", titleStyle);
        }
    }
}