Shader "TecWolf/SkyGradient" {
	Properties {
		_Color ("Top Color", Color) = (1,1,1,1)
		_Color1 ("Bottom Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types.
		#pragma surface surf Standard

		// Use shader model 3.0 target, to get nicer looking lighting.
		#pragma target 2.0

		fixed4 _Color;
		fixed4 _Color1;

		struct Input {
			fixed4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			fixed4 c = lerp(_Color, _Color1, screenUV.y);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	FallBack "VertexLit"
}
