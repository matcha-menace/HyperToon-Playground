// Shader: Skybox.shader
// Description: Procedural Skybox shader optimized for Meta Quest 3.
// Revision: v12
// Last change: Optimized v11 logic.
// Current change: Critical Fix. Replaced all non-breaking space (U+00A0) characters with standard spaces (ASCII 32) to fix VSC warnings and Unity parser error "unexpected $end".
Shader "Evets/Skybox_edited"
{
    Properties
    {
        [NoScaleOffset] _SunZenithGrad ("Sun-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _ViewZenithGrad ("View-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _SunViewGrad ("Sun-View gradient", 2D) = "white" {}
        
        [Header(Global Settings)]
        _SkyExposure ("Sky Exposure", Range(0, 4)) = 1.0

        [Header(Sun Settings)]
        _SunRadius ("Sun Radius (for Eclipses)", Range(0.001, 0.2)) = 0.05
        _SunSize ("Sun Size", Range(0, 0.2)) = 0.04
        _SunHaze ("Sun Haze", Range(0, 1)) = 0.1
        _SunIntensity ("Sun Intensity", Range(1, 25)) = 15
        [MaterialToggle] _SunColorCustomize ("Sun color customize", Float) = 0
        [NoScaleOffset] _SunColorGrad ("Sun color gradient", 2D) = "white" {}
        
        // Moon
        [NoScaleOffset] _MoonCubeMap ("Moon cube map", Cube) = "black" {}
        [MaterialToggle] _MoonOn("Moon On", Float) = 1
        _MoonRadius ("Moon radius", Range(0, 1)) = 0.05
        _MoonEdgeStrength ("Moon edge strength", Range(0.01, 1)) = 0.5
        _MoonExposure ("Moon exposure", Range(-16, 0)) = 0
        _MoonDarkside ("Moon darkside", Range(0, 1)) = 0.5
        // Moon 1
        [NoScaleOffset] _MoonCubeMap1 ("Moon cube map", Cube) = "black" {}
        [MaterialToggle] _MoonOn1("Moon On", Float) = 1
        _MoonRadius1 ("Moon radius", Range(0, 1)) = 0.05
        _MoonEdgeStrength1 ("Moon edge strength", Range(0.01, 1)) = 0.5
        _MoonExposure1 ("Moon exposure", Range(-16, 0)) = 0
        _MoonDarkside1 ("Moon darkside", Range(0, 1)) = 0.5
        // Moon 2
        [NoScaleOffset] _MoonCubeMap2 ("Moon cube map", Cube) = "black" {}
        [MaterialToggle] _MoonOn2("Moon On", Float) = 1
        _MoonRadius2 ("Moon radius", Range(0, 1)) = 0.05
        _MoonEdgeStrength2 ("Moon edge strength", Range(0.01, 1)) = 0.5
        _MoonExposure2 ("Moon exposure", Range(-16, 0)) = 0
        _MoonDarkside2 ("Moon darkside", Range(0, 1)) = 0.5
        // Clouds
        [NoScaleOffset] _CloudGrad ("Cloud color gradient", 2D) = "white" {}
        [NoScaleOffset] _CloudCubeMap ("Cloud cube map", Cube) = "black" {}
        [MaterialToggle] _CloudOn("Cloud On", Float) = 1
        _CloudAlpha ("Cloud alpha", Range(0.2, 1)) = 0.6
        _CloudSpeed ("Cloud speed", Float) = 0.001
        [NoScaleOffset] _CloudBackCubeMap ("Cloud cube map", Cube) = "black" {}
        _Cloudiness ("Cloudiness", Range(0, 1)) = 0.5
        // Stars
        [NoScaleOffset] _StarCubeMap ("Star cube map", Cube) = "black" {}
        _StarExposure ("Star exposure", Range(-16, 16)) = 0
        _StarPower ("Star power", Range(1, 5)) = 1
        _StarLatitude ("Star latitude", Range(-90, 90)) = 0
        _StarSpeed ("Star speed", Float) = 0.001
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_SunZenithGrad); SAMPLER(sampler_SunZenithGrad);
            TEXTURE2D(_ViewZenithGrad); SAMPLER(sampler_ViewZenithGrad);
            TEXTURE2D(_SunViewGrad); SAMPLER(sampler_SunViewGrad);
            TEXTURE2D(_CloudGrad); SAMPLER(sampler_CloudGrad);
            TEXTURE2D(_SunColorGrad); SAMPLER(sampler_SunColorGrad);
            TEXTURECUBE(_MoonCubeMap); SAMPLER(sampler_MoonCubeMap);
            TEXTURECUBE(_MoonCubeMap1); SAMPLER(sampler_MoonCubeMap1);
            TEXTURECUBE(_MoonCubeMap2); SAMPLER(sampler_MoonCubeMap2);
            TEXTURECUBE(_StarCubeMap); SAMPLER(sampler_StarCubeMap);
            TEXTURECUBE(_CloudCubeMap); SAMPLER(sampler_CloudCubeMap);
            TEXTURECUBE(_CloudBackCubeMap); SAMPLER(sampler_CloudBackCubeMap);

            struct Attributes { float4 posOS : POSITION; };
            struct Varyings { float4 posCS : SV_POSITION; float3 viewDirWS : TEXCOORD0; };

            Varyings Vertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.posOS.xyz);
                o.posCS = vertexInput.positionCS;
                o.viewDirWS = vertexInput.positionWS;
                return o;
            }
            
            float _SkyExposure;
            float3 _SunDir;
            float _SunIntensity;
            float _SunRadius;
            float _SunSize;
            float _SunHaze;
            float _SunColorCustomize;
            
            float3 _MoonDir; float _MoonOn; float _MoonRadius; float _MoonEdgeStrength; float _MoonExposure; float _MoonDarkside; float4x4 _MoonSpaceMatrix;
            float3 _MoonDir1; float _MoonOn1; float _MoonRadius1; float _MoonEdgeStrength1; float _MoonExposure1; float _MoonDarkside1; float4x4 _MoonSpaceMatrix1;
            float3 _MoonDir2; float _MoonOn2; float _MoonRadius2; float _MoonEdgeStrength2; float _MoonExposure2; float _MoonDarkside2; float4x4 _MoonSpaceMatrix2;
            float _StarExposure, _StarPower; float _StarLatitude, _StarSpeed;
            float _CloudSpeed, _CloudOn, _CloudAlpha, _Cloudiness;
            
            // --- HELPER FUNCTIONS ---
            float SphereIntersect(float3 rayDir, float3 spherePos, float radius)
            {
                float3 oc = -spherePos;
                float b = dot(oc, rayDir);
                float c = dot(oc, oc) - radius * radius;
                float h = b * b - c;
                if(h < 0.0) return -1.0;
                h = sqrt(h);
                return -b - h;
            }

            float3 GetMoonTexture(float3 normal, int moonIndex)
            {
                float4x4 m;
                if (moonIndex == 0) m = _MoonSpaceMatrix;
                else if (moonIndex == 1) m = _MoonSpaceMatrix1;
                else m = _MoonSpaceMatrix2;
                
                float3 uvw = mul(m, float4(normal,0)).xyz;
                float3x3 correctionMatrix = float3x3(0, -0.2588190451, -0.9659258263, 0.08715574275, 0.9622501869, -0.2578341605, 0.9961946981, -0.08418598283, 0.02255756611);
                uvw = mul(correctionMatrix, uvw);
                
                if (moonIndex == 0) return SAMPLE_TEXTURECUBE(_MoonCubeMap, sampler_MoonCubeMap, uvw).rgb;
                if (moonIndex == 1) return SAMPLE_TEXTURECUBE(_MoonCubeMap1, sampler_MoonCubeMap1, uvw).rgb;
                return SAMPLE_TEXTURECUBE(_MoonCubeMap2, sampler_MoonCubeMap2, uvw).rgb;
            }
            
            float3x3 AngleAxis3x3(float angle, float3 axis)
            {
                float c, s;
                sincos(angle, s, c);
                float t = 1 - c;
                float x = axis.x; float y = axis.y; float z = axis.z;
                return float3x3( t * x * x + c, t * x * y - s * z, t * x * z + s * y, t * x * y + s * z, t * y * y + c, t * y * z - s * x, t * x * z - s * y, t * y * z + s * x, t * z * z + c );
            }

            // --- OPTIMIZATION START (v11) ---
            // Dedicated cloud UV function, removes tilt calculation (latitude 90 -> tilt 0)
            float3 GetCloudUVW(float3 viewDir, float cloudTime)
            {
                // We only need the spin, as tilt is 0
                float spin = (0.75 - cloudTime) * 2 * PI;
                float3x3 spinRotation = AngleAxis3x3(spin, float3(0, 1, 0));
                return mul(spinRotation, viewDir);
            }
            // --- OPTIMIZATION END (v11) ---

            float3 GetStarUVW(float3 viewDir, float latitude, float localSiderealTime)
            {
                float tilt = PI * (latitude - 90) / 180;
                float3x3 tiltRotation = AngleAxis3x3(tilt, float3(1,0,0));
                float spin = (0.75-localSiderealTime) * 2 * PI;
                float3x3 spinRotation = AngleAxis3x3(spin, float3(0, 1, 0));
                float3x3 fullRotation = mul(spinRotation, tiltRotation);
                return mul(fullRotation, viewDir);
            }

            float4 Fragment(Varyings v) : SV_TARGET
            {
                float3 viewDir = normalize(v.viewDirWS);
                float sunViewDot = dot(_SunDir, viewDir);
                float sunZenithDot = _SunDir.y;
                float viewZenithDot = viewDir.y;
                float sunMoonDot = dot(_SunDir, _MoonDir);
                float sunViewDot1 = (sunViewDot + 1) * 0.5;
                float sunZenithDot1 = (sunZenithDot + 1) * 0.5;

                float3 sunZenithColor = SAMPLE_TEXTURE2D(_SunZenithGrad, sampler_SunZenithGrad, float2(sunZenithDot1, 0.5)).rgb;
                float3 viewZenithColor = SAMPLE_TEXTURE2D(_ViewZenithGrad, sampler_ViewZenithGrad, float2(sunZenithDot1, 0.5)).rgb;
                
                // --- OPTIMIZATION START (v11) ---
                // Replaced pow(x, 4)
                float vzMask_sat = saturate(1.0 - viewZenithDot);
                float vzMask_2 = vzMask_sat * vzMask_sat;
                float vzMask = vzMask_2 * vzMask_2;
                // --- OPTIMIZATION END (v11) ---

                float3 sunViewColor = SAMPLE_TEXTURE2D(_SunViewGrad, sampler_SunViewGrad, float2(sunZenithDot1, 0.5)).rgb;

                // --- OPTIMIZATION START (v11) ---
                // Replaced pow(x, 6)
                // Calculate sunViewDot_sat once and reuse for pow(24) later
                float sunViewDot_sat = saturate(sunViewDot);
                float svMask_2 = sunViewDot_sat * sunViewDot_sat;
                float svMask_3 = svMask_2 * sunViewDot_sat;
                float svMask = svMask_3 * svMask_3;
                // --- OPTIMIZATION END (v11) ---
                
                // --- MODIFICATION START (v10 fix) ---
                float dist = distance(viewDir, _SunDir);
                float sunDisc = 1.0 - smoothstep(_SunSize - 0.005, _SunSize, dist);
                
                float hazeDist = max(0.0, dist - _SunSize);
                float hazeRadius = _SunSize + _SunHaze;
                float hazeFalloff = 1.0 - saturate(hazeDist / hazeRadius);
                
                // --- OPTIMIZATION START (v11) ---
                // Replaced pow(x, 4)
                float hazeFalloff_2 = hazeFalloff * hazeFalloff;
                float sunHaze = hazeFalloff_2 * hazeFalloff_2;
                // --- OPTIMIZATION END (v11) ---

                float3 sunOverlayColor = _SunColorCustomize == 0 ? _MainLightColor.rgb : SAMPLE_TEXTURE2D(_SunColorGrad, sampler_SunColorGrad, float2(sunZenithDot1, 0.5)).rgb;
                float3 sunColor = sunOverlayColor * (sunDisc + sunHaze) * _SunIntensity;
                // --- MODIFICATION END (v10 fix) ---
                
                
                // --- OPTIMIZATION START (v11) ---
                // Refactored moon logic for dynamic branching.
                // Calculations are now SKIPPED if _MoonOn == 0.
                
                float3 moonColor = float3(0.0, 0.0, 0.0);
                float3 moonColor1 = float3(0.0, 0.0, 0.0);
                float3 moonColor2 = float3(0.0, 0.0, 0.0);
                
                float moonMask = 0.0;
                float moonMask1 = 0.0;
                float moonMask2 = 0.0;

                // Moon 0
                if (_MoonOn > 0.5)
                {
                    float moonIntersect = SphereIntersect(viewDir, _MoonDir, _MoonRadius);
                    moonMask = moonIntersect * 2.0 > -1.0 ? 1.0 : 0.0;
                    if (moonMask > 0.0)
                    {
                        float3 moonNormal = normalize(_MoonDir - viewDir * moonIntersect); 
                        float moonNdotL = saturate(dot(moonNormal, -_SunDir)); 
                        float3 moonTexture = GetMoonTexture(moonNormal, 0); 
                        moonColor = moonNdotL * exp2(_MoonExposure); 
                        moonColor = smoothstep(0.0, _MoonEdgeStrength, moonColor) * moonTexture; 
                        moonColor += saturate(_MoonDarkside * moonTexture);
                    }
                }

                // Moon 1
                if (_MoonOn1 > 0.5)
                {
                    float moonIntersect1 = SphereIntersect(viewDir, _MoonDir1, _MoonRadius1);
                    moonMask1 = moonIntersect1 * 2.0 > -1.0 ? 1.0 - moonMask : 0.0; // Sequential occlusion
                    if (moonMask1 > 0.0)
                    {
                        float3 moonNormal1 = normalize(_MoonDir1 - viewDir * moonIntersect1); 
                        float moonNdotL1 = saturate(dot(moonNormal1, -_SunDir)); 
                        float3 moonTexture1 = GetMoonTexture(moonNormal1, 1); 
                        moonColor1 = moonMask1 * moonNdotL1 * exp2(_MoonExposure1); // Original code multiplies by mask
                        moonColor1 = smoothstep(0.0, _MoonEdgeStrength1, moonColor1) * moonTexture1; 
                        moonColor1 += moonMask1 * saturate(_MoonDarkside1 * moonTexture1);
                    }
                }

                // Moon 2
                if (_MoonOn2 > 0.5)
                {
                    float moonIntersect2 = SphereIntersect(viewDir, _MoonDir2, _MoonRadius2);
                    moonMask2 = moonIntersect2 * 2.0 > -1.0 ? 1.0 - moonMask - moonMask1 : 0.0; // Sequential occlusion
                    if (moonMask2 > 0.0)
                    {
                        float3 moonNormal2 = normalize(_MoonDir2 - viewDir * moonIntersect2); 
                        float moonNdotL2 = saturate(dot(moonNormal2, -_SunDir)); 
                        float3 moonTexture2 = GetMoonTexture(moonNormal2, 2); 
                        moonColor2 = moonMask2 * moonNdotL2 * exp2(_MoonExposure2); 
                        moonColor2 = smoothstep(0.0, _MoonEdgeStrength2, moonColor2) * moonTexture2; 
                        moonColor2 += moonMask2 * saturate(_MoonDarkside2 * moonTexture2);
                    }
                }
                
                // This mask is used to block sun/stars
                float allMoonMask = moonMask + moonMask1 + moonMask2;
                // --- OPTIMIZATION END (v11) ---

                // --- OPTIMIZATION START (v11) ---
                // Replaced GetStarUVW with GetCloudUVW
                float cloudTime = _Time.y * _CloudSpeed % 1.0;
                float3 cloudUVW = GetCloudUVW(viewDir, cloudTime);
                float3 cloudColor = SAMPLE_TEXTURECUBE_BIAS(_CloudCubeMap, sampler_CloudCubeMap, cloudUVW, -1).rgb; 
                cloudColor *= _CloudOn * _CloudAlpha; 
                
                float cloudBackTime = _Time.y * (_CloudSpeed / 4.0) % 1.0;
                float3 cloudBackUVW = GetCloudUVW(viewDir, cloudBackTime);
                float3 cloudBackColor = SAMPLE_TEXTURECUBE_BIAS(_CloudBackCubeMap, sampler_CloudBackCubeMap, cloudBackUVW, -1).rgb; 
                cloudBackColor *= _Cloudiness * _CloudAlpha * _CloudOn;
                // --- OPTIMIZATION END (v11) ---
                
                float3 cloudBlocking = 1 - smoothstep(0.01, .1, cloudColor + cloudBackColor); 
                float3 skyColor = sunZenithColor + vzMask * viewZenithColor + svMask * cloudBlocking * (sunViewColor * lerp(1, 1 - allMoonMask, 1)); 
                skyColor *= lerp(1, 0.8, _Cloudiness * _CloudOn);
                
                // --- OPTIMIZATION START (v11) ---
                // Replaced GetStarUVW time calculation
                float starTime = _Time.y * _StarSpeed % 1.0;
                float3 starUVW = GetStarUVW(viewDir, _StarLatitude, starTime); 
                // --- OPTIMIZATION END (v11) ---
                
                float3 starColor = SAMPLE_TEXTURECUBE_BIAS(_StarCubeMap, sampler_StarCubeMap, starUVW, -1).rgb; 
                
                // --- OPTIMIZATION START (v11) ---
                // Kept pow() here because _StarPower is a dynamic uniform
                starColor = pow(abs(starColor), _StarPower); 
                // --- OPTIMIZATION END (v11) ---

                float starStrength = (1 - sunViewDot1) * saturate(-sunZenithDot); 
                starColor *= (1 - allMoonMask) * exp2(_StarExposure) * starStrength;
                
                sunColor *= 1 - allMoonMask;
                
                // Final composition
                sunColor = sunColor * cloudBlocking;
                moonColor = moonColor * cloudBlocking; 
                moonColor1 = moonColor1 * cloudBlocking; 
                moonColor2 = moonColor2 * cloudBlocking; 
                starColor = starColor * cloudBlocking;
                
                float3 cloudRawColor = SAMPLE_TEXTURE2D(_CloudGrad, sampler_CloudGrad, float2(sunZenithDot1, 0.5)).rgb; 
                float3 cloudColoring = (1 - starStrength) * cloudRawColor; 
                cloudColor *= cloudColoring; 
                cloudBackColor *= cloudColoring; 
                float3 frontCloudBlocking = 1 - smoothstep(0.01, .1, cloudColor); 
                
                // --- OPTIMIZATION START (v11) ---
                // Replaced pow(x, 24)
                // We reuse svMask_2 from the pow(6) optimization
                float sv_4 = svMask_2 * svMask_2;
                float sv_8 = sv_4 * sv_4;
                float sv_16 = sv_8 * sv_8;
                float sv_24 = sv_16 * sv_8; // 5 muls total for pow(24)
                cloudBackColor *= sv_24 * frontCloudBlocking + cloudBackColor;
                // --- OPTIMIZATION END (v11) ---

                float3 col = skyColor + sunColor + cloudBackColor + cloudColor + starColor + moonColor + moonColor1 + moonColor2;
                
                col *= _SkyExposure;
                
                return float4(col, 1);
            }
            ENDHLSL
        }
    }
    CustomEditor "Evets.CustomSkyboxShaderGUI"
    Fallback "Skybox/Procedural"
}