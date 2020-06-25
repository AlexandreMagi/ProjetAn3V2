// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PPSDirtLens"
{
	Properties
	{
		_Lensdirttexture("Lens dirt texture", 2D) = "white" {}
		_DirtMask("DirtMask", 2D) = "white" {}
		_MaskMultiplier("MaskMultiplier", Float) = 0.1
		_Range("Range", Float) = 0
		_Fuzziness("Fuzziness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			
		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform sampler2D _DirtMask;
			uniform float4 _DirtMask_ST;
			uniform float _MaskMultiplier;
			uniform sampler2D _Lensdirttexture;
			uniform float4 _Lensdirttexture_ST;
			uniform float _Range;
			uniform float _Fuzziness;


			
			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 mainTex397 = tex2D( _MainTex, uv_MainTex );
				float2 uv_DirtMask = i.texcoord.xy * _DirtMask_ST.xy + _DirtMask_ST.zw;
				float2 uv_Lensdirttexture = i.texcoord.xy * _Lensdirttexture_ST.xy + _Lensdirttexture_ST.zw;
				float4 color403 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float grayscale399 = Luminance(mainTex397.rgb);
				float3 temp_cast_2 = (grayscale399).xxx;
				float4 blendOpSrc217 = mainTex397;
				float4 blendOpDest217 = ( ( ( 1.0 - tex2D( _DirtMask, uv_DirtMask ) ) * _MaskMultiplier ) * ( tex2D( _Lensdirttexture, uv_Lensdirttexture ) * saturate( ( 1.0 - ( ( distance( color403.rgb , temp_cast_2 ) - _Range ) / max( _Fuzziness , 1E-05 ) ) ) ) ) );
				

				float4 color = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc217 ) * ( 1.0 - blendOpDest217 ) ) ));
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
1920;0;1920;1019;2544.566;-396.2087;2.184643;True;False
Node;AmplifyShaderEditor.CommentaryNode;401;-1883.827,473.9579;Inherit;False;900.448;280;Comment;3;69;397;76;MainTex;1,1,1,1;0;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;76;-1833.827,552.1918;Inherit;False;0;0;_MainTex;Pass;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;69;-1584.13,523.9579;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;397;-1207.379,533.7718;Inherit;False;mainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;400;-1413.25,1539.27;Inherit;False;397;mainTex;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;265;-996.3518,1031.595;Inherit;True;Property;_DirtMask;DirtMask;1;0;Create;True;0;0;False;0;86f26fb0047536c46909ce135114df30;86f26fb0047536c46909ce135114df30;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;403;-993.8721,1660.877;Inherit;False;Constant;_Color0;Color 0;2;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;416;-727.837,1908.833;Inherit;False;Property;_Fuzziness;Fuzziness;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;245;-739.8862,1039.239;Inherit;True;Property;_Mask;Mask;1;0;Create;True;0;0;False;0;-1;86f26fb0047536c46909ce135114df30;86f26fb0047536c46909ce135114df30;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;415;-733.837,1820.833;Inherit;False;Property;_Range;Range;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;399;-973.5021,1573.151;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;387;-420.556,1251.71;Inherit;False;Property;_MaskMultiplier;MaskMultiplier;2;0;Create;True;0;0;False;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;385;-355.7415,1159.202;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;82;-751.0284,1367.558;Inherit;True;Property;_Lensdirttexture;Lens dirt texture;0;0;Create;True;0;0;False;0;-1;b6beeebe19990a842b9ce6fa1954ad96;b6beeebe19990a842b9ce6fa1954ad96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;402;-515.2397,1752.183;Inherit;False;Color Mask;-1;;1;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;1.5;False;5;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;386;-170.5561,1175.71;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;389;-174.312,1508.524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;398;586.2697,969.2704;Inherit;False;397;mainTex;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;479.0823,1131.933;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;217;825.963,970.3867;Inherit;False;Screen;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;75;1091.929,1006.564;Float;False;True;-1;2;ASEMaterialInspector;0;2;PPSDirtLens;32139be9c1eb75640a847f011acf3bcf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;True;2;False;-1;False;False;True;2;False;-1;True;7;False;-1;False;False;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;69;0;76;0
WireConnection;397;0;69;0
WireConnection;245;0;265;0
WireConnection;399;0;400;0
WireConnection;385;0;245;0
WireConnection;402;1;399;0
WireConnection;402;3;403;0
WireConnection;402;4;415;0
WireConnection;402;5;416;0
WireConnection;386;0;385;0
WireConnection;386;1;387;0
WireConnection;389;0;82;0
WireConnection;389;1;402;0
WireConnection;335;0;386;0
WireConnection;335;1;389;0
WireConnection;217;0;398;0
WireConnection;217;1;335;0
WireConnection;75;0;217;0
ASEEND*/
//CHKSM=76EEF5F5CE90945D6E28284AE030E9D3D0247470