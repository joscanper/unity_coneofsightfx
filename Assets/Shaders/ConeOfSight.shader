// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ShaderPack/ConeOfSight" {
	Properties {
		_Color("Color",Color) = (1,1,1,1)
		_SightAngle("SightAngle",Float) = 0.5
		_FarHardness("FarHardness",Float) = 0.5
		_RangeHardness("RangeHardness",Range(0,100)) = 5
		_RangeStep("RangeStep",Range(0,1)) = 0.7
		_SourceWhiteness("SourceWhiteness",Range(0,1)) = 1
		//_SourceGlow("SourceGlow",Range(1,10)) = 1
	}

	SubShader{
	 	Tags { 
	 		"Queue"="Transparent"
	 		"RenderType"="Transparent" 
	 	} 
	 	Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			ZWrite Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f{
				float4 position : SV_POSITION;
				float4 uv : TEXCOORD0;
			};

			half4 _Color;
			half _SightAngle;
			half _FarHardness;
			half _RangeHardness;
			half _RangeStep;
			half _SourceWhiteness;
			//half _SourceGlow;

			//uniform half _CurrentAngle = 0;
			//int _BufferSize = 64;
			uniform float _SightDepthBuffer[256];

			//Vertex
			v2f vert(appdata_base IN){
				v2f o;
				o.position = UnityObjectToClipPos(IN.vertex);
				o.uv = IN.texcoord;
				return o;
			}

			//Fragment
			fixed4 frag(v2f IN) : SV_Target{
				const float PI = 3.14159;

				IN.uv.x -= 0.5f;
				IN.uv.y -= 0.5f;
				half distcenter = 1-sqrt(IN.uv.x*IN.uv.x + IN.uv.y*IN.uv.y)*2;

				half2 fragmentDir = normalize(IN.uv.xy);
				half viewDotPos = clamp(dot(half2(1,0), fragmentDir),0,1);
				half sightAngleRads = _SightAngle/2 * PI / 180;
				half sightVal = cos(sightAngleRads);

				half4 col = lerp(_Color,half4(1,1,1,1),distcenter*_SourceWhiteness);
				col.a *= pow(viewDotPos/sightVal,_RangeHardness) *distcenter * pow(distcenter,_FarHardness);

				//col.a *= clamp(distcenter*abs(pow(col.a,-_FarHardness*10)),0,1);
				if (viewDotPos<sightVal){
					col.a *= _RangeStep;
				}else{
					// --- Depth check
					float fragmentAngle = asin(fragmentDir.y)+sightAngleRads;
					float fragmentVal = 1.0f-(fragmentAngle)/(sightAngleRads*2);
					int index =  fragmentVal * 256;
					if (_SightDepthBuffer[index]>0 && (1-distcenter)>_SightDepthBuffer[index])
						col *= 0;
				}

				col.a *= _Color.a;
				return saturate(col);
			}

			ENDCG
		}
	}

}

