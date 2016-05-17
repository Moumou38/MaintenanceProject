Shader "Custom/LineShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }
   
    SubShader {
        Tags {"Queue"="Transparent"  "RenderType"="Transparent" }
        LOD 100
        Pass {
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha 
            Cull Back
            SetTexture[_] {
                constantColor [_Color]
                Combine constant
            }
        }
    }	
    FallBack "Diffuse"
}
