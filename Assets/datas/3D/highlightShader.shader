Shader "Custom/highlightShader" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1) 
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite off
		ZTest Always
		//Offset 0, 10
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * (sin(_Time.w / 2) *0.2 + 0.5);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
