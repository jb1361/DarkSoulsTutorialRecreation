// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Time of Day/Moon (Flat)"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent-480"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}

		Pass
		{
			Cull Back
			ZWrite Off
			ZTest LEqual
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "TOD_Base.cginc"

			uniform float3 _Color;
			uniform float _Phase;
			uniform float _Contrast;
			uniform float _Brightness;

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			struct v2f {
				float4 position : SV_POSITION;
				float3 tex      : TEXCOORD0;
				float3 normal   : TEXCOORD1;
				float4 shading  : TEXCOORD2;
			};

			v2f vert(appdata_base v) {
				v2f o;

				float phaseabs = abs(_Phase);
				float3 offset = float3(0, _Phase, -phaseabs) * 5;

				float3 viewdir = normalize(ObjSpaceViewDir(v.vertex));

				o.position    = TOD_TRANSFORM_VERT(v.vertex);
				o.tex.xy      = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.tex.z       = (mul(TOD_World2Sky, mul(unity_ObjectToWorld, v.vertex)).y + TOD_Horizon) * 25;
				o.normal      = v.normal;
				o.shading.xyz = normalize(viewdir + offset);
				o.shading.w   = (1-phaseabs);

				return o;
			}

			half4 frag(v2f i) : COLOR {
				half4 color = half4(_Color, 1);

				half shading = i.shading.w * sqrt(max(0, dot(i.normal, i.shading.xyz))) * saturate(i.tex.z);

				half3 moontex = tex2D(_MainTex, i.tex).rgb;
				color.rgb *= moontex * shading;

				color.rgb = pow(color.rgb, _Contrast) * _Brightness;

				return color;
			}

			ENDCG
		}
	}

	Fallback Off
}
