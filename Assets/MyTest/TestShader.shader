Shader "TestShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_XRange("X range", Range(-1,1)) = 0
		_YRange("Y Range", Range(-1,1)) = 0
	}
	
	CGINCLUDE
	
	half4 _Color;
	half _XRange;
	half _YRange;
	
	struct v2f
	{
		half4 position : SV_POSITION;
		half4 worldPosition : TEXCOORD0;
	};
	
	struct vertexInput
	{
		half4 vertex : POSITION;
	};
	
	v2f vert(vertexInput v)
	{
		v2f vf;
		vf.worldPosition = mul(_Object2World, v.vertex);
		vf.position = mul(UNITY_MATRIX_MVP, v.vertex);
		return vf;
	}
	
	half4 frag(v2f v) : COLOR
	{
		half4 modelPosition = mul(_World2Object, v.worldPosition);
		half4 pixelViewportPosition = mul(UNITY_MATRIX_MV, modelPosition);
		
		half4 c = _Color;
		
		if(pixelViewportPosition.x > _XRange || pixelViewportPosition.y > _YRange)
			c.a = 0;
		
		return c;
	}
	
	ENDCG
	
	SubShader 
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}

