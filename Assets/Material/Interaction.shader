Shader "Unlit/Interaction"
{
    Properties
    {
        _MainTex ("纹理", 2D) = "white" {}  // 主纹理，默认为白色
        // AlphaThreshold("Alpha 阈值", Range(0,1))=0.1  // Alpha 阈值，范围在 0 到 1 之间，默认值为 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }  // 设置渲染类型为不透明，渲染队列为透明
        ZWrite Off  // 关闭深度写入
        Blend SrcAlpha OneMinusSrcAlpha  // 使用 alpha 混合模式
        LOD 100  // 设置细节级别为 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert  // 定义顶点着色器
            #pragma fragment frag  // 定义片段着色器

            #include "UnityCG.cginc"  // 包含 Unity 的 CG 库

            struct appdata
            {
                float4 vertex : POSITION;  // 顶点位置
                float2 uv : TEXCOORD0;  // 纹理坐标
                float4 col : COLOR;  // 顶点颜色
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;  // 传递给片段着色器的纹理坐标
                float4 vertex : SV_POSITION;  // 传递给片段着色器的裁剪空间顶点位置
                float4 col : TEXCOORD1;  // 传递给片段着色器的顶点颜色
            };

            sampler2D _MainTex;  // 主纹理采样器
            float4 _MainTex_ST;  // 主纹理的平铺和偏移值
            // float AlphaThreshold;  // Alpha 阈值

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);  // 将顶点位置从模型空间转换到裁剪空间
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);  // 应用纹理的平铺和偏移
                o.col = v.col;  // 传递顶点颜色
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.col;  // 从主纹理中采样颜色，并与顶点颜色相乘
            
                // float brightness = (col.r + col.g + col.b) / 3.0;  // 计算颜色的亮度
                // col.a = smoothstep(AlphaThreshold, 1.0, brightness);  // 根据亮度和阈值调整 alpha 值
                return col;  // 返回最终颜色
            }
            ENDCG
        }
    }
}