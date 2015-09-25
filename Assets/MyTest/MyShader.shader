Shader "Custom/MyShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#define t _Time
	#define r _ScreenParams.xy
	
	
	struct vert_Out
	{
		float4 pos : SV_POSITION;
		float4 srcPos : TEXCOORD0;
	};
	
	struct vert_In
	{
		float4 vertex : POSITION;
	};
	
	vert_Out vert(vert_In v)
	{
		vert_Out o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.srcPos = ComputeScreenPos(o.pos);
		return o;
	}
	
	float4 frag(vert_Out vv) : COLOR
	{
		float2 fragCoord = ((vv.srcPos.xy / vv.srcPos.w) * r);
		float3 c;
		float l, z = t * 10;
		for(int i = 0; i < 3; i++)
		{
			float2 uv, p = fragCoord.xy / r; 
			uv = p;
			p -= .5f;
			p.x *= r.x / r.y;
			z += .07f;
			l = length(p);
			uv += p / l * (sin(z) + 1) * abs(sin(l * 9 - z * 2));
			c[i] = .01f / length(abs(fmod(uv, 1) - .5));
		}
		return float4(c / l, t.y);
	}
	
	ENDCG
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	} 
	FallBack "Diffuse"
}


