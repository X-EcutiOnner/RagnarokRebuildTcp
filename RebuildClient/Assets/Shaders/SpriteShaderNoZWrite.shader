// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Ragnarok/CharacterSpriteShaderNoZWrite"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Color("Tint", Color) = (1,1,1,1)
		[PerRendererData] _Offset("Offset", Float) = 0
		[PerRendererData] _Width("Width", Float) = 0
		_Rotation("Rotation", Range(0,360)) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"ForceNoShadowCasting" = "True"
			"DisableBatching" = "true"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			//#pragma multi_compile _ PIXELSNAP_ON
			//#pragma multi_compile _ WATER_OFF
		

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "Billboard.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				half4  worldPos : TEXCOORD2;
				UNITY_FOG_COORDS(3)
			};

			fixed4 _Color;
			fixed _Offset;
			fixed _Rotation;
			fixed _Width;


			sampler2D _MainTex;

			float4 _ClipRect;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			sampler2D _WaterDepth;
			sampler2D _WaterImageTexture;
			float4 _WaterImageTexture_ST;
								
			float _MaskSoftnessX;
			float _MaskSoftnessY;

			//from our globals
			float4 _RoAmbientColor;
			float4 _RoDiffuseColor;

			float4 Rotate(float4 vert)
			{
				float4 vOut = vert;
				vOut.x = vert.x * cos(radians(_Rotation)) - vert.y * sin(radians(_Rotation));
				vOut.y = vert.x * sin(radians(_Rotation)) + vert.y * cos(radians(_Rotation));
				return vOut;
			}

			v2f vert(appdata_t v)
			{
				v2f o;
				
				//v.vertex = Rotate(v.vertex);
		
				//--------------------------------------------------------------------------------------------
				//start of billboard code
				//--------------------------------------------------------------------------------------------

				float2 pos = v.vertex.xy;
	
				float3 worldPos = mul(unity_ObjectToWorld, float4(pos.x, pos.y, 0, 1)).xyz;
				float3 originPos = mul(unity_ObjectToWorld, float4(pos.x, 0, 0, 1)).xyz; //world position of origin
				float3 upPos = originPos + float3(0, 1, 0); //up from origin

				float outDist = abs(pos.y); //distance from origin should always be equal to y

				float angleA = Angle(originPos, upPos, worldPos); //angle between vertex position, origin, and up
				float angleB = Angle(worldPos, _WorldSpaceCameraPos.xyz, originPos); //angle between vertex position, camera, and origin

				float camDist = distance(_WorldSpaceCameraPos.xyz, worldPos.xyz);

				if (pos.y > 0)
				{
					angleA = 90 - (angleA - 90);
					angleB = 90 - (angleB - 90);
				}

				float angleC = 180 - angleA - angleB; //the third angle

				float fixDist = 0;
				if (pos.y > 0)
					fixDist = (outDist / sin(radians(angleC))) * sin(radians(angleA)); //supposedly basic trigonometry

				//determine move as a % of the distance from the point to the camera
				float decRate = (fixDist * 0.7 - _Offset / 4) / camDist; //where does the value come from? Who knows!
				float decRateNoOffset = (fixDist * 0.7) / camDist; //where does the value come from? Who knows!
				float decRate2 = (fixDist) / camDist; //where does the value come from? Who knows!

				float4 view = mul(UNITY_MATRIX_V, float4(worldPos, 1));

				float4 pro = mul(UNITY_MATRIX_P, view);

				#if UNITY_UV_STARTS_AT_TOP
					// Windows - DirectX
					view.z -= abs(UNITY_NEAR_CLIP_VALUE - view.z) * decRate2;
					pro.z -= abs(UNITY_NEAR_CLIP_VALUE - pro.z) * decRate;
				#else
					// WebGL - OpenGL
					view.z += abs(UNITY_NEAR_CLIP_VALUE) * decRate2;
					pro.z += abs(UNITY_NEAR_CLIP_VALUE) * decRate;
				#endif

				o.vertex = pro;

				//--------------------------------------------------------------------------------------------
				//end of billboard code
				//--------------------------------------------------------------------------------------------
		
				o.texcoord = v.texcoord;
				o.color = v.color * _Color;

				float4 tempVertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o, tempVertex);
	
				//smoothpixelshader stuff here
				#ifdef SMOOTHPIXEL
				float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
				float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
				o.texcoord = float4(v.texcoord.x, v.texcoord.y, maskUV.x, maskUV.y);
				#else
				o.texcoord = v.texcoord;
				#endif
				//end of smooth pixel
				/*
				#ifndef WATER_OFF

					//this mess fully removes the rotation from the matrix	
					float3 scale = float3(
						length(unity_ObjectToWorld._m00_m10_m20),
						length(unity_ObjectToWorld._m01_m11_m21),
						length(unity_ObjectToWorld._m02_m12_m22)
					);

					unity_ObjectToWorld._m00_m10_m20 = float3(scale.x, 0, 0);
					unity_ObjectToWorld._m01_m11_m21 = float3(0, scale.y, 0);
					unity_ObjectToWorld._m02_m12_m22 = float3(0, 0, scale.z);

					//build info needed for water line
					worldPos = mul(unity_ObjectToWorld, float4(pos.x, pos.y*1.5, 0, 1)).xyz; //fudge y sprite height 
					o.screenPos = ComputeScreenPos(o.vertex);
					o.worldPos = float4(pos.x, worldPos.y, 0, 0);
				#endif
				*/
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 env = 1 - ((1 - _RoDiffuseColor) * (1 - _RoAmbientColor));
				env = env * 0.5 + 0.5;
	
				//smoothpixel
				// apply anti-aliasing
				#ifdef SMOOTHPIXEL
				float2 texturePosition = i.texcoord * _MainTex_TexelSize.zw;
				float2 nearestBoundary = round(texturePosition);
				float2 delta = float2(abs(ddx(texturePosition.x)) + abs(ddx(texturePosition.y)),
					abs(ddy(texturePosition.x)) + abs(ddy(texturePosition.y)));
	
				float2 samplePosition = (texturePosition - nearestBoundary) / delta;
				samplePosition = clamp(samplePosition, -0.5, 0.5) + nearestBoundary;

				fixed4 diff = tex2D(_MainTex, samplePosition * _MainTex_TexelSize.xy);
				#else
				fixed4 diff = tex2D(_MainTex,i.texcoord.xy);
				#endif
				//endsmoothpixel


				fixed4 c = diff * i.color * float4(env.rgb,1);

				//if(c.a < 0.001)
				//	discard;
		
				UNITY_APPLY_FOG(i.fogCoord, c);
			
				c *= i.color;
				c.rgb *= c.a;

				//
				//
				// c = float4(i.color.r+0, i.color.g+0, i.color.b+0, 1);
				
	/*
				#ifndef WATER_OFF
					float2 uv = (i.screenPos.xy / i.screenPos.w);
					float4 water = tex2D(_WaterDepth, uv);
					float2 wateruv = TRANSFORM_TEX(water.xy, _WaterImageTexture);
	
					if (water.a < 0.1)
						return c;
	
					float4 waterTex = tex2D(_WaterImageTexture, wateruv);
					float height = water.z;
					
					waterTex = float4(0.5, 0.5, 0.5, 1) + (waterTex / 2);
	
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, waterTex);
	
					float simHeight = i.worldPos.y - abs(i.worldPos.x)/(_Width)*0.5;
	
					simHeight = clamp(simHeight, i.worldPos.y - 0.4, i.worldPos.y);
	
					if (height-0 > simHeight)
						c.rgb *= lerp(float3(1, 1, 1), waterTex.rgb, saturate(((height - 0) - simHeight) * 10));
					//c.rgb *= waterTex.rgb;
	
				#endif
	*/
				return c;
			}
		ENDCG
		}
	}
}
