Shader "StencilTest" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	
	
	
	SubShader {
		Tags{"Queue"="Transparent"}
		Pass{
			Material 
			{
				Diffuse[_Color]
			}
			Lighting On
			SetTexture[_MainTex]
			{
				Combine texture * primary double
			}
		}
		Pass{
			Stencil{
				Ref 1
				Comp Equal
				Pass Keep
			}
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			half4 _Color;
			half4x4 _RefMat;
			
			struct vInput{
				half4 vertex : POSITION;
				half4 uv : TEXCOORD0;
			};
			
			struct v2f {
				half4 position : SV_POSITION;
				half4 uv : TEXCOORD0;
			};
			
			v2f vert(vInput v) {
				v2f o;
				o.position = v.vertex;
				o.position = mul(_Object2World, o.position);
				o.position = mul(_RefMat, o.position);
				o.position = mul(_World2Object, o.position);
				o.position = mul(UNITY_MATRIX_MVP, o.position);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag (v2f v) : COLOR
			{
				return tex2D(_MainTex, v.uv.xy) * _Color;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

