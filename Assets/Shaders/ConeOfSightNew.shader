Shader "GPUMan/ConeOfSightNew" {
	Properties{
		_Color("Color",Color) = (1,1,1,1)
		_SightAngle("Sight Angle", Range(0.01,90)) = 45
	}

		Subshader{
			Tags 
			{
				"RenderType" = "Transparent"
				
			}
			Pass {
				
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct v2f
				{
					float4 pos : SV_POSITION;
					//float4 uv : TEXCOORD0;
					float3 ray : TEXCOORD1;
					float4 screenUV : TEXCOORD2;
				};

				sampler2D _ViewDepthTexture;
				sampler2D_float _CameraDepthTexture;

				float4x4 _ViewSpaceMatrix;
				float4 _Color;
				float _SightAngle;

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.ray = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);
					o.screenUV = ComputeScreenPos(o.pos);
					
					return o;
				}

				float linearizeDepth(float z, float n, float f)
				{
					float c1 = f / n;
					float c0 = 1.0 - c1;
					return 1.0 / (c0 * z + c1);
				}

				half4 frag(v2f i) : COLOR
				{
					const float PI = 3.14159;

					i.ray = i.ray * (_ProjectionParams.z / i.ray.z);  // farPlane / rayZ ???? what for? normalizing the ray?

					// 3D point reconstruction ----------------
					float2 uv = i.screenUV.xy / i.screenUV.w;
					float depth  = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
					depth = Linear01Depth(depth);
					float4 vpos = float4(i.ray * depth, 1);
					float4 wpos = mul(unity_CameraToWorld, vpos);
					float3 opos = mul(unity_WorldToObject, wpos);
					//opos.y = 0;
					
					//Discard hit point if it is outside the box
					clip(float3(0.5, 0.5, 0.5) - abs(opos.xyz));

					wpos.w = 1;
					float4 posViewSpace = mul(_ViewSpaceMatrix, wpos);
					
					float sightAngleRadians = _SightAngle / 2 * PI / 180;

					float distFromCenter = length(opos.xz);
					float innerCircleAlpha = saturate(1 - (0.05 - distFromCenter) * 50);
					float alpha = max((0.5 - length(opos)) * 10 * innerCircleAlpha, 0);
					
					float3 npos = normalize(opos);
					float fwdDotPos = max(dot(float3(0, 0, 1), npos), 0);
					//float crossSign = sign(dot(float3(0,1,0), cross(float3(0, 0, 1), npos)));
					float angle = acos(fwdDotPos);
										
					float angleF = angle / sightAngleRadians;
					alpha *= step(angleF,1);

					//float f = (angle*crossSign + sightAngleRadians) / (_SightAngle * PI / 180);
					//float sightDepth = SAMPLE_DEPTH_TEXTURE(_ViewDepthTexture, float2(f,1));

					
					float3 projCoords = posViewSpace.xyz / posViewSpace.w;
					projCoords = projCoords * 0.5 + 0.5;
					float3 col = tex2D(_ViewDepthTexture, projCoords.xy).r * 100;//getDepth(posViewSpace);
					return saturate(float4(col, alpha));

					//float3 col * 100;
					//return float4(col,1);

				}
				ENDCG
			}
	}
}