Shader "Pools/PoolBallEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Outline ("Out line", Range(0,0.001)) = 0
		_AmbientColor ("Ambient color", Color) = (1,1,1,1)
		_Emission ("Emission", Color) = (1,1,1,1)
	}
	SubShader {
		Pass{
			Cull Front
		
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			struct vInput{
				fixed4 vertex : POSITION;
				fixed4 normal : NORMAL;
			};
			
			struct v2f{
				fixed4 position : SV_POSITION;
			};
			
			fixed _Outline;
			
			v2f vert(vInput v)
			{
				v2f o;
				
				o.position = v.vertex + v.normal * _Outline;
				o.position = mul(UNITY_MATRIX_MVP, o.position);
				
				
				return o;
			}
			
			fixed4 frag(v2f v) : COLOR{
				return fixed4(0,0,0,1);
			}
						
			ENDCG
		}
		
		Pass{
			Material{
				Diffuse(1,1,1,1)
				Ambient[_AmbientColor]
				Emission[_Emission]
			}
			Lighting On
			SetTexture[_MainTex]{
				Combine primary * texture double
			}
		}
	}
	FallBack "Diffuse"
}
