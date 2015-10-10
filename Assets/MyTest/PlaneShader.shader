Shader "Custom/PlaneShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags{
			"Queue"="Geometry"
		}
		
		GrabPass{
			"_MyGrabTexture"
		}
		
		Pass{
			Stencil{
				Ref 1
				Comp Always
				Pass Replace
			}
			SetTexture[_MyGrabTexture]
			{
				ConstantColor[_Color]
				Combine texture * constant
			}
		}
	} 
	FallBack "Diffuse"
}
