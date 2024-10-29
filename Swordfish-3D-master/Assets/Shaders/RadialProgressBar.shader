// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Custom/RadialProgressBar"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Progress("Progress", Range( 0 , 1)) = 1
		_ColorA("Color A", Color) = (1,0.8081139,0,0)
		_ColorB("Color B", Color) = (1,0.08913773,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ColorA;
		uniform float4 _ColorB;
		uniform float _Progress;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 lerpResult42 = lerp( _ColorA , _ColorB , _Progress);
			o.Emission = lerpResult42.rgb;
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float2 temp_output_21_0 = (float2( 1,1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( -1,-1 ) - float2( 1,1 )) / (float2( 1,1 ) - float2( 0,0 )));
			float2 break26 = temp_output_21_0;
			o.Alpha = ( tex2D( _Texture, uv_Texture ).r * step( (0.0 + (atan2( break26.x , break26.y ) - ( -1.0 * UNITY_PI )) * (1.0 - 0.0) / (UNITY_PI - ( -1.0 * UNITY_PI ))) , _Progress ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
7;113;1906;795;267.8932;-223.0482;1;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1009.508,463.9604;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;21;-737.7219,463.2875;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;1,1;False;4;FLOAT2;-1,-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;26;-400.1287,674.838;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ATan2OpNode;22;-127.2136,677.3271;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;32;-111.7822,506.0973;Float;False;1;0;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;34;-113.7822,595.0973;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;33;178.2178,669.0973;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;169.1092,935.7834;Float;False;Property;_Progress;Progress;1;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;35;509.7013,797.9324;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;725.951,27.63055;Float;False;Property;_ColorA;Color A;2;0;Create;True;0;0;False;0;1,0.8081139,0,0;1,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;43;727.6528,221.3139;Float;False;Property;_ColorB;Color B;3;0;Create;True;0;0;False;0;1,0.08913773,0,0;0,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;451.5912,420.2302;Float;True;Property;_Texture;Texture;0;0;Create;True;0;0;False;0;None;7b5bef9880a5fa04c81db594699e65b3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;812.7736,561.108;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;42;1074.177,145.0335;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LengthOpNode;19;-399.7219,447.2875;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;1529.389,377.2834;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;_Custom/RadialProgressBar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;9;0
WireConnection;26;0;21;0
WireConnection;22;0;26;0
WireConnection;22;1;26;1
WireConnection;33;0;22;0
WireConnection;33;1;32;0
WireConnection;33;2;34;0
WireConnection;35;0;33;0
WireConnection;35;1;36;0
WireConnection;11;0;3;1
WireConnection;11;1;35;0
WireConnection;42;0;4;0
WireConnection;42;1;43;0
WireConnection;42;2;36;0
WireConnection;19;0;21;0
WireConnection;2;2;42;0
WireConnection;2;9;11;0
ASEEND*/
//CHKSM=55244373B2B3DD6E1704EF3A488548EDFA571016