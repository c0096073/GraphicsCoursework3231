Shader "Custom/WaterShader"
{
    Properties
    {
        //water properties
        _Color ("Color", Color) = (1,1,1,1)
        _NormalTex1 ("Normal texture 1", 2D) = "bump" {}
        _NormalTex2 ("Normal texture 2", 2D) = "bump" {}
        _NoiseTex ("Noise texture", 2D) = "white" {}
        _Glossiness ("Glossiness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Scale ("Noise scale", Range(0.01, 0.1)) = 0.03
        _Amplitude ("Amplitude", Range(0.01, 0.1)) = 0.015
        _Speed ("Speed", Range(0.01, 0.3)) = 0.15
        _NormalStrength ("Normal Strength", Range(0, 1)) = 0.5
        _SoftFactor("Soft Factor", Range(0.01, 3.0)) = 1.0

        //3D wave movement properties
        _3DWaveSpeed ("3DWave Speed", Range(0.1, 2.0)) = 0.5
        _3DWaveFrequency ("3DWave Frequency", Range(1, 10)) = 3.0
        _3DWaveAmplitude ("3DWave Amplitude", Range(0.1, 2.0)) = 0.2
    }

  
    SubShader
    {
      
        Tags { "RenderType"="Opaque" "ForceNoShadowCasting" = "True"}
        
   

       
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha vertex:vertex

        #pragma target 3.0

        //textures
        sampler2D _NormalTex1;
        sampler2D _NormalTex2;
        sampler2D _NoiseTex;
        sampler2D _CameraDepthTexture;

        //2D to create the look of the water
        float _Scale;
        float _Amplitude;
        float _Speed;
        float _NormalStrength;
        float _SoftFactor;

      //3D to simulate the 3D effect of waves
        float _3DWaveSpeed;
        float _3DWaveFrequency;
        float _3DWaveAmplitude;

        //variables for the shine and darkness of the water as well as the colour
        half _Glossiness;
        half _Metallic;
        fixed4 _Colour;

       
        struct Input
        {
            float2 uv_NormalTex1; 
            float4 screenPos; 
            float eyeDepth; 
        };

  
        void vertex (inout appdata_full v, out Input o)
        {
            float2 NoiseUV = float2((v.texcoord.xy + _Time * _Speed) * _Scale); //calculate UV coordinates for noise texture 
            float NoiseValue = tex2Dlod(_NoiseTex, float4(NoiseUV, 0, 0)).x * _Amplitude; //sample noise exture and apply displacement
            v.vertex.y += NoiseValue; //apply displacement to vertex Y position

            float waveDisplacement = sin((v.vertex.x + v.vertex.z) * _3DWaveFrequency + _Time.y * _3DWaveSpeed) * _3DWaveAmplitude; //apply the 3D wave movement as a sin wave to the vertex y position
            v.vertex.y += waveDisplacement;

         
            UNITY_INITIALIZE_OUTPUT(Input, o);
            COMPUTE_EYEDEPTH(o.eyeDepth);
        }

      
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
           
            fixed4 c = _Colour;

           //decide how "shiny" the water should be and also how dark
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness  = _Glossiness;

            //depth calculation, areas with different depths have different levels of transparency
            float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
            float sceneZ = LinearEyeDepth(rawZ);
            float partZ = IN.eyeDepth;

            //calculate fade based on the difference in depth
            float fade = saturate(_SoftFactor * (sceneZ - partZ));

          
            o.Alpha = fade * 0.5;

            //offset the UV coordinates for normal textures based on time
            float normalUVX = IN.uv_NormalTex1.x + sin(_Time) * 5;
            float normalUVY = IN.uv_NormalTex1.y + sin(_Time) * 5;

            //combine normal textures, apply strength and fade, and set the final normal
            float2 normalUV1 = float2(normalUVX, IN.uv_NormalTex1.y);
            float2 normalUV2 = float2(IN.uv_NormalTex1.x, normalUVY);
            o.Normal = UnpackNormal((tex2D(_NormalTex1, normalUV1) + tex2D(_NormalTex2, normalUV2)) * _NormalStrength * fade);
        }

 
        ENDCG
    }

   
    FallBack "Diffuse"
}
