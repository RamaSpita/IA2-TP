// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "shader_opasityfade_prototype"
{
	Properties
	{
		_Distance_Effect("Distance_Effect", Range( 0 , 10)) = 0
		_Back_Face("Back_Face", 2D) = "black" {}
		_Cutoff( "Mask Clip Value", Float ) = 1
		_CutOut_Noise("CutOut_Noise", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_Float2("Float 2", Range( 0 , 50)) = 0
		_Normal("Normal", 2D) = "bump" {}
		_MetallicSmoothness("MetallicSmoothness", 2D) = "gray" {}
		_AO("AO", 2D) = "bump" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_BackFace_Fresnel_Intencity("BackFace_Fresnel_Intencity", Range( 0 , 1)) = 0.5
		_Radius("Radius", Float) = 5
		_FallOff("FallOff", Float) = 1
		_NormalFresnelStrenght("Normal Fresnel Strenght", Float) = 0
		_GhostColor("Ghost Color", Color) = (1,1,1,0)
		_Distance_EffectProto("Distance_EffectProto", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			fixed ASEVFace : VFACE;
			float3 worldPos;
			INTERNAL_DATA
			float3 worldNormal;
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Back_Face;
		uniform float4 _Back_Face_ST;
		uniform float4 _GhostColor;
		uniform float _BackFace_Fresnel_Intencity;
		uniform float _NormalFresnelStrenght;
		uniform sampler2D _CutOut_Noise;
		uniform float4 _CutOut_Noise_ST;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Distance_Effect;
		uniform float _Radius;
		uniform float _FallOff;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _Distance_EffectProto;
		uniform float _Float2;
		uniform float _Cutoff = 1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode46 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = tex2DNode46;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode42 = tex2D( _Albedo, uv_Albedo );
			float2 uv_Back_Face = i.uv_texcoord * _Back_Face_ST.xy + _Back_Face_ST.zw;
			float4 switchResult41 = (((i.ASEVFace>0)?(tex2DNode42):(tex2D( _Back_Face, uv_Back_Face ))));
			o.Albedo = switchResult41.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 temp_output_92_0 = ( tex2DNode46 * _NormalFresnelStrenght );
			float fresnelNDotV60 = dot( WorldNormalVector( i , temp_output_92_0 ), ase_worldViewDir );
			float fresnelNode60 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNDotV60, 5.0 ) );
			float temp_output_91_0 = saturate( ( _BackFace_Fresnel_Intencity * fresnelNode60 ) );
			float2 uv_CutOut_Noise = i.uv_texcoord * _CutOut_Noise_ST.xy + _CutOut_Noise_ST.zw;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth7 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth7 = abs( ( screenDepth7 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Distance_Effect ) );
			float temp_output_68_0 = pow( ( distanceDepth7 / _Radius ) , _FallOff );
			float temp_output_74_0 = saturate( ( ( tex2D( _CutOut_Noise, uv_CutOut_Noise ).r * temp_output_68_0 ) + temp_output_68_0 ) );
			float lerpResult103 = lerp( 0.0 , temp_output_91_0 , ( 1.0 - temp_output_74_0 ));
			o.Emission = ( _GhostColor * lerpResult103 ).rgb;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode47 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			o.Metallic = tex2DNode47.r;
			o.Smoothness = ( tex2DNode47.a * _Smoothness );
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			float screenDepth108 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth108 = abs( ( screenDepth108 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Distance_EffectProto ) );
			float4 lerpResult85 = lerp( tex2DNode42 , float4( 0,0,0,0 ) , temp_output_74_0);
			o.Alpha = saturate( ( ( ( distanceDepth108 + -2.0 ) + lerpResult85 ) / _Float2 ) ).r;
			clip( saturate( ( temp_output_91_0 + temp_output_74_0 ) ) - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
2956;620;1906;1012;-4131.096;206.4527;2.406354;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;491.0425,943.3459;Float;False;Property;_Distance_Effect;Distance_Effect;0;0;Create;True;0;0;False;0;0;3.63;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;1670.869,1924.416;Float;False;Property;_Radius;Radius;11;0;Create;True;0;0;False;0;5;1.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;7;789.9948,945.7669;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;67;1898.868,1629.416;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1894.787,1953.409;Float;False;Property;_FallOff;FallOff;12;0;Create;True;0;0;False;0;1;1.63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;1057.833,1081.494;Float;True;Property;_CutOut_Noise;CutOut_Noise;3;0;Create;True;0;0;False;0;None;5bd2ed48276ce514eb6bc5178da2ebd7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;68;2193.069,1652.46;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;1761.285,898.0859;Float;True;Property;_Normal;Normal;6;0;Create;True;0;0;False;0;None;b1c6e3a1b8e7ea849bf8f663457e56be;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;93;2229.948,2218.575;Float;False;Property;_NormalFresnelStrenght;Normal Fresnel Strenght;13;0;Create;True;0;0;False;0;0;0.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;2339.776,1289.13;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;2611.95,2237.176;Float;False;2;2;0;FLOAT3;1,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;2615.536,1341.256;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;5619.61,235.6034;Float;False;Property;_Distance_EffectProto;Distance_EffectProto;15;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;108;5847.94,244.4388;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;5886.36,317.5573;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;1615.018,581.67;Float;True;Property;_Albedo;Albedo;4;0;Create;True;0;0;False;0;None;db1053624b07af74cb4a573f6f3565d1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;74;3097.037,1366.249;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;60;2848.915,2304.685;Float;False;Tangent;4;0;FLOAT3;0,0,1;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;3182.66,1999.334;Float;False;Property;_BackFace_Fresnel_Intencity;BackFace_Fresnel_Intencity;10;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;3649.449,2060.449;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;85;5852.085,397.973;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;6045.028,277.5516;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;111;6187.137,278.5629;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;114;6122.821,459.5609;Float;False;Property;_Float2;Float 2;5;0;Create;True;0;0;False;0;0;0;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;104;5255.665,1113.906;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;3832.405,2170.002;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;106;5613.831,772.4047;Float;False;Property;_GhostColor;Ghost Color;14;0;Create;True;0;0;False;0;1,1,1,0;0.2782222,0.3750164,0.4558824,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;47;2989.112,646.9369;Float;True;Property;_MetallicSmoothness;MetallicSmoothness;7;0;Create;True;0;0;False;0;None;440938269cbfefa4986bfb6c92946417;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;6376.204,293.2903;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;6069.332,1142.517;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;3279.003,1628.781;Float;True;Property;_Back_Face;Back_Face;1;0;Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;3289.448,802.8632;Float;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;103;5525.631,994.058;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;113;6532.787,290.6703;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwitchByFaceNode;41;4711.015,1698.843;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;6244.891,698.7448;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;90;6317.943,1025.223;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;49;2985.297,845.2835;Float;True;Property;_AO;AO;8;0;Create;True;0;0;False;0;None;9593e4882e75ab7418473eaaf9ed9378;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;95;2202.962,1972.853;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;94;2939.948,2026.575;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;3561.006,707.9155;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;6962.893,577.5558;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;shader_opasityfade_prototype;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;Back;1;False;-1;0;False;-1;False;0;0;False;0;Custom;1;True;True;0;True;Opaque;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;-1;False;-1;-1;False;-1;0;False;0.005;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;5;0
WireConnection;67;0;7;0
WireConnection;67;1;69;0
WireConnection;68;0;67;0
WireConnection;68;1;70;0
WireConnection;72;0;58;1
WireConnection;72;1;68;0
WireConnection;92;0;46;0
WireConnection;92;1;93;0
WireConnection;73;0;72;0
WireConnection;73;1;68;0
WireConnection;108;0;107;0
WireConnection;74;0;73;0
WireConnection;60;0;92;0
WireConnection;62;0;63;0
WireConnection;62;1;60;0
WireConnection;85;0;42;0
WireConnection;85;2;74;0
WireConnection;110;0;108;0
WireConnection;110;1;109;0
WireConnection;111;0;110;0
WireConnection;111;1;85;0
WireConnection;104;0;74;0
WireConnection;91;0;62;0
WireConnection;112;0;111;0
WireConnection;112;1;114;0
WireConnection;89;0;91;0
WireConnection;89;1;74;0
WireConnection;103;1;91;0
WireConnection;103;2;104;0
WireConnection;113;0;112;0
WireConnection;41;0;42;0
WireConnection;41;1;10;0
WireConnection;105;0;106;0
WireConnection;105;1;103;0
WireConnection;90;0;89;0
WireConnection;94;0;95;0
WireConnection;94;1;92;0
WireConnection;51;0;47;4
WireConnection;51;1;52;0
WireConnection;0;0;41;0
WireConnection;0;1;46;0
WireConnection;0;2;105;0
WireConnection;0;3;47;1
WireConnection;0;4;51;0
WireConnection;0;5;49;0
WireConnection;0;9;113;0
WireConnection;0;10;90;0
ASEEND*/
//CHKSM=60DE06823807E8E98E9A5B958E7D28CF787C167C