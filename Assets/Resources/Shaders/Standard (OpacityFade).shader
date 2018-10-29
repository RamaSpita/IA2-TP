// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "shader_opasityfade"
{
	Properties
	{
		_Distance_Effect("Distance_Effect", Range( 0 , 10)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.01
		_Back_Face("Back_Face", 2D) = "black" {}
		_CutOut_Noise("CutOut_Noise", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_MetallicSmoothness("MetallicSmoothness", 2D) = "gray" {}
		_AO("AO", 2D) = "bump" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Radius("Radius", Float) = 5
		_FallOff("FallOff", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		ZWrite On
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			fixed ASEVFace : VFACE;
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Back_Face;
		uniform float4 _Back_Face_ST;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform sampler2D _CutOut_Noise;
		uniform float4 _CutOut_Noise_ST;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Distance_Effect;
		uniform float _Radius;
		uniform float _FallOff;
		uniform float _Cutoff = 0.01;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode46 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = tex2DNode46;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode42 = tex2D( _Albedo, uv_Albedo );
			float2 uv_Back_Face = i.uv_texcoord * _Back_Face_ST.xy + _Back_Face_ST.zw;
			float4 tex2DNode10 = tex2D( _Back_Face, uv_Back_Face );
			float4 switchResult41 = (((i.ASEVFace>0)?(tex2DNode42):(tex2DNode10)));
			o.Albedo = switchResult41.rgb;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode47 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			o.Metallic = tex2DNode47.r;
			float temp_output_51_0 = ( tex2DNode47.a * _Smoothness );
			o.Smoothness = temp_output_51_0;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			float4 tex2DNode49 = tex2D( _AO, uv_AO );
			o.Occlusion = tex2DNode49.r;
			o.Alpha = 1;
			float2 uv_CutOut_Noise = i.uv_texcoord * _CutOut_Noise_ST.xy + _CutOut_Noise_ST.zw;
			float4 tex2DNode58 = tex2D( _CutOut_Noise, uv_CutOut_Noise );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth7 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth7 = abs( ( screenDepth7 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Distance_Effect ) );
			float temp_output_68_0 = pow( ( distanceDepth7 / _Radius ) , _FallOff );
			float temp_output_74_0 = saturate( ( ( tex2DNode58.r * temp_output_68_0 ) + temp_output_68_0 ) );
			clip( temp_output_74_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
136;427;993;732;-6043.3;-412.4809;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;491.0425,943.3459;Float;False;Property;_Distance_Effect;Distance_Effect;0;0;Create;True;0;0;False;0;0;3.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;1670.869,1924.416;Float;False;Property;_Radius;Radius;13;0;Create;True;0;0;False;0;5;3.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;7;789.9948,945.7669;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1894.787,1953.409;Float;False;Property;_FallOff;FallOff;14;0;Create;True;0;0;False;0;1;3.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;67;1898.868,1629.416;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;68;2193.069,1652.46;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;1057.833,1081.494;Float;True;Property;_CutOut_Noise;CutOut_Noise;3;0;Create;True;0;0;False;0;None;79e39ab93ab3c0147b40fb373f88c4e0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;2339.776,1289.13;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;47;2989.112,646.9369;Float;True;Property;_MetallicSmoothness;MetallicSmoothness;7;0;Create;True;0;0;False;0;None;720135d7d1e3ca54ebf4753877ee5284;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;3289.448,802.8632;Float;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;1183.847,91.6083;Float;True;Property;_Albedo;Albedo;4;0;Create;True;0;0;False;0;None;60f13f83c6031cc408bd733927717b18;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;3279.003,1628.781;Float;True;Property;_Back_Face;Back_Face;2;0;Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;2615.536,1341.256;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;86;4316.62,1876.75;Float;False;Property;_Uses_BackFace_Fresnel;Uses_BackFace_Fresnel;12;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;739.8838,1054.135;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;53;3766.777,963.4504;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DistanceOpNode;66;1712.869,1526.416;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;3616.713,1237.015;Float;False;Property;_Gradient_Length;Gradient_Length;5;0;Create;True;0;0;False;0;0;0;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;64;1345.213,1676.953;Float;False;Property;_Camera;Camera;11;0;Create;True;0;0;False;0;0,0,0;0.2741983,1.535593,-1.380526;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;85;5613.117,1175.512;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;74;3097.037,1366.249;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;14;4238.419,922.5515;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;3561.006,707.9155;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;65;1370.869,1467.416;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;49;2985.297,845.2835;Float;True;Property;_AO;AO;8;0;Create;True;0;0;False;0;None;716f8e4ae75e10947b16f4516c4f49db;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;41;4711.015,1698.843;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwitchByFaceNode;56;4119.32,445.3764;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;3845.264,1891.169;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;3649.449,2060.449;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;3316.19,2122.152;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;3182.66,1999.334;Float;False;Property;_BackFace_Fresnel_Intencity;BackFace_Fresnel_Intencity;10;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;60;3044.47,2174.95;Float;False;Tangent;4;0;FLOAT3;0,0,1;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;3276.95,527.0463;Float;True;Property;_Normal;Normal;6;0;Create;True;0;0;False;0;None;d1b673e1129bd0b4696ef78cb994bada;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;88;4127.605,1994.769;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwitchByFaceNode;57;4511.092,138.8864;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;1026.031,935.8249;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;1456.328,958.275;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;55;3772.492,840.3418;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;4080.496,1062.186;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;3452.437,262.4738;Float;False;Constant;_Float3;Float 3;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;6725.35,591.8445;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;shader_opasityfade;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;True;Off;1;False;-1;0;False;-1;False;0;0;False;0;Custom;0.01;True;True;0;True;Transparent;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;-1;False;-1;-1;False;-1;0;False;0.005;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;5;0
WireConnection;67;0;7;0
WireConnection;67;1;69;0
WireConnection;68;0;67;0
WireConnection;68;1;70;0
WireConnection;72;0;58;1
WireConnection;72;1;68;0
WireConnection;73;0;72;0
WireConnection;73;1;68;0
WireConnection;86;0;10;0
WireConnection;86;1;88;0
WireConnection;53;0;49;0
WireConnection;53;1;54;0
WireConnection;66;0;65;0
WireConnection;66;1;64;0
WireConnection;85;0;42;0
WireConnection;85;2;74;0
WireConnection;74;0;73;0
WireConnection;14;0;39;0
WireConnection;51;0;47;4
WireConnection;51;1;52;0
WireConnection;41;0;42;0
WireConnection;41;1;10;0
WireConnection;56;0;47;1
WireConnection;56;1;54;0
WireConnection;87;0;10;0
WireConnection;87;1;62;0
WireConnection;62;0;63;0
WireConnection;62;1;59;0
WireConnection;59;0;60;0
WireConnection;60;0;46;0
WireConnection;88;0;87;0
WireConnection;57;0;46;0
WireConnection;57;1;54;0
WireConnection;11;0;7;0
WireConnection;11;1;12;0
WireConnection;13;0;11;0
WireConnection;13;1;58;1
WireConnection;55;0;51;0
WireConnection;55;1;54;0
WireConnection;39;0;13;0
WireConnection;39;1;34;0
WireConnection;0;0;41;0
WireConnection;0;1;46;0
WireConnection;0;3;47;1
WireConnection;0;4;51;0
WireConnection;0;5;49;0
WireConnection;0;10;74;0
ASEEND*/
//CHKSM=707A9A8486BB0A047CF0B1ECF4DAF44EBC1FC116