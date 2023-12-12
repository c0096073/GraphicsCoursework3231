Shader "Custom/WindSwayShader"
{
    Properties
    {
        _MainTex("Grass Texture", 2D) = "white" { }
        _Color("Color", Color) = (1, 1, 1, 1)
        _WindSpeed("Wind Speed", Range(0, 1)) = 0.5
        _WaveFrequency("Wave Frequency", Range(0, 5)) = 1.0
        _WaveAmplitude("Wave Amplitude", Range(0, 1)) = 0.1
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _WindMultiplier("Wind Multiplier", Range(0, 5)) = 1.0
        _TopSway("Top Sway", Range(0, 5)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        //disable backface culling
        Cull Off

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

       
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _Color;
        float _WindSpeed;
        float _WaveFrequency;
        float _WaveAmplitude;
        float _Cutoff;
        float _WindMultiplier;
        float _TopSway;

        UNITY_INSTANCING_BUFFER_START(Props)
           
        UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void vert(inout appdata_full v)
        {
            //calculate the sway based on the distance from the base
            float baseDistance = length(v.vertex.xz);
            float normalizedDistance = baseDistance / (_TopSway + 0.1); //add a small value to avoid divide by 0

            //simulate wind using sin wave
            float wave = sin(_WaveFrequency * _Time.y + normalizedDistance * _WaveFrequency);
            float windEffect = _WindSpeed * _WaveAmplitude * wave * _WindMultiplier;

            v.vertex.y += windEffect;  //apply wind  to vertex position
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            //sample the texture
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            //alpha cutoff
            clip(c.a - _Cutoff);

            o.Albedo = c.rgb;
        }
        ENDCG
    }

    Fallback "Diffuse"
}
