Shader "Planets/Atmosphere" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0
	}
	SubShader {
	ZWrite OFF
	Cull Back
	Blend SrcAlpha OneMinusSrcAlpha
	Tags { "Queue"="Overlay" "RenderType"="Transparent" }
		Pass {
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		// Use shader model 3.0 
		#pragma target 3.0

		uniform float4 _Color;
		uniform float _RimPower;

		// unity defined
		uniform float4 _LightColor0;

		struct vertexInput {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 posWorld : TEXCORD0;
			float3 normalDir : TEXCORD1;
		};

		vertexOutput vert(vertexInput v) {
			vertexOutput o;

			o.posWorld = mul(_Object2World, v.vertex);
			o.normalDir = normalize( mul( float4(v.normal, 0.0), _World2Object).xyz );
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			return o;
		}

		float4 frag(vertexOutput i) : COLOR {
			float3 normalDirection = i.normalDir;
			float3 viewDirection  = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
			float rim = 1.0 - dot(viewDirection, normalDirection);
			float rimAttenuated = pow(rim, _RimPower);
			return float4(_Color.xyz, rimAttenuated * _Color.w);
		}

		ENDCG
		}
	}
	FallBack "Diffuse"
}
