Shader "Unlit/Color" 
{ 
	Properties 
	{ 
		_Color("Color", Color) = (1, 1, 1, 1) 
	} 
	SubShader 
	{ 
		Lighting Off 
		Color[_Color] 
		//ColorMask G
		ZWrite on
		ZTest LEqual
		Offset 10, 10
		Pass {} 
	}
}