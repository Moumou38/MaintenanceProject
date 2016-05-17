Shader "Custom/Ghost" 
{ 
	Properties 
	{ 
		_Color("Color", Color) = (1, 1, 1, 1) 
	} 
	SubShader 
	{ 
		Tags { "RenderType"="Opaque" }
		Lighting Off 
		Color[_Color]
		ZWrite On
		ZTest LEqual
		Offset 1, 0
		Pass {} 
	}
}