using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Evets
{
    public class SkyboxGUI
    {
        public static class Info {
            private const string Version = "1.3.2";
            private const string Message = "by evets.";
    
            public static readonly string FullInfo = $"{Message} {Version}";
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
            
            string coloredTitle = "<color=#7ad8f4>S</color><color=#7acaf4>k</color><color=#7abcf4>y</color>" +
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
    
    [CustomEditor(typeof(SkyboxSettings)), CanEditMultipleObjects]
    public class SkyboxEditorInspector : Editor
    {
        private bool sunFoldout = true;
        private bool moonGeneralFoldout = true;
        private bool cloudFoldout = true;
        private bool starFoldout = true;
        
        private SerializedProperty nightDayGradient;
        private SerializedProperty horizonZenithGradient;
        private SerializedProperty sunHaloGradient;
        private SerializedProperty cloudColorGradient;
        private SerializedProperty sunColorGradient;
        
        private SerializedProperty sunRadius;
        private SerializedProperty sunIntensity;
        private SerializedProperty synthwaveSun;
        private SerializedProperty synthSunBottom;
        private SerializedProperty synthSunLines;
        private SerializedProperty sunTextureSwitch;
        private SerializedProperty sunTexture;
        private SerializedProperty sunTextureStrength;
        private SerializedProperty sunColorCustomizeSwitch;

        // Moons
        private bool moonFoldout = true;
        private SerializedProperty moonCount;
        private SerializedProperty moonTurnOn;
        private SerializedProperty moonRadius;
        private SerializedProperty moonEdgeStrength;
        private SerializedProperty moonExposure;
        private SerializedProperty moonDarkside;
        private SerializedProperty moonTexture;
        
        private bool moon1Foldout = true;
        private SerializedProperty moonTurnOn1;
        private SerializedProperty moonRadius1;
        private SerializedProperty moonEdgeStrength1;
        private SerializedProperty moonExposure1;
        private SerializedProperty moonDarkside1;
        private SerializedProperty moonTexture1;
        
        private bool moon2Foldout = true;
        private SerializedProperty moonTurnOn2;
        private SerializedProperty moonRadius2;
        private SerializedProperty moonEdgeStrength2;
        private SerializedProperty moonExposure2;
        private SerializedProperty moonDarkside2;
        private SerializedProperty moonTexture2;
        
        private SerializedProperty starCubeMap;
        private SerializedProperty starSpeed;
        private SerializedProperty starExposure;
        private SerializedProperty starPower;
        private SerializedProperty starLatitude;

        private SerializedProperty cloudTurnOn;
        private SerializedProperty cloudAlpha;
        private SerializedProperty cloudCubeMap;
        private SerializedProperty cloudSpeed;
        private SerializedProperty cloudBackCubeMap;
        private SerializedProperty cloudiness;

        private void OnEnable()
        {
            sunRadius = serializedObject.FindProperty("sunRadius");
            sunIntensity = serializedObject.FindProperty("sunIntensity");
            synthwaveSun = serializedObject.FindProperty("synthwaveSun");
            synthSunBottom = serializedObject.FindProperty("synthSunBottom");
            synthSunLines = serializedObject.FindProperty("synthSunLines");
            sunTextureSwitch = serializedObject.FindProperty("sunTexture");
            sunTexture = serializedObject.FindProperty("sunCubeMap");
            sunTextureStrength = serializedObject.FindProperty("sunTextureStrength");
            sunColorCustomizeSwitch = serializedObject.FindProperty("customizeSunColors");

            // moons
            moonCount = serializedObject.FindProperty("moonCount");
            moonTurnOn = serializedObject.FindProperty("moonTurnOn");
            moonRadius = serializedObject.FindProperty("moonRadius");
            moonEdgeStrength = serializedObject.FindProperty("moonEdgeStrength");
            moonExposure = serializedObject.FindProperty("moonExposure");
            moonDarkside = serializedObject.FindProperty("moonDarkside");
            moonTexture = serializedObject.FindProperty("moonTexture");
            
            moonTurnOn1 = serializedObject.FindProperty("moonTurnOn1");
            moonRadius1 = serializedObject.FindProperty("moonRadius1");
            moonEdgeStrength1 = serializedObject.FindProperty("moonEdgeStrength1");
            moonExposure1 = serializedObject.FindProperty("moonExposure1");
            moonDarkside1 = serializedObject.FindProperty("moonDarkside1");
            moonTexture1 = serializedObject.FindProperty("moonTexture1");
            
            moonTurnOn2 = serializedObject.FindProperty("moonTurnOn2");
            moonRadius2 = serializedObject.FindProperty("moonRadius2");
            moonEdgeStrength2 = serializedObject.FindProperty("moonEdgeStrength2");
            moonExposure2 = serializedObject.FindProperty("moonExposure2");
            moonDarkside2 = serializedObject.FindProperty("moonDarkside2");
            moonTexture2 = serializedObject.FindProperty("moonTexture2");

            // clouds
            cloudTurnOn = serializedObject.FindProperty("cloudTurnOn");
            cloudAlpha = serializedObject.FindProperty("cloudAlpha");
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
            sunColorGradient = serializedObject.FindProperty("sunColorGradient");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SkyboxGUI.DrawLogo();
            
            // --- GRADIENTS ---
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dynamic Sky Gradients", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Gradient dictates the element's colors from NIGHT to DAY." +
                                    "\nReset Gradient: Reset all gradients to a preset default color scheme." +
                                    "\nSave Gradient: Auto-generate a 128x1 gradient texture to be saved under /Textures/SkyGradients."
                                    , MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(nightDayGradient);
            EditorGUILayout.PropertyField(horizonZenithGradient);
            EditorGUILayout.PropertyField(sunHaloGradient);
            EditorGUILayout.LabelField("Optional Colors", EditorStyles.label);
            EditorGUI.indentLevel++;
            if (sunColorCustomizeSwitch.boolValue)
            {
                EditorGUILayout.PropertyField(sunColorGradient);
            }
            else
            {
                EditorGUILayout.HelpBox("Turn on Customize Sun Color to modify.", MessageType.None);
            }
            if (cloudTurnOn.boolValue)
            {
                EditorGUILayout.PropertyField(cloudColorGradient);
            }
            else
            {
                EditorGUILayout.HelpBox("Turn on Clouds to modify.", MessageType.None);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            SkyboxSettings s = (SkyboxSettings)target;

            if (GUILayout.Button("Reset All Gradients"))
                s.ResetGradients();

            if (GUILayout.Button("Update All Gradients"))
                s.UpdateGradients();

            // --- SUN ---
            EditorGUILayout.Space();
            sunFoldout = EditorGUILayout.Foldout(sunFoldout, "Sun", true, new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold});
            EditorGUILayout.HelpBox("Controls sun visuals and intensity.", MessageType.Info);
            if (sunFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(sunRadius);
                EditorGUILayout.PropertyField(sunIntensity);
                EditorGUILayout.PropertyField(sunColorCustomizeSwitch);
                if (!sunColorCustomizeSwitch.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.HelpBox("Sun colors are controlled by Directional Light color by default", MessageType.None);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(sunTextureSwitch);
                if (sunTextureSwitch.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sunTexture);
                    EditorGUILayout.PropertyField(sunTextureStrength);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(synthwaveSun);
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Synthwave Sun effect only applies when the sun is at a low angle.", MessageType.None);
                EditorGUI.indentLevel--;
                if (synthwaveSun.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(synthSunBottom);
                    EditorGUILayout.PropertyField(synthSunLines);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            // --- MOON ---
            EditorGUILayout.Space();
            moonGeneralFoldout = EditorGUILayout.Foldout(moonGeneralFoldout, "Moon", true, new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold});
            EditorGUILayout.HelpBox("Toggles moon visuals and sets appearance details like exposure and edge strength. " +
                                    "You can apply customized moon texture by changing the cubemap.", MessageType.Info);
            if (moonGeneralFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(moonCount);
                if (moonTurnOn.boolValue)
                {
                    EditorGUI.indentLevel++;
                    moonFoldout = EditorGUILayout.Foldout(moonFoldout, "Moon 0", true);
                    if (moonFoldout)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(moonTexture);
                        EditorGUILayout.PropertyField(moonRadius);
                        EditorGUILayout.PropertyField(moonEdgeStrength);
                        EditorGUILayout.PropertyField(moonExposure);
                        EditorGUILayout.PropertyField(moonDarkside);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                
                if (moonTurnOn1.boolValue)
                {
                    EditorGUI.indentLevel++;
                    moon1Foldout = EditorGUILayout.Foldout(moon1Foldout, "Moon 1", true);
                    if (moon1Foldout)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(moonTexture1);
                        EditorGUILayout.PropertyField(moonRadius1);
                        EditorGUILayout.PropertyField(moonEdgeStrength1);
                        EditorGUILayout.PropertyField(moonExposure1);
                        EditorGUILayout.PropertyField(moonDarkside1);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                
                if (moonTurnOn2.boolValue)
                {
                    EditorGUI.indentLevel++;
                    moon2Foldout = EditorGUILayout.Foldout(moon2Foldout, "Moon 2", true);
                    if (moon2Foldout)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(moonTexture2);
                        EditorGUILayout.PropertyField(moonRadius2);
                        EditorGUILayout.PropertyField(moonEdgeStrength2);
                        EditorGUILayout.PropertyField(moonExposure2);
                        EditorGUILayout.PropertyField(moonDarkside2);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            
            // --- STARS ---
            EditorGUILayout.Space();
            starFoldout = EditorGUILayout.Foldout(starFoldout, "Stars", true, new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold});
            EditorGUILayout.HelpBox("Stars will only show up when sun is at low or negative angles. " +
                                    "Alternatively you can use your own custom cubemap for stars.", MessageType.Info);
            if (starFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(starCubeMap);
                EditorGUILayout.PropertyField(starSpeed);
                EditorGUILayout.PropertyField(starExposure);
                EditorGUILayout.PropertyField(starPower);
                EditorGUILayout.PropertyField(starLatitude);
                EditorGUI.indentLevel--;
            }
            
            // --- CLOUDS ---
            EditorGUILayout.Space();
            cloudFoldout = EditorGUILayout.Foldout(cloudFoldout, "Clouds", true, new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold});
            EditorGUILayout.HelpBox("Cloud using 2 layers of cubemaps. Adjust speed and density here. " +
                                    "Each layer can be adjusted separately with your custom cubemaps.", MessageType.Info);
            if (cloudFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(cloudTurnOn);
                if (cloudTurnOn.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(cloudAlpha);
                    EditorGUILayout.PropertyField(cloudCubeMap);
                    EditorGUILayout.PropertyField(cloudBackCubeMap);
                    EditorGUILayout.PropertyField(cloudiness);
                    EditorGUILayout.PropertyField(cloudSpeed);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

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
            titleStyle.normal.textColor = new Color(1f, .8f, .5f);
            
            EditorGUI.indentLevel = 0;
            GUILayout.Space(20);
            GUIContent warningIcon = EditorGUIUtility.IconContent("console.warnicon.sml");
            GUILayout.Label(warningIcon, GUILayout.Width(20), GUILayout.Height(20));
            GUILayout.Label(
                "This material is controlled by the SkyboxSettings ScriptableObject.", titleStyle);
            GUILayout.Label("It is HIGHLY RECOMMENDED to make changes there.", titleStyle);
            GUILayout.Label("", GUI.skin.horizontalSlider);
            
            GUILayout.Space(20);
            base.OnGUI(materialEditor, properties);
        }
    }
}