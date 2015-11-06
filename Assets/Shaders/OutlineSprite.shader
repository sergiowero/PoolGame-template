Shader "Pools/OutlineSprite" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader {
			Tags
			{
				"Queue"="Transparent"
				"IgnoreProjector"="False"
				"RenderType"="Transparent"
				"PreviewType"="Plane"
				"CanUseSpriteAtlas"="True"
			}
		
		Pass
		{
			Cull Off
			ZWrite Off
			Fog {Mode Off}
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting On
			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
				Combine texture * constant
			}

			
			
		}
	} 
	FallBack "Diffuse"
}
