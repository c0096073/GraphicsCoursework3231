Shader "Custom/TerrainShader"
{
    Properties
    {
        _textureScale("Texture scale", float) = 1
        _specularIntensity("Specular Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        #define MAX_TEXTURES 32

        // Declare shader properties
        float _textureScale;
        float _specularIntensity;
        float minTerrainHeight;
        float maxTerrainHeight;

        float terrainHeights[MAX_TEXTURES];
        UNITY_DECLARE_TEX2DARRAY(terrainTextures);

        int numTextures;

        // Input structure defining data passed to the shader from Unity's rendering pipeline
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // Surface shader function where the main computations happen
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 scaledWorldPos = IN.worldPos / _textureScale;
            float3 worldPosY = IN.worldPos.y;
            float heightValue = saturate((worldPosY - minTerrainHeight) / (maxTerrainHeight - minTerrainHeight));
            int layerIndex = -1;

            // Find the layer based on height
            for (int i = 0; i < numTextures - 1; i++) 
            {
                if (heightValue >= terrainHeights[i] && heightValue <= terrainHeights[i + 1]) 
                {
                    layerIndex = i;
                    break;
                }
            }

            // If the layer is not found, use the last layer
            if (layerIndex == -1) 
            {
                layerIndex = numTextures - 1;
            }

            // Linearly blend between the textures based on the height
            float blendFactor = (heightValue - terrainHeights[layerIndex]) / (terrainHeights[layerIndex + 1] - terrainHeights[layerIndex]);

            // Sample colors from two adjacent textures
            float3 color1 = UNITY_SAMPLE_TEX2DARRAY(terrainTextures, float3(scaledWorldPos.xz, layerIndex));
            float3 color2 = UNITY_SAMPLE_TEX2DARRAY(terrainTextures, float3(scaledWorldPos.xz, layerIndex + 1));

            // Perform linear interpolation between the colors based on the blend factor
            o.Albedo = lerp(color1, color2, blendFactor);

            // Calculate the specular term
            float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos); // Corrected the variable name
            float specular = pow(saturate(dot(viewDir, reflect(-_WorldSpaceLightPos0, o.Normal))), 16.0);

            // Combine the specular term with the Albedo
            o.Albedo += _specularIntensity * _LightColor0.rgb * specular;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
