 
Shader "Tiles" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _gNumTiles ("Number Of Tiles", Range (1,100)) = 1
        _gThreshhold ("Edge Width", Range (0.0,2.0)) = 0.15
        _gEdgeColor ("Color", Color)  = (.5, .5, .5, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
 
        struct Input {
            float2 uv_MainTex;
        };
       
        float _gNumTiles;
        float _gThreshhold;
        float4 _gEdgeColor;
       
    void surf (Input IN, inout SurfaceOutput o) {
           
        half size = 1.0/_gNumTiles;
        half2 Pbase = IN.uv_MainTex - fmod(IN.uv_MainTex,size.xx);
        half2 PCenter = Pbase + (size/2.0).xx;
        half2 st = (IN.uv_MainTex - Pbase)/size;
        half4 c1 = (half4)0;
        half4 c2 = (half4)0;
        half4 invOff = half4((1-_gEdgeColor.xyz),1);
        if (st.x > st.y) { c1 = invOff; }
        half threshholdB =  1.0 - _gThreshhold;
        if (st.x > threshholdB) { c2 = c1; }
        if (st.y > threshholdB) { c2 = c1; }
        half4 cBottom = c2;
        c1 = (half4)0;
        c2 = (half4)0;
        if (st.x > st.y) { c1 = invOff; }
        if (st.x < _gThreshhold) { c2 = c1; }
        if (st.y < _gThreshhold) { c2 = c1; }
        half4 cTop = c2;
        half4 tileColor = tex2D(_MainTex,PCenter);
        half4 result = tileColor + cTop - cBottom;
        o.Albedo = result.rgb;
        o.Alpha = result.a;
           
        }
        ENDCG
    }
    FallBack "Diffuse"
}