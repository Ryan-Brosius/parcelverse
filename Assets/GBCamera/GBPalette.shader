Shader "GBCamera/GBSimpleTrippyPixel" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FadeAmount ("Fade Amount", Range(0, 1)) = 0
        _TrippyPower ("Trippy Power", Range(0, 1)) = 0.5
        _PixelPower ("Pixelation Strength", Range(0, 1)) = 0.5
        _ColorStreaks ("Color Streaks", Range(0, 1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass {
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

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float time = _Time.y;

                float wave = sin(i.uv.y * 40.0 + time * 10.0) * 0.01 * _TrippyPower;
                float2 uv = i.uv + wave;

                float pixelSize = lerp(0.001, 0.05, _PixelPower * _FadeAmount);
                uv = floor(uv / pixelSize) * pixelSize;

                float streakR = sin((uv.y + time) * 20.0) * _ColorStreaks;
                float streakG = cos((uv.y + time) * 15.0) * _ColorStreaks;
                float streakB = sin((uv.x + time) * 10.0) * _ColorStreaks;

                fixed4 tex = tex2D(_MainTex, uv);
                tex.rgb += float3(streakR, streakG, streakB);

                tex.a *= 1.0;

                return tex * _Color * i.color;
            }
            ENDCG
        }
    }
}
