Shader "GBCamera/GBSimpleTrippyPixel"
{
    Properties
    {
        [PerRendererData]_MainTex("MainTex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _FadeAmount("Fade Amount", Range(0, 1)) = 0
        _TrippyPower("Trippy Power", Range(0, 1)) = 0.5
        _PixelPower("Pixelation Strength", Range(0, 1)) = 0.5
        _ColorStreaks("Color Streaks", Range(0, 1)) = 0.5
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FadeAmount;
            float _TrippyPower;
            float _PixelPower;
            float _ColorStreaks;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y;

                // Wavy distortion in multiple directions
                float2 offset;
                offset.x = sin(i.uv.y * 40.0 + time * 4.0) * 0.01;
                offset.y = cos(i.uv.x * 50.0 + time * 6.0) * 0.01;
                offset *= _TrippyPower;

                float2 uv = i.uv + offset;

                // Pixelation effect
                float pixelSize = lerp(0.001, 0.05, _PixelPower * _FadeAmount);
                uv = floor(uv / pixelSize) * pixelSize;

                // Color streaks with multiple wave layers
                float rWave = sin((uv.y + time * 1.3) * 10.0 + cos(uv.x * 15.0 + time * 2.0));
                float gWave = sin((uv.x + time * 1.7) * 12.0 + cos(uv.y * 10.0 + time));
                float bWave = sin((uv.x + uv.y + time * 2.0) * 8.0);

                float3 streaks = float3(rWave, gWave, bWave) * 0.2 * _ColorStreaks;

                // Sample base texture
                fixed4 texColor = tex2D(_MainTex, uv);
                texColor.rgb = texColor.rgb * (1.0 - _FadeAmount) + streaks;

                return texColor * i.color * _Color;
            }
            ENDCG
        }
    }
}
