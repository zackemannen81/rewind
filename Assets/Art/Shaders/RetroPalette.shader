Shader "Art/RetroPaletteLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.105, 0.121, 0.165, 1)
        _AccentColor ("Accent Color", Color) = (0.996, 0.176, 0.584, 1)
        _AccentMask ("Accent Mask (RGB)", 2D) = "white" {}
        _AccentIntensity ("Accent Intensity", Range(0, 5)) = 1
        _GlitchStrength ("Glitch Rim Strength", Range(0, 1)) = 0.1
        _Smoothness ("Smoothness", Range(0, 1)) = 0.1
        _Metallic ("Metallic", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #pragma multi_compile _ RETRO_ALERT RETRO_LOOPEND

        sampler2D _AccentMask;
        half _AccentIntensity;
        half _GlitchStrength;
        fixed4 _BaseColor;
        fixed4 _AccentColor;
        half _Smoothness;
        half _Metallic;

        struct Input
        {
            float2 uv_AccentMask;
            float3 viewDir;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseCol = _BaseColor;
            #ifdef RETRO_ALERT
            baseCol.rgb = lerp(baseCol.rgb, fixed3(0.4, 0.05, 0.05), 0.35);
            #endif

            #ifdef RETRO_LOOPEND
            baseCol.rgb = lerp(baseCol.rgb, fixed3(0.3, 0.3, 0.3), 0.6);
            #endif

            fixed4 accentMask = tex2D(_AccentMask, IN.uv_AccentMask);
            fixed3 accentContribution = _AccentColor.rgb * accentMask.rgb * _AccentIntensity;
            fixed rim = saturate(1.0 - dot(normalize(IN.viewDir), o.Normal));
            fixed3 glitch = accentContribution * rim * _GlitchStrength;

            fixed3 finalColor = baseCol.rgb + accentContribution + glitch;

            o.Albedo = finalColor;
            o.Emission = accentContribution + glitch;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Standard"
}
