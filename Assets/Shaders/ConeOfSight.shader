Shader "GPUMan/ConeOfSight" {
	Properties{
		_Color("Color",Color) = (1,1,1,1)
		_NonVisibleColor("Non Visible Color",Color) = (0,0,0,1)
		_ViewAngle("Sight Angle", Range(0.01,90)) = 45
		_AngleStrength("Angle Strength", Float) = 1
		_ViewIntervals("Intervals", Range(0, 1)) = 0.0075
		_ViewIntervalsStep("Intervals Step", Float) = 0.0025
		_InnerCircleSize("InnerCircleSize", Range(0, 1)) = 0.05
		_CircleStrength("CircleStrength", Float) = 70
	}

		Subshader{
			Tags
			{
				"Queue" = "Transparent"
			}
			Pass {
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				
				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 ray : TEXCOORD1;
					float4 screenUV : TEXCOORD2;
				};

				sampler2D _ViewDepthTexture;
				sampler2D_float _CameraDepthTexture;

				float4x4 _ViewSpaceMatrix;
				half _AngleStrength;
				half4 _Color;
				half4 _NonVisibleColor;
				half _ViewAngle;
				float _ViewIntervals;
				float _ViewIntervalsStep;
				half _InnerCircleSize;
				half _CircleStrength;

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					o.ray = UnityObjectToViewPos(v.vertex) * float3(1, 1, -1);

					o.screenUV = ComputeScreenPos(o.pos);

					return o;
				}

				float getRadiusAlpha(float distFromCenter)
				{
					float innerCircleAlpha = saturate(distFromCenter - _InnerCircleSize);
					return max((0.5 - distFromCenter) * innerCircleAlpha  * _CircleStrength, 0);
				}

				float getAngleAlpha(float2 pos)
				{
					const float PI = 3.14159;
					float sightAngleRadians = _ViewAngle / 2 * PI / 180;
					float2 npos = normalize(pos);
					float fwdDotPos = max(dot(float2(0, 1), npos), 0);
					float angle = acos(fwdDotPos);
					float angleF = angle / sightAngleRadians;
					return max(1.0 - pow(angleF, _AngleStrength), 0);
				}

				float getObstacleAlpha(float4 worldPos)
				{
					const float BIAS = 0.0001;
					float4 posViewSpace = mul(_ViewSpaceMatrix, worldPos);
					float3 projCoords = posViewSpace.xyz / posViewSpace.w;	// ndc : -1 to 1
					projCoords = projCoords * 0.5 + 0.5; // 0 to 1
					float sampledDepth = (1.0 - SAMPLE_DEPTH_TEXTURE(_ViewDepthTexture, projCoords.xy));
					float depthDiff = (projCoords.z - BIAS) - sampledDepth;
					return saturate(depthDiff > 0 ? 0 : 1);
				}

				half4 frag(v2f i) : COLOR
				{
					i.ray = i.ray * (_ProjectionParams.z / i.ray.z);  // farPlane / rayZ

					// 3D point reconstruction from depth texture
					float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenUV);
					depth = Linear01Depth(depth);
					float4 vpos = float4(i.ray * depth, 1);
					float4 wpos = mul(unity_CameraToWorld, vpos);

					// Discard point if is a vertical surface
					clip((dot(normalize(ddy(wpos)), float3(0, 1, 0)) > 0.45) ? -1 : 1);

					float3 opos = mul(unity_WorldToObject, wpos);
					opos.y = 0;

					// Discard hit point if it is outside the box
					clip(float3(0.5, 0.5, 0.5) - abs(opos.xyz));

					// Alpha calculation
					float2 pos2D = opos.xz;
					float distFromCenter = length(pos2D);
					float obstacleAlpha = getObstacleAlpha(wpos); // 0 if occluded, 1 if not
					float alpha = getRadiusAlpha(distFromCenter) * getAngleAlpha(pos2D);

					// Cone stripes
					float intervals = _ViewIntervals > 0 ? (distFromCenter % _ViewIntervals) : 0;
					alpha *= step(_ViewIntervalsStep, intervals);

					float4 col = obstacleAlpha > 0 ? _Color : _NonVisibleColor;
					return saturate(float4(col.rgb, alpha * col.a));
				}
				ENDCG
			}
		}
		Subshader {
			Tags
			{
				"Queue" = "Transparent"
				"RenderPipeline" = "UniversalRenderPipeline"
			}
			Pass {
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off

				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"  
				
            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;                 
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
            };            

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.            
            half4 frag() : SV_Target
            {
                // Defining the color variable and returning it.
                half4 customColor;
                customColor = half4(0.5, 0, 0, 1);
                return customColor;
            }
				ENDHLSL
		}
		
	}
}