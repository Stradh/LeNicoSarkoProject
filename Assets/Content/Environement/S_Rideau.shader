// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "S_Rideau"
{
	Properties
	{
		_Trans("Trans", Range( 0 , 1)) = 0.5
		_Color0("Color 0", Color) = (0.5686275,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "bump" {}
		_Normal_Scale("Normal_Scale", Range( 0 , 10)) = 1
		_Bias("Bias", Range( 0 , 10)) = 0
		_Scale("Scale", Range( 0 , 30)) = 3
		_Power("Power", Range( 0 , 50)) = 5
		_Max_Trans("Max_Trans", Range( 0 , 1)) = 0.8
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 4.0
		#pragma surface surf Standard alpha:fade keepalpha 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Normal_Scale;
		uniform float4 _Color0;
		uniform float _Bias;
		uniform float _Scale;
		uniform float _Power;
		uniform float _Trans;
		uniform float _Max_Trans;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _TextureSample0, uv_TextureSample0 ), _Normal_Scale );
			o.Albedo = _Color0.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( _Bias + _Scale * pow( 1.0 - fresnelNdotV1, _Power ) );
			float clampResult15 = clamp( ( fresnelNode1 + _Trans ) , 0.0 , _Max_Trans );
			o.Alpha = clampResult15;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
254;264;1920;1005;1680;1653.973;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;18;-1414.132,43.99219;Inherit;False;Property;_Scale;Scale;7;0;Create;True;0;0;0;False;0;False;3;3;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1416.132,120.9922;Inherit;False;Property;_Power;Power;8;0;Create;True;0;0;0;False;0;False;5;5;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1417.132,-33.00781;Inherit;False;Property;_Bias;Bias;6;0;Create;True;0;0;0;False;0;False;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-978.8483,305.5712;Inherit;False;Property;_Trans;Trans;0;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1;-970.8486,62.1712;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-568.1807,225.9471;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-819.0042,488.7209;Inherit;False;Property;_Max_Trans;Max_Trans;9;0;Create;True;0;0;0;False;0;False;0.8;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1299.397,-218.0239;Inherit;False;Property;_Normal_Scale;Normal_Scale;5;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;15;-315.9727,227.1515;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-920.8876,-1162.472;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;0.1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-975.7112,-216.4558;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;a53cf5449d11a15d1100a04b44295342;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;14;-179.8989,-1117.703;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-468.2042,-1145.57;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-484.7039,-875.1698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-882.9879,-887.2711;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0.4;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;13;-201.5038,-895.9708;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-866.2485,-1385.829;Inherit;False;Property;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;0.5686275,0,0,0;0.5686275,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;276.1609,-57.19308;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;S_Rideau;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;1;17;0
WireConnection;1;2;18;0
WireConnection;1;3;19;0
WireConnection;6;0;1;0
WireConnection;6;1;3;0
WireConnection;15;0;6;0
WireConnection;15;2;20;0
WireConnection;7;5;16;0
WireConnection;14;0;11;0
WireConnection;11;0;8;0
WireConnection;11;1;1;0
WireConnection;10;0;9;0
WireConnection;10;1;1;0
WireConnection;13;0;10;0
WireConnection;0;0;2;0
WireConnection;0;1;7;0
WireConnection;0;9;15;0
ASEEND*/
//CHKSM=FF3C5CA20A586EEFB16E890E0622ED91590961DD