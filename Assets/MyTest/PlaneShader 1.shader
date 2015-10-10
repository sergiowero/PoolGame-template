Shader "Custom/PlaneShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags{
			"Queue"="Transparent+10" 
		}
		
		Pass{
			Stencil{
				Ref 1
				Comp Always
				Pass Replace
			}
			Blend SrcAlpha OneMinusSrcAlpha
			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
				Combine texture * constant
			}
		}
	} 
	FallBack "Diffuse"
}
