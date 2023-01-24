Shader "Unlit/GrassMobileRef1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaCutout("_AlphaCutout",Range(0,1.01)) = 0.5
        [Header(LightMap)]
        [Toggle]_USELIGHTMAP("UseLightMap",float) = 0
        [Toggle]_USELIGHTMAPCOLOR("UseLightMapColor",float) = 0
        [HDR]_LightMapColor("LightMapColor",color) = (0,0,0,0)

        [Header(VertexAnimation)]
        [Toggle]_USEVERTEXANIMATION("_UseVertexAnim",float) = 0
        _MoveAmount("MoveAmount",float) = 0.2    
        _MoveSpeed("MoveSpeed",float) = 2
        _MoveTilling("MoveTilling",float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        cull off
        Lighting OFF

        Pass
        {
            Lighting OFF
            CGPROGRAM
            
            //#pragma surface surf Standard fullforwardshadows
            #pragma vertex vert fragment frag


            //fog work making
            #pragma multi_complie_fog

            //use lightmap
            #pragma shader_feature _USELIGHTMAP_OFF _USELIGHTMAP_ON
            #pragma shader_feature _USELIGHTCOLOR_OFF _USELIGHTCOLOR_ON
            #pragma shader_feature _USEVERTEXANIMATION_OFF _USEVERTEXANIMATION_ON

            #include "UnityCG.cginc"

            struct appdata 
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct appdata_lightmap
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };

            struct v2f 
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 uv1 : TEXCOORD3;

                //world position
                float4 worldSpacePos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _AlphaCutout;
            half3 _LightMapColor;

            //vectex Anim data
            half _MoveAmount;
            half _MoveSpeed;
            half _MoveTilling;

            v2f vert(appdata_full v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                //vertex anim
                #if _USEVERTEXANIMATION_ON
                o.worldSpacePos = mul(unity_ObjectToWorld,v.vertex);
                o.vertex.x += (sin(o.worldSpacePos.z * _MoveTilling) + _Time.y * _MoveSpeed) * o.uv.y;
                #endif

                //LightMap
                #if _USELIGHTMAP_ON 

                //using unity_LightmapST
                o.uv1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                #endif
                    

                UNITY_TRANSFER_FOG(o, o.vertex);
                return 0;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                //sampling texture
                fixed4 col = tex2D(_MainTex, i.uv);

                UNITY_APPLY_FOG(i.fogCoord, col);
                clip(col.a - _AlphaCutout);

                #if _USELIGHTMAP_ON

                col.rgb *= (DecodeLightMap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1)))
                    #if _USELIGHTMAPCOLOR_ON
                     +_LightMapColor
                    #endif

                #endif

                return col;
            }
        
            ENDCG
        }
        
    }
}
