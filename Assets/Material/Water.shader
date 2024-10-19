Shader "Unlit/Water"
{
    Properties
    {
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _BumpTex ("BumpTex", 2D) = "white" {}
        _DeepColor ("DeepColor", Color) = (0,0,0,0)
        _ShallowColor ("ShallowColor", Color) = (0,0,0,0)
        _RampDistance ("RampDistance", float) = 0
        _Scale("TexScale",Vector) = (0,0,0,0)
        _NoiseCutOff("NoiseCutOff",float) = 0
        _FoamThickness("FoamThickness",float) = 0
        _FoamColor("FoamColor", Color) = (0,0,0,0)
        _Distortion("Distortion",float) = 0  // 添加用于折射的扭曲参数
        _Visility("Visibility",range(0,1)) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
       
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _NoiseTex, _CameraDepthTexture;
            sampler2D _BumpTex, _CameraOpaqueTexture;  // 使用CameraOpaqueTexture代替GrabPass
            float4 _DeepColor, _ShallowColor, _Scale, _FoamColor;
            float _RampDistance, _NoiseCutOff, _FoamThickness, _Distortion,_Visility;
            uniform float _OrthographicSize;
            uniform float4 _Position;
            uniform sampler2D _RenderTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.worldPos = mul(v.vertex, unity_ObjectToWorld);
                return o;
            }

            fixed4 BlendColor(fixed4 top, fixed4 bottom)
            {
                float3 col = top.rgb * top.a + bottom.rgb * (1 - top.a);
                float a = top.a + bottom.a * (1 - top.a);
                return fixed4(col, a);
            }

            fixed4 frag (v2f i) : SV_Target
            {
               // 计算当前世界坐标位置相对于指定位置的纹理坐标
float2 rtUv = i.worldPos.xz - _Position.xz; 
// 根据正交大小归一化纹理坐标
rtUv /= _OrthographicSize * 2;
// 将纹理坐标偏移至中心为原点的坐标系
rtUv += 0.5;

// 从渲染纹理中采样得到涟漪效果的亮度值
float ripples = tex2D(_RenderTexture, rtUv).b;

// 使用阈值比较将涟漪效果转换为二进制值
ripples = step(.8, ripples);

                // 生成噪声
                fixed4 noise = tex2D(_NoiseTex, float2(i.worldPos.x * _Scale.x, i.worldPos.z) + _Time.x) +
                               tex2D(_NoiseTex, float2(i.worldPos.x, i.worldPos.z * _Scale.y) + _Time.x);
                               
                // 获取深度纹理
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, uv));
                
                // 水深的差异计算
                float waterDepthDifference = saturate((depth - i.screenPos.w) / _RampDistance);
                
                // 泡沫深度差异计算
                float foamDepthDifference = saturate((depth - i.screenPos.w) / _FoamThickness);
                foamDepthDifference += _NoiseCutOff;
                noise += ripples;
                float foam = noise > foamDepthDifference ? 1 : 0;
                fixed4 foamColor = foam * _FoamColor;

                // 计算法线扰动
                fixed3 bump = UnpackNormal(tex2D(_BumpTex, i.worldPos.xz + _Time.x));
                float2 offset = bump.xy * _Distortion; // 用于水面折射的扭曲
                
                // 获取折射颜色 (CameraOpaqueTexture)
                float4 screenPos = float4(i.screenPos.xy + offset, i.screenPos.zw);
                uv = screenPos.xy / screenPos.w;
                 depth = LinearEyeDepth(tex2D(_CameraDepthTexture, uv));
                 waterDepthDifference = saturate(depth - screenPos.w);
                fixed4 refrColor = tex2D(_CameraOpaqueTexture, uv);

                // 混合颜色：浅水和深水之间的过渡
                fixed4 waterColor = lerp(_ShallowColor, _DeepColor, waterDepthDifference);

                // 最终的水面颜色，带有泡沫和折射效果
                waterColor = BlendColor(fixed4(waterColor.rgb,_Visility),refrColor);
                // return BlendColor(foamColor, waterColor) + refrColor * ripples;
                return BlendColor(foamColor, waterColor);
            }
            ENDCG
        }
    }
}
