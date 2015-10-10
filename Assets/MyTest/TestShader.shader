Shader "TestShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white"{}
	}
	
	SubShader 
	{
		Tags{"Queue"="Transparent"}
	
		Pass
		{
			Blend DstColor Zero
			ZWrite off
			ZTest Always
			
			SetTexture[_MainTex]
			{
				Combine texture
			}
		}
	} 
	FallBack "Diffuse"
}

