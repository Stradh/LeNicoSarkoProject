// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "StochasticExample"
{
	Properties
	{
		_AlbedoHeight("AlbedoHeight", 2D) = "white" {}
		_SmoothnessAO("SmoothnessAO", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Contrast("Contrast", Range( 0.01 , 1)) = 0.15
		_Scale("Scale", Range( 0.5 , 1.5)) = 1
		[Toggle(_USESTOCHASTIC_ON)] _useStochastic("useStochastic", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Assets/AmplifyStochasticNode/StochasticSampling.cginc"
		#pragma target 4.6
		#pragma shader_feature _USESTOCHASTIC_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform sampler2D _AlbedoHeight;
		uniform float _Scale;
		uniform float _Contrast;
		uniform sampler2D _SmoothnessAO;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord4 = i.uv_texcoord * float2( 10,10 );
			half3 cw3 = 0;
			float2 uv13 = 0;
			float2 uv23 = 0;
			float2 uv33 = 0;
			float2 dx3 = 0;
			float2 dy3 = 0;
			half4 stochasticSample3 = StochasticSample2DWeightsA(_AlbedoHeight,uv_TexCoord4,cw3,uv13,uv23,uv33,dx3,dy3,_Scale,_Contrast);
			half3 cw5 = cw3;
			float2 uv15 = uv13;
			float2 uv25 = uv23;
			float2 uv35 = uv33;
			float2 dx5 = dx3;
			float2 dy5 = dy3;
			half4  stochasticSample5 = half4(UnpackNormal(StochasticSample2D(_Normal,cw3,uv13,uv23,uv33,dx3,dy3)), 1);
			#ifdef _USESTOCHASTIC_ON
				float4 staticSwitch31 = stochasticSample5;
			#else
				float4 staticSwitch31 = float4( UnpackNormal( tex2D( _Normal, uv_TexCoord4 ) ) , 0.0 );
			#endif
			o.Normal = staticSwitch31.xyz;
			#ifdef _USESTOCHASTIC_ON
				float4 staticSwitch30 = stochasticSample3;
			#else
				float4 staticSwitch30 = tex2D( _AlbedoHeight, uv_TexCoord4 );
			#endif
			o.Albedo = staticSwitch30.rgb;
			half3 cw9 = cw5;
			float2 uv19 = uv15;
			float2 uv29 = uv25;
			float2 uv39 = uv35;
			float2 dx9 = dx5;
			float2 dy9 = dy5;
			half4  stochasticSample9 = StochasticSample2D(_SmoothnessAO,cw5,uv15,uv25,uv35,dx5,dy5);
			#ifdef _USESTOCHASTIC_ON
				float4 staticSwitch33 = stochasticSample9;
			#else
				float4 staticSwitch33 = tex2D( _SmoothnessAO, uv_TexCoord4 );
			#endif
			float4 break35 = staticSwitch33;
			o.Smoothness = break35.g;
			o.Occlusion = break35.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
168;128;1450;782;1294.658;1038.994;1.28573;False;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1314.76,-720.0712;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;10,10;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-588.6688,250.9637;Float;False;Property;_Contrast;Contrast;3;0;Create;True;0;0;False;0;0.15;0.19;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-774.7231,-662.7855;Float;True;Property;_AlbedoHeight;AlbedoHeight;0;0;Create;True;0;0;False;0;880a75ad4e4124da78f400820a0f9f9a;880a75ad4e4124da78f400820a0f9f9a;False;white;LockedToTexture2D;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-577.8066,169.599;Float;False;Property;_Scale;Scale;4;0;Create;True;0;0;False;0;1;1.05;0.5;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;13;-746.4445,-456.6097;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;ead9df3cf7f8140bbaac1d868aa48695;ead9df3cf7f8140bbaac1d868aa48695;True;bump;LockedToTexture2D;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.AmplifyStochasticNode;3;-227.9221,-37.9369;Float;False;3;1;0.15;False;5;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;2;FLOAT;1;False;3;FLOAT;0.15;False;4;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;6;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT4x4;5
Node;AmplifyShaderEditor.TexturePropertyNode;10;-744.3207,-215.2426;Float;True;Property;_SmoothnessAO;SmoothnessAO;1;0;Create;True;0;0;False;0;9a8dd88696a6e4787b38a3fe818ac0f6;9a8dd88696a6e4787b38a3fe818ac0f6;False;white;LockedToTexture2D;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.AmplifyStochasticNode;5;28.8089,151.4425;Float;False;4;1;0.15;True;5;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;2;FLOAT;1;False;3;FLOAT;0.15;False;4;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;6;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT4x4;5
Node;AmplifyShaderEditor.SamplerNode;34;-19.78601,-302.6701;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AmplifyStochasticNode;9;354.4239,354.7065;Float;False;4;1;0.15;False;5;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;2;FLOAT;1;False;3;FLOAT;0.15;False;4;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;6;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT4x4;5
Node;AmplifyShaderEditor.StaticSwitch;33;612.9677,-196.8079;Float;False;Property;_useStochastic;useStochastic;5;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;29;-9.860748,-679.6432;Float;True;Property;_defaultContrastTextureSample0;defaultContrastTexture Sample 0;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;32;-9.517509,-477.6432;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;30;602.6922,-512.3836;Float;False;Property;_useStochastic;useStochastic;5;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;31;592.787,-403.0405;Float;False;Property;_useStochastic;useStochastic;6;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;35;856.9058,-207.7165;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;28;1169.604,-513.3189;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;StochasticExample;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;4;0
WireConnection;3;1;2;0
WireConnection;3;2;16;0
WireConnection;3;3;15;0
WireConnection;5;0;4;0
WireConnection;5;1;13;0
WireConnection;5;4;3;5
WireConnection;34;0;10;0
WireConnection;34;1;4;0
WireConnection;9;0;4;0
WireConnection;9;1;10;0
WireConnection;9;4;5;5
WireConnection;33;1;34;0
WireConnection;33;0;9;0
WireConnection;29;0;2;0
WireConnection;29;1;4;0
WireConnection;32;0;13;0
WireConnection;32;1;4;0
WireConnection;30;1;29;0
WireConnection;30;0;3;0
WireConnection;31;1;32;0
WireConnection;31;0;5;0
WireConnection;35;0;33;0
WireConnection;28;0;30;0
WireConnection;28;1;31;0
WireConnection;28;4;35;1
WireConnection;28;5;35;3
ASEEND*/
//CHKSM=FE8F2C094F484650D5576EFFB48A865E619B4681