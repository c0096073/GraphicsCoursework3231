Shader "Custom/SpaceShipBeam" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _Emission ("Emission", Color) = (0,0,0,0)
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    
    SubShader {
        Tags { "Queue" = "Overlay" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert

        fixed4 _Color;
        fixed4 _Emission;

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
          
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            //emission
            o.Emission = _Emission.rgb;

           
            o.Alpha = c.a;
        }
        ENDCG
    }
    
    Fallback "Diffuse"
}
