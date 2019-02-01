Shader "EQ/AlphaFromBrightness" 
{
    Properties 
    {
        _MainTex ("_MainTex", 2D) = "white" {}
    }
    
    SubShader 
    {
        Tags 
        {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        
        LOD 200
        
        Pass 
        {
            Name "FORWARD"
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
			
            uniform sampler2D _MainTex; 
            uniform float4 _MainTex_ST;

			
            struct VertexInput 
            {
                float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;

            };
            
            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            
            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
				o.uv0 = v.texcoord0;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            
            float4 frag(VertexOutput i) : COLOR 
            {
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                _MainTex_var.a = (_MainTex_var.r + _MainTex_var.b + _MainTex_var.g) / 3.0;
                UNITY_APPLY_FOG(i.fogCoord, _MainTex_var);
                return _MainTex_var;
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
