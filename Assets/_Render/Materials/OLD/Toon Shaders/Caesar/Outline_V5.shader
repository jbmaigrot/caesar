// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

Shader "Toony Colors Pro 2/Caesar/Outline_V5"
{
	Properties
	{
	[TCP2Separator]
	[Header(Base)]
	[Space]
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}

	[TCP2Separator]
	[Header(Toony Colors Pro 2)]
	[Space]
		//Highlight/Shadow colors
		[HDR] _HColor("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor("Shadow Color", Color) = (0.195,0.195,0.195,1.0)
	[Space]
		//Ramp shading
		_RampThreshold("Ramp Threshold", Range(0,1)) = 0.75
		_RampSmoothness("Ramp Smoothness", Range(0,1)) = 0.01		

	[TCP2Separator]
	[Header(Specular)]
	[Space]
		//Specular
		_Shininess("Shininess", Range(0.01, 10.0)) = 0.5
		[HDR] _SpecColor("Specular Color", Color) = (1.0, 1.0, 1.0)

	[TCP2Separator]
	[Header(Outline)]
	[Space]
		//OUTLINE
		[HDR] _OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1.0)
		_Outline ("Outline Width", Float) = 1

		//Outline Textured
		[Toggle(TCP2_OUTLINE_TEXTURED)] _EnableTexturedOutline ("Color from Texture", Float) = 0
		[TCP2KeywordFilter(TCP2_OUTLINE_TEXTURED)] _TexLod ("Texture LOD", Range(0,10)) = 5

		//Constant-size outline
		[Toggle(TCP2_OUTLINE_CONST_SIZE)] _EnableConstSizeOutline ("Constant Size Outline", Float) = 0

		//Z Offset
		_Offset1 ("Depth Offset Slope", Float) = 0
		_Offset2 ("Depth Offset Distance", Float) = 0

		//This property will be ignored and will draw the custom normals GUI instead
		[TCP2OutlineNormalsGUI] __outline_gui_dummy__ ("_unused_", Float) = 0
		_StencilRef ("Stencil Outline Group", Range(0,255)) = 1

		// Blending state & alpha cutoff
		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "LightweightPipeline"
			"IgnoreProjector" = "True"
		}
		LOD 300

		//================================================================
		// OUTLINE INCLUDE

		HLSLINCLUDE
  #if defined(TCP2_OUTLINE_INCLUDE)

		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"

		struct a2v
		{
			float4 vertex : POSITION;
			float3 normalOS : NORMAL;
	#if TCP2_OUTLINE_TEXTURED
			float3 texcoord : TEXCOORD0;
	#endif
	#if TCP2_COLORS_AS_NORMALS
			float4 color : COLOR;
	#endif
	#if TCP2_UV2_AS_NORMALS
			float2 uv2 : TEXCOORD1;
	#endif
	#if TCP2_TANGENT_AS_NORMALS
			float4 tangent : TANGENT;
	#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
	#if TCP2_OUTLINE_TEXTURED
			float3 texlod : TEXCOORD1;
	#endif
		};

		float _Outline;
		half4 _OutlineColor;

	#if TCP2_OUTLINE_TEXTURED
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _TexLod;
	#endif

		#define OUTLINE_WIDTH(vertex) _Outline


		#ifdef TCP2_COLORS_AS_NORMALS
			#define GET_OUTLINE_NORMAL(vertex)	float3 normal = (vertex.color.xyz*2) - 1;
		#elif TCP2_TANGENT_AS_NORMALS
			#define GET_OUTLINE_NORMAL(vertex)	float3 normal = vertex.tangent.xyz;
		#elif TCP2_UV2_AS_NORMALS
			#define GET_OUTLINE_NORMAL(vertex)	\
				float3 n; \
				vertex.uv2.x = vertex.uv2.x * 255.0/16.0; \
				n.x = floor(vertex.uv2.x) / 15.0; \
				n.y = frac(vertex.uv2.x) * 16.0 / 15.0; \
				n.z = vertex.uv2.y; \
				n = n*2 - 1; \
				float3 normal = n;
		#else
			#define GET_OUTLINE_NORMAL(vertex)	float3 normal = vertex.normalOS;
		#endif


	#ifdef TCP2_OUTLINE_CONST_SIZE
			//Camera-independent outline size
		#define OUTLINE_SIZE	distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex))
	#else
		#define OUTLINE_SIZE	1.0
	#endif

		v2f TCP2_Outline_Vert(a2v v)
		{
			v2f o;

			//GPU instancing support
			UNITY_SETUP_INSTANCE_ID(v);

			GET_OUTLINE_NORMAL(v)
			o.pos = TransformObjectToHClip(v.vertex.xyz + normal.xyz * OUTLINE_WIDTH(v) * 0.01 * OUTLINE_SIZE);

	#if TCP2_OUTLINE_TEXTURED
			half2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.texlod = tex2Dlod(_MainTex, float4(uv, 0, _TexLod)).rgb;
	#endif

			return o;
		}

		#define OUTLINE_COLOR _OutlineColor

		float4 TCP2_Outline_Frag (v2f IN) : SV_Target
		{
	#if TCP2_OUTLINE_TEXTURED
			return float4(IN.texlod, 1) * OUTLINE_COLOR;
	#else
			return OUTLINE_COLOR;
	#endif
		}

  #endif	//TCP2_OUTLINE_INCLUDE
		ENDHLSL

		// OUTLINE INCLUDE END
		//================================================================

		Stencil
		{
			Ref [_StencilRef]
			Comp Always
			Pass Replace
		}

		Pass
		{
			Name "ForwardLit"
			Tags { "LightMode" = "LightweightForward" }

			// Use same blending / depth states as Standard shader
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
			Cull[_Cull]

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			//--------------------------------------
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON
			//#pragma shader_feature _ _SPECGLOSSMAP _SPECULAR_COLOR
			//#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA
			//#pragma shader_feature _NORMALMAP
			//#pragma shader_feature _EMISSION
			//#pragma shader_feature _RECEIVE_SHADOWS_OFF

			//--------------------------------------
			// Lightweight Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			//--------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex LitPassVertexSimple
			#pragma fragment LitPassFragmentSimple

			#define _SPECULAR_COLOR 1					//Enable specular color

			// SimpleLitInput.hlsl replacement

			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			half4 _Color;
			half4 _SpecColor;
			half4 _EmissionColor;
			half _Cutoff;
			half _Shininess;
			
			// TCP2 Variables
			half4 _HColor;
			half4 _SColor;
			half _RampThreshold;
			half _RampSmoothness;
			CBUFFER_END

			TEXTURE2D(_SpecGlossMap);       SAMPLER(sampler_SpecGlossMap);

			half4 SampleSpecularGloss(half2 uv, half alpha, half4 specColor, TEXTURE2D_ARGS(specGlossMap, sampler_specGlossMap))
			{
				half4 specularGloss = half4(0.0h, 0.0h, 0.0h, 1.0h);
			#ifdef _SPECGLOSSMAP
				specularGloss = SAMPLE_TEXTURE2D(specGlossMap, sampler_specGlossMap, uv);
			#elif defined(_SPECULAR_COLOR)
				specularGloss = specColor;
			#endif

			#ifdef _GLOSSINESS_FROM_BASE_ALPHA
				specularGloss.a = alpha;
			#endif
				return specularGloss;
			}
			//SimpleLitInput.hlsl end
			//--------------------------------------
			// SimpleLitForwardPass.hlsl

			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"

			struct Attributes
			{
				float4 positionOS    : POSITION;
				float3 normalOS      : NORMAL;
				float4 tangentOS     : TANGENT;
				float2 texcoord      : TEXCOORD0;
				float2 lightmapUV    : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float2 uv						: TEXCOORD0;
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

				float4 posWSShininess			: TEXCOORD2;    // xyz: posWS, w: Shininess * 128

			#ifdef _NORMALMAP
				half4 normal					: TEXCOORD3;    // xyz: normal, w: viewDir.x
				half4 tangent					: TEXCOORD4;    // xyz: tangent, w: viewDir.y
				half4 bitangent					: TEXCOORD5;    // xyz: bitangent, w: viewDir.z
			#else
				half3  normal					: TEXCOORD3;
				half3 viewDir					: TEXCOORD4;
			#endif

				half4 fogFactorAndVertexLight	: TEXCOORD6; // x: fogFactor, yzw: vertex light

			#ifdef _MAIN_LIGHT_SHADOWS
				float4 shadowCoord				: TEXCOORD7;
			#endif

				float4 positionCS				: SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			//--------------------------------------
			//TCP2 Lighting functions
			half3 LightingRamp(half3 lightColor, half3 lightDir, half3 normal, half attenuation, half rampThreshold, half rampSmoothness, out half NdotL)
			{
				NdotL = dot(normal, lightDir);
				half wrappedNdotL = max(0, NdotL * 0.5 + 0.5);
				NdotL = max(0, NdotL);
				half3 ramp = smoothstep(rampThreshold - rampSmoothness * 0.5, rampThreshold + rampSmoothness * 0.5, wrappedNdotL).xxx;
				return lightColor * attenuation * ramp;
			}

			half3 LightingRampColor(half3 lightColor, half3 lightDir, half3 normal, half attenuation, half rampThreshold, half rampSmoothness, half4 highlightColor, half4 shadowColor, out half NdotL)
			{
				NdotL = dot(normal, lightDir);
				half wrappedNdotL = max(0, NdotL * 0.5 + 0.5);
				NdotL = max(0, NdotL);
				half3 ramp = smoothstep(rampThreshold - rampSmoothness * 0.5, rampThreshold + rampSmoothness * 0.5, wrappedNdotL).xxx;
				ramp = lerp(shadowColor.rgb, highlightColor.rgb, ramp * attenuation);
				return lightColor * ramp;
			}

			half3 LightingSpecularStylized(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir, half4 specularGloss, half shininess)
			{
				half3 halfVec = SafeNormalize(lightDir + viewDir);
				half NdotH = saturate(dot(normal, halfVec));
				half modifier = pow(NdotH, shininess) * specularGloss.a;
				half3 specularReflection = specularGloss.rgb * modifier;
				return lightColor * specularReflection;
			}

			// For some reason, BUMP_SCALE_NOT_SUPPORTED doesn't work properly so we redefine this function here
			half3 SampleNormal_TCP2(float2 uv, TEXTURE2D_ARGS(bumpMap, sampler_bumpMap))
			{
			#ifdef _NORMALMAP
				half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
					return UnpackNormal(n);
			#else
				return half3(0.0h, 0.0h, 1.0h);
			#endif
			}

			void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
			{
				inputData.positionWS = input.posWSShininess.xyz;

			#ifdef _NORMALMAP
				half3 viewDirWS = half3(input.normal.w, input.tangent.w, input.bitangent.w);
				inputData.normalWS = TransformTangentToWorld(normalTS,
					half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz));
			#else
				half3 viewDirWS = input.viewDir;
				inputData.normalWS = input.normal;
			#endif

			#if SHADER_HINT_NICE_QUALITY
				viewDirWS = SafeNormalize(viewDirWS);
			#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);

				inputData.viewDirectionWS = viewDirWS;
			#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
				inputData.shadowCoord = input.shadowCoord;
			#else
				inputData.shadowCoord = float4(0, 0, 0, 0);
			#endif
				inputData.fogCoord = input.fogFactorAndVertexLight.x;
				inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
			}

			//===========================================================================//
			//                  Vertex and Fragment functions                            //
			//===========================================================================//

			// Used in Standard (Simple Lighting) shader
			Varyings LitPassVertexSimple(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
				half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;

			#if !SHADER_HINT_NICE_QUALITY
				viewDirWS = SafeNormalize(viewDirWS);
			#endif

				half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
				half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

				output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.posWSShininess.xyz = vertexInput.positionWS;
				output.posWSShininess.w = _Shininess * 128.0;
				output.positionCS = vertexInput.positionCS;

			#ifdef _NORMALMAP
				output.normal = half4(normalInput.normalWS, viewDirWS.x);
				output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
				output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);
			#else
				output.normal = normalInput.normalWS;
				output.viewDir = viewDirWS;
			#endif

				OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
				OUTPUT_SH(output.normal.xyz, output.vertexSH);

				output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

			#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif

				return output;
			}

			// from Lighting.hlsl
			half4 LightweightFragmentTCP2(InputData inputData, half3 diffuse, half4 specularGloss, half shininess, half3 emission, half alpha)
			{
				Light mainLight = GetMainLight(inputData.shadowCoord);
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

				half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
				//half3 diffuseColor = inputData.bakedGI + LightingLambert(attenuatedLightColor, mainLight.direction, inputData.normalWS);
				half NdotL;
				half3 diffuseColor = inputData.bakedGI + LightingRampColor(mainLight.color, mainLight.direction, inputData.normalWS, mainLight.distanceAttenuation * mainLight.shadowAttenuation, _RampThreshold, _RampSmoothness, _HColor, _SColor, NdotL);
				half3 specularColor = LightingSpecular(attenuatedLightColor, mainLight.direction, inputData.normalWS, inputData.viewDirectionWS, specularGloss, shininess);

			#ifdef _ADDITIONAL_LIGHTS
				int pixelLightCount = GetAdditionalLightsCount();
				for (int i = 0; i < pixelLightCount; ++i)
				{
					Light light = GetAdditionalLight(i, inputData.positionWS);
					half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
					//diffuseColor += LightingLambert(attenuatedLightColor, light.direction, inputData.normalWS);
					diffuseColor += LightingRamp(attenuatedLightColor, light.direction, inputData.normalWS, 1, _RampThreshold, _RampSmoothness, NdotL);
					specularColor += LightingSpecular(attenuatedLightColor, light.direction, inputData.normalWS, inputData.viewDirectionWS, specularGloss, shininess);
				}
			#endif

			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				diffuseColor += inputData.vertexLighting;
			#endif

				half3 finalColor = diffuseColor * diffuse + emission;

			#if defined(_SPECGLOSSMAP) || defined(_SPECULAR_COLOR)
				finalColor += specularColor;
			#endif

				return half4(finalColor, alpha);
			}

			// Used for StandardSimpleLighting shader
			half4 LitPassFragmentSimple(Varyings input
									   ) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = input.uv;
				half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_PARAM(_MainTex, sampler_MainTex));
				half3 diffuse = diffuseAlpha.rgb * _Color.rgb;

				half alpha = diffuseAlpha.a * _Color.a;
				AlphaDiscard(alpha, _Cutoff);
			#ifdef _ALPHAPREMULTIPLY_ON
				diffuse *= alpha;
			#endif

				half3 normalTS = SampleNormal_TCP2(uv, TEXTURE2D_PARAM(_BumpMap, sampler_BumpMap));
				half3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_PARAM(_EmissionMap, sampler_EmissionMap));
				half4 specularGloss = SampleSpecularGloss(uv, diffuseAlpha.a, _SpecColor, TEXTURE2D_PARAM(_SpecGlossMap, sampler_SpecGlossMap));
				half shininess = input.posWSShininess.w;

				InputData inputData;
				InitializeInputData(input, normalTS, inputData);

				half4 color = LightweightFragmentTCP2(inputData, diffuse, specularGloss, shininess, emission, alpha);
				color.rgb = MixFog(color.rgb, inputData.fogCoord);
				return color;
			};
			// SimpleLitForwardPass.hlsl

			ENDHLSL
		}

		//Outline
		Pass
		{
			Cull Front
			Offset [_Offset1],[_Offset2]

			Stencil
			{
				Ref [_StencilRef]
				Comp NotEqual
				Pass Keep
			}

			HLSLPROGRAM
			#pragma target 3.0

			//Hack to force the keyword to be set even though HLSLINCLUDE occured before this block (#define doesn't work in that case)
			#pragma multi_compile TCP2_OUTLINE_INCLUDE

			#pragma vertex TCP2_Outline_Vert
			#pragma fragment TCP2_Outline_Frag

			//--------------------------------------
			// TCP2 Outline keywords
			#pragma multi_compile _ TCP2_OUTLINE_CONST_SIZE
			#pragma multi_compile _ TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			#pragma multi_compile _ TCP2_OUTLINE_TEXTURED

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags{"LightMode" = "ShadowCaster"}

			ZWrite On
			ZTest LEqual
			Cull[_Cull]

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
			//#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma multi_compile TCP2_OUTLINE_INCLUDE
			#pragma multi_compile _ TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS

			#pragma vertex ShadowPassVertex_TCP2
			#pragma fragment ShadowPassFragment

			#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/Shaders/ShadowCasterPass.hlsl"

			//Take outline into account
			struct AttributesOutline
			{
				float3 normalOS : NORMAL;
		#if TCP2_COLORS_AS_NORMALS
				float4 color : COLOR;
		#endif
		#if TCP2_UV2_AS_NORMALS
				float2 uv2 : TEXCOORD1;
		#endif
		#if TCP2_TANGENT_AS_NORMALS
				float4 tangent : TANGENT;
		#endif
			};
			
			Varyings ShadowPassVertex_TCP2(Attributes input, AttributesOutline outlineInput)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
				GET_OUTLINE_NORMAL(outlineInput)
				input.positionOS.xyz += normal.xyz * OUTLINE_WIDTH(outlineInput) * 0.01;
				output.positionCS = GetShadowPositionHClip(input);
				return output;
			}

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0
			Cull[_Cull]

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			#pragma vertex DepthOnlyVertex_TCP2
			#pragma fragment DepthOnlyFragment

			//--------------------------------------
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
			//#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA
		
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			
			#pragma multi_compile TCP2_OUTLINE_INCLUDE
			#pragma multi_compile _ TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS

			#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/Shaders/DepthOnlyPass.hlsl"
			
			//Take outline into account
			struct AttributesOutline
			{
				float3 normalOS : NORMAL;
		#if TCP2_COLORS_AS_NORMALS
				float4 color : COLOR;
		#endif
		#if TCP2_UV2_AS_NORMALS
				float2 uv2 : TEXCOORD1;
		#endif
		#if TCP2_TANGENT_AS_NORMALS
				float4 tangent : TANGENT;
		#endif
			};
			
			Varyings DepthOnlyVertex_TCP2(Attributes input, AttributesOutline outlineInput)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
				GET_OUTLINE_NORMAL(outlineInput)
				input.position.xyz += normal.xyz * OUTLINE_WIDTH(outlineInput) * 0.01;
				output.positionCS = TransformObjectToHClip(input.position.xyz);
				return output;
			}

			ENDHLSL
		}		

		// This pass it not used during regular rendering, only for lightmap baking.
		Pass
		{
			Name "Meta"
			Tags{ "LightMode" = "Meta" }

			Cull Off

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma vertex LightweightVertexMeta
			#pragma fragment LightweightFragmentMetaSimple

			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP

			#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitMetaPass.hlsl"

			ENDHLSL
		}
	}

	Fallback "Hidden/InternalErrorShader"
	CustomEditor "TCP2_MaterialInspector_LWRP_SG"
}
