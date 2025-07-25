Shader "Evets/Skybox"
{
    Properties
    {
        [NoScaleOffset] _SunZenithGrad ("Sun-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _ViewZenithGrad ("View-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _SunViewGrad ("Sun-View gradient", 2D) = "white" {}
        // Sun
        _SunRadius ("Sun radius", Range(0, 1)) = 0.05
        _SunIntensity ("Sun intensity", Range(1, 3)) = 1
        [MaterialToggle] _SunColorCustomize ("Sun color customize", Float) = 0
        [NoScaleOffset] _SunColorGrad ("Sun color gradient", 2D) = "white" {}
        [MaterialToggle] _SunTextureOn ("Sun texture", Float) = 0
        [NoScaleOffset] _SunCubeMap ("Sun cube map", Cube) = "black" {}
        _SunTextureStrength ("Sun texture strength", Range(0, 1)) = 0.5
        [MaterialToggle] _SynthSun ("Synth sun", Float) = 0
        _SynthSunLines ("Synth sun lines", Range(0, 1)) = 0.5
        _SynthSunBottom ("Synth sun bottom", Range(0, 1)) = 0.5
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

            TEXTURE2D(_SunZenithGrad);
            SAMPLER(sampler_SunZenithGrad);
            TEXTURE2D(_ViewZenithGrad);
            SAMPLER(sampler_ViewZenithGrad);
            TEXTURE2D(_SunViewGrad);
            SAMPLER(sampler_SunViewGrad);
            TEXTURE2D(_CloudGrad);
            SAMPLER(sampler_CloudGrad);
            TEXTURE2D(_SunColorGrad);
            SAMPLER(sampler_SunColorGrad);

            TEXTURECUBE(_SunCubeMap);
            SAMPLER(sampler_SunCubeMap);
            TEXTURECUBE(_MoonCubeMap);
            SAMPLER(sampler_MoonCubeMap);
            TEXTURECUBE(_MoonCubeMap1);
            SAMPLER(sampler_MoonCubeMap1);
            TEXTURECUBE(_MoonCubeMap2);
            SAMPLER(sampler_MoonCubeMap2);
            TEXTURECUBE(_StarCubeMap);
            SAMPLER(sampler_StarCubeMap);
            TEXTURECUBE(_CloudCubeMap);
            SAMPLER(sampler_CloudCubeMap);
            TEXTURECUBE(_CloudBackCubeMap);
            SAMPLER(sampler_CloudBackCubeMap);

            struct Attributes
            {
                float4 posOS    : POSITION;
                float2 uv       : TEXCOORD1;
            };

            struct Varyings
            {
                float4 posCS        : SV_POSITION;
                float3 viewDirWS    : TEXCOORD0;
                float2 uv           : TEXCOORD1;
            };

            Varyings Vertex(Attributes v)
            {
                Varyings o = (Varyings)0;
    
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.posOS.xyz);
    
                o.posCS = vertexInput.positionCS;
                o.viewDirWS = vertexInput.positionWS;
                o.uv = v.uv;

                return o;
            }

            float3 _SunDir;
            float _SunIntensity;
            float _SunRadius;
            float _SynthSun, _SynthSunLines, _SynthSunBottom;
            float _SunTextureOn, _SunTextureStrength;
            float _SunColorCustomize;
            
            float3 _MoonDir;
            float _MoonOn;
            float _MoonRadius;
            float _MoonEdgeStrength;
            float _MoonExposure;
            float _MoonDarkside;
            float4x4 _MoonSpaceMatrix;

            float3 _MoonDir1;
            float _MoonOn1;
            float _MoonRadius1;
            float _MoonEdgeStrength1;
            float _MoonExposure1;
            float _MoonDarkside1;
            float4x4 _MoonSpaceMatrix1;

            float3 _MoonDir2;
            float _MoonOn2;
            float _MoonRadius2;
            float _MoonEdgeStrength2;
            float _MoonExposure2;
            float _MoonDarkside2;
            float4x4 _MoonSpaceMatrix2;

            float _StarExposure, _StarPower;
            float _StarLatitude, _StarSpeed;

            float _CloudSpeed, _CloudOn, _CloudAlpha, _Cloudiness;

            float GetSunMask(float sunViewDot, float sunRadius)
            {
                float stepRadius = 1 - sunRadius * sunRadius;
                return step(stepRadius, sunViewDot);
            }

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
                if (moonIndex == 0)
                    m = _MoonSpaceMatrix;
                else if (moonIndex == 1)
                    m = _MoonSpaceMatrix1;
                else
                    m = _MoonSpaceMatrix2;
                
                float3 uvw = mul(m, float4(normal,0)).xyz;
                float3x3 correctionMatrix = float3x3(0, -0.2588190451, -0.9659258263,
                    0.08715574275, 0.9622501869, -0.2578341605,
                    0.9961946981, -0.08418598283, 0.02255756611);
                uvw = mul(correctionMatrix, uvw);
                
                if (moonIndex == 0) return SAMPLE_TEXTURECUBE(_MoonCubeMap, sampler_MoonCubeMap, uvw).rgb;
                if (moonIndex == 1) return SAMPLE_TEXTURECUBE(_MoonCubeMap1, sampler_MoonCubeMap1, uvw).rgb;
                return SAMPLE_TEXTURECUBE(_MoonCubeMap2, sampler_MoonCubeMap2, uvw).rgb;
            }

            float3 GetSunTexture(float3 normal)
            {
                float3 uvw = mul(float3x3(0, -0.2588190451, -0.9659258263,
                    0.08715574275, 0.9622501869, -0.2578341605,
                    0.9961946981, -0.08418598283, 0.02255756611), normal);
                return SAMPLE_TEXTURECUBE(_SunCubeMap, sampler_SunCubeMap, uvw).rgb;
            }

            float3x3 AngleAxis3x3(float angle, float3 axis)
            {
                float c, s;
                sincos(angle, s, c);

                float t = 1 - c;
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;

                return float3x3(
                    t * x * x + c, t * x * y - s * z, t * x * z + s * y,
                    t * x * y + s * z, t * y * y + c, t * y * z - s * x,
                    t * x * z - s * y, t * y * z + s * x, t * z * z + c
                    );
            }

            float3 GetStarUVW(float3 viewDir, float latitude, float localSiderealTime)
            {
                // tilt = 0 at the north pole, where latitude = 90 degrees
                float tilt = PI * (latitude - 90) / 180;
                float3x3 tiltRotation = AngleAxis3x3(tilt, float3(1,0,0));

                // 0.75 is a texture offset for lST = 0 equals noon
                float spin = (0.75-localSiderealTime) * 2 * PI;
                float3x3 spinRotation = AngleAxis3x3(spin, float3(0, 1, 0));
                
                // The order of rotation is important
                float3x3 fullRotation = mul(spinRotation, tiltRotation);

                return mul(fullRotation, viewDir);
            }

            float4 Fragment(Varyings v) : SV_TARGET
            {
                float3 viewDir = normalize(v.viewDirWS);

                // angles
                float sunViewDot = dot(_SunDir, viewDir);
                float sunZenithDot = _SunDir.y;
                float viewZenithDot = viewDir.y;
                float sunMoonDot = dot(_SunDir, _MoonDir);

                float sunViewDot1 = (sunViewDot + 1) * 0.5;
                float sunZenithDot1 = (sunZenithDot + 1) * 0.5;

                // sky colors
                float3 sunZenithColor = SAMPLE_TEXTURE2D(_SunZenithGrad, sampler_SunZenithGrad, float2(sunZenithDot1, 0.5)).rgb;
                float3 viewZenithColor = SAMPLE_TEXTURE2D(_ViewZenithGrad, sampler_ViewZenithGrad, float2(sunZenithDot1, 0.5)).rgb;
                float vzMask = pow(saturate(1.0 - viewZenithDot), 4);
                float3 sunViewColor = SAMPLE_TEXTURE2D(_SunViewGrad, sampler_SunViewGrad, float2(sunZenithDot1, 0.5)).rgb;
                float svMask = pow(saturate(sunViewDot), 6);
                
                // The sun
                float sunMask = GetSunMask(sunViewDot, _SunRadius);
                float bottom = step(1 + lerp(-.3, .3, _SynthSunBottom) - sunZenithDot, 1 - v.uv.g) * smoothstep(.7, .75, 1 - sunZenithDot);
                float smoothBottom = smoothstep(1 - sunZenithDot, 2 - sunZenithDot, 1 - v.uv.g);
                float lines = lerp(_SynthSunLines, _SynthSunLines * 5, smoothBottom);
                sunMask = _SynthSun < 1 ? sunMask : 
                    saturate(
                    step(.5, tan(v.uv.g * lines + 1))
                    ) * bottom * sunMask + (1 - bottom) * sunMask;
                float sunIntersect = SphereIntersect(viewDir, _SunDir, _SunRadius);
                sunMask = sunIntersect > -1 ? sunMask : 0;
                float3 sunNormal = normalize(_SunDir - viewDir * sunIntersect);
                float3 sunTexture = _SunTextureOn == 0 ? 1 : GetSunTexture(sunNormal) * _SunTextureStrength + 1 - _SunTextureStrength;
                float3 sunOverlayColor = _SunColorCustomize == 0 ? _MainLightColor.rgb : SAMPLE_TEXTURE2D(_SunColorGrad, sampler_SunColorGrad, float2(sunZenithDot1, 0.5)).rgb;
                float3 sunColor = sunOverlayColor * sunMask * sunTexture;

                // The moon
                float moonIntersect = SphereIntersect(viewDir, _MoonDir, _MoonRadius);
                float moonMask = moonIntersect > -1 ? 1 : 0;
                float3 moonNormal = normalize(_MoonDir - viewDir * moonIntersect);
                float moonNdotL = saturate(dot(moonNormal, -_SunDir));
                float3 moonTexture = GetMoonTexture(moonNormal, 0);
                float3 moonColor = moonMask * moonNdotL * exp2(_MoonExposure);
                moonColor = smoothstep(0, _MoonEdgeStrength, moonColor) * moonTexture;
                moonColor += moonMask * saturate(_MoonDarkside * moonTexture);
                moonColor *= _MoonOn;

                // The moon1
                float moonIntersect1 = SphereIntersect(viewDir, _MoonDir1, _MoonRadius1);
                float moonMask1 = moonIntersect1 > -1 ? 1 - moonMask : 0;
                float3 moonNormal1 = normalize(_MoonDir1 - viewDir * moonIntersect1);
                float moonNdotL1 = saturate(dot(moonNormal1, -_SunDir));
                float3 moonTexture1 = GetMoonTexture(moonNormal1, 1);
                float3 moonColor1 = moonMask1 * moonNdotL1 * exp2(_MoonExposure1);
                moonColor1 = smoothstep(0, _MoonEdgeStrength1, moonColor1) * moonTexture1;
                moonColor1 += moonMask1 * saturate(_MoonDarkside1 * moonTexture1);
                moonColor1 *= _MoonOn1;

                // The moon2
                float moonIntersect2 = SphereIntersect(viewDir, _MoonDir2, _MoonRadius2);
                float moonMask2 = moonIntersect2 > -1 ? 1 - moonMask - moonMask1 : 0;
                float3 moonNormal2 = normalize(_MoonDir2 - viewDir * moonIntersect2);
                float moonNdotL2 = saturate(dot(moonNormal2, -_SunDir));
                float3 moonTexture2 = GetMoonTexture(moonNormal2, 2);
                float3 moonColor2 = moonMask2 * moonNdotL2 * exp2(_MoonExposure2);
                moonColor2 = smoothstep(0, _MoonEdgeStrength2, moonColor2) * moonTexture2;
                moonColor2 += moonMask2 * saturate(_MoonDarkside2 * moonTexture2);
                moonColor2 *= _MoonOn2;

                float allMoonMask = moonMask * _MoonOn + moonMask1 * _MoonOn1 + moonMask2 * _MoonOn2;

                // clouds
                float3 cloudUVW = GetStarUVW(viewDir, 90, _Time.y * _CloudSpeed % 1);
                float3 cloudColor = SAMPLE_TEXTURECUBE_BIAS(_CloudCubeMap, sampler_CloudCubeMap, cloudUVW, -1).rgb;
                cloudColor *= _CloudOn * _CloudAlpha;
                // clouds back
                float3 cloudBackUVW = GetStarUVW(viewDir, 90, _Time.y * (_CloudSpeed / 4) % 1);
                float3 cloudBackColor = SAMPLE_TEXTURECUBE_BIAS(_CloudBackCubeMap, sampler_CloudBackCubeMap, cloudBackUVW, -1).rgb;
                cloudBackColor *= _Cloudiness * _CloudAlpha * _CloudOn;

                // cloud blocking
                float3 cloudBlocking = 1 - smoothstep(0.01, .1, cloudColor + cloudBackColor);
                // calculate sky color (with cloud)
                float3 skyColor = sunZenithColor + vzMask * viewZenithColor + svMask * cloudBlocking * (sunViewColor * lerp(1, 1 - allMoonMask, 1)); // moon masking sun halo
                skyColor *= lerp(1, 0.8, _Cloudiness * _CloudOn); // darken color for when cloudy

                // stars
                float3 starUVW = GetStarUVW(viewDir, _StarLatitude, _Time.y * _StarSpeed % 1);
                float3 starColor = SAMPLE_TEXTURECUBE_BIAS(_StarCubeMap, sampler_StarCubeMap, starUVW, -1).rgb;
                starColor = pow(abs(starColor), _StarPower);
                float starStrength = (1 - sunViewDot1) * saturate(-sunZenithDot);
                starColor *= (1 - sunMask) * (1 - allMoonMask) * exp2(_StarExposure) * starStrength;
                
                // solar eclipse
                sunColor *= 1 - allMoonMask;
                float sunMoonDot1 = dot(_SunDir, _MoonDir1);
                float sunMoonDot2 = dot(_SunDir, _MoonDir2);
                float solarEclipse01 = smoothstep(1 - _SunRadius * (_MoonRadius + 0.4), 1.0, sunMoonDot);
                float solarEclipse02 = smoothstep(1 - _SunRadius * (_MoonRadius1 + 0.4), 1.0, sunMoonDot1);
                float solarEclipse03 = smoothstep(1 - _SunRadius * (_MoonRadius2 + 0.4), 1.0, sunMoonDot2);
                skyColor *= lerp(1, 0.3, solarEclipse01); // darken color for when eclipsed
                skyColor *= lerp(1, 0.3, solarEclipse02);
                skyColor *= lerp(1, 0.3, solarEclipse03);
                sunColor *= (1 - moonMask) * lerp(1, 4, solarEclipse01); // increase sun brightness for when eclipsed;
                sunColor *= (1 - moonMask1) * lerp(1, 4, solarEclipse02);
                sunColor *= (1 - moonMask2) * lerp(1, 4, solarEclipse03);
                sunColor *= _SunIntensity;

                // lunar eclipse
                float lunarEclipseMask = 1 - step(1 - _SunRadius * _SunRadius, -sunViewDot);
                float lunarEclipse01 = smoothstep(1 - _SunRadius * _SunRadius * 0.05, 1.0, -sunMoonDot);
                moonColor *= lerp(lunarEclipseMask, float3(0.4, 0.05, 0), lunarEclipse01);
                
                float lunarEclipseMask1 = 1 - step(1 - _SunRadius * _SunRadius * 0.5, -sunViewDot);
                float lunarEclipse011 = smoothstep(1 - _SunRadius * _SunRadius * 0.05, 1.0, -sunMoonDot1);
                moonColor1 *= lerp(lunarEclipseMask1, float3(0.4, 0.05, 0), lunarEclipse011);
                
                float lunarEclipseMask2 = 1 - step(1 - _SunRadius * _SunRadius * 0.25, -sunViewDot);
                float lunarEclipse012 = smoothstep(1 - _SunRadius * _SunRadius * 0.05, 1.0, -sunMoonDot2);
                moonColor2 *= lerp(lunarEclipseMask2, float3(0.4, 0.05, 0), lunarEclipse012);

                // clouds block sun, moon, stars
                sunColor = sunColor * cloudBlocking;
                moonColor = moonColor * cloudBlocking;
                moonColor1 = moonColor1 * cloudBlocking;
                moonColor2 = moonColor2 * cloudBlocking;
                starColor = starColor * cloudBlocking;
                // darken clouds for night
                float3 cloudRawColor = SAMPLE_TEXTURE2D(_CloudGrad, sampler_CloudGrad, float2(sunZenithDot1, 0.5)).rgb;
                float3 cloudColoring = (1 - starStrength) * cloudRawColor;
                cloudColor *= cloudColoring;
                cloudBackColor *= cloudColoring;
                float3 frontCloudBlocking = 1 - smoothstep(0.01, .1, cloudColor);
                cloudBackColor *= pow(saturate(sunViewDot), 24) * frontCloudBlocking + cloudBackColor;
                

                float3 col = skyColor + sunColor + cloudBackColor + cloudColor + starColor + moonColor + moonColor1 + moonColor2;
                // col = allMoonMask;
                
                return float4(col, 1);
            }
            ENDHLSL
        }
    }
    CustomEditor "Evets.CustomSkyboxShaderGUI"
    Fallback "Skybox/Procedural"
}
