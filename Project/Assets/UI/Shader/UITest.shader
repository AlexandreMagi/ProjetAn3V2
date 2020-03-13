// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Custom Shader UI/UITest"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_Sprite("Sprite", 2D) = "white" {}
		_Flowtexture("Flow texture", 2D) = "white" {}
		_Flowmask("Flow mask", 2D) = "white" {}
		[NoScaleOffset]_Shielddistortiontext("Shield distortion text", 2D) = "white" {}
		[NoScaleOffset][Normal]_Shielddistortionnormal("Shield distortion normal", 2D) = "bump" {}
		_Distortionamount("Distortion amount", Range( 0 , 1)) = 0
		_Mask("Mask", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform sampler2D _Sprite;
			uniform float4 _Sprite_ST;
			uniform sampler2D _Shielddistortiontext;
			uniform sampler2D _Shielddistortionnormal;
			uniform float4 _Shielddistortiontext_ST;
			uniform float _Distortionamount;
			uniform sampler2D _Flowtexture;
			uniform sampler2D _Flowmask;
			uniform float4 _Flowmask_ST;
			uniform float _Mask;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_Sprite = IN.texcoord.xy * _Sprite_ST.xy + _Sprite_ST.zw;
				float4 baseSprite46 = ( tex2D( _Sprite, uv_Sprite ) * _Color );
				float4 temp_output_192_0_g2 = baseSprite46;
				float2 uv0_Shielddistortiontext = IN.texcoord.xy * _Shielddistortiontext_ST.xy + _Shielddistortiontext_ST.zw;
				float2 MainUvs222_g2 = uv0_Shielddistortiontext;
				float4 tex2DNode65_g2 = tex2D( _Shielddistortionnormal, MainUvs222_g2 );
				float4 appendResult82_g2 = (float4(0.0 , tex2DNode65_g2.g , 0.0 , tex2DNode65_g2.r));
				float2 temp_output_84_0_g2 = (UnpackScaleNormal( appendResult82_g2, _Distortionamount )).xy;
				float2 panner179_g2 = ( 1.0 * _Time.y * float2( 0,-0.1 ) + MainUvs222_g2);
				float2 temp_output_71_0_g2 = ( temp_output_84_0_g2 + panner179_g2 );
				float4 tex2DNode96_g2 = tex2D( _Shielddistortiontext, temp_output_71_0_g2 );
				float4 shieldDistortion54 = ( temp_output_192_0_g2 + ( tex2DNode96_g2 * (temp_output_192_0_g2).a ) );
				float4 temp_output_192_0_g3 = shieldDistortion54;
				float2 uv0_Flowmask = IN.texcoord.xy * _Flowmask_ST.xy + _Flowmask_ST.zw;
				float4 tex2DNode14_g3 = tex2D( _Flowmask, uv0_Flowmask );
				float2 appendResult20_g3 = (float2(tex2DNode14_g3.r , tex2DNode14_g3.g));
				float2 temp_cast_0 = (_SinTime.w).xx;
				float2 temp_output_18_0_g3 = ( appendResult20_g3 - temp_cast_0 );
				float4 tex2DNode72_g3 = tex2D( _Flowtexture, temp_output_18_0_g3 );
				float4 flow68 = ( temp_output_192_0_g3 + ( ( tex2DNode72_g3 * tex2DNode14_g3.a ) * (temp_output_192_0_g3).a ) );
				
				fixed4 c = saturate( ( IN.color * ( flow68 * ( 1.0 - ( IN.texcoord.xy.y * (  ( _Mask - 0.05 > 1.0 ? 0.0 : _Mask - 0.05 <= 1.0 && _Mask + 0.05 >= 1.0 ? 100.0 : _Mask )  * 3.0 ) ) ) ) ) );
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17700
206;198;1437;641;-443.1718;316.4713;2.135498;True;False
Node;AmplifyShaderEditor.CommentaryNode;45;-4062.535,-181.5453;Inherit;False;1643.485;592.8389;Comment;5;46;43;41;44;40;Base sprite;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;40;-4012.536,-131.5453;Inherit;True;Property;_Sprite;Sprite;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;41;-3704.395,-90.00165;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;44;-3552.636,141.8769;Inherit;False;0;0;_Color;Shader;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-3352.509,-33.40115;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;52;-2315.215,-179.8271;Inherit;False;1561.283;562.3608;Comment;7;54;47;49;51;58;48;50;Shield distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-2829.193,-30.05726;Inherit;False;baseSprite;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;50;-2219.202,162.5237;Inherit;True;Property;_Shielddistortionnormal;Shield distortion normal;11;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;False;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1922.896,214.8401;Inherit;False;Property;_Distortionamount;Distortion amount;12;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-2133.547,-129.8271;Inherit;False;46;baseSprite;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;58;-1883.681,51.80791;Inherit;False;Constant;_Vector1;Vector 1;5;0;Create;True;0;0;False;0;0,-0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;49;-2265.215,-49.96709;Inherit;True;Property;_Shielddistortiontext;Shield distortion text;10;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.CommentaryNode;56;756.7478,-119.7895;Inherit;False;1765.342;949.4526;Comment;15;1;81;76;75;62;55;61;77;60;90;96;91;95;97;78;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;47;-1547.497,8.464135;Inherit;False;UI-Sprite Effect Layer;1;;2;789bf62641c5cfe4ab7126850acc22b8;18,204,0,74,0,191,0,225,0,242,0,237,0,249,0,186,0,177,1,182,0,229,0,92,0,98,0,234,0,126,0,129,1,130,0,31,2;18;192;COLOR;1,1,1,1;False;39;COLOR;1,1,1,1;False;37;SAMPLER2D;;False;218;FLOAT2;0,0;False;239;FLOAT2;0,0;False;181;FLOAT2;0,0;False;75;SAMPLER2D;;False;80;FLOAT;1;False;183;FLOAT2;0,0;False;188;SAMPLER2D;;False;33;SAMPLER2D;;False;248;FLOAT2;0,0;False;233;SAMPLER2D;;False;101;SAMPLER2D;;False;57;FLOAT4;0,0,0,0;False;40;FLOAT;0;False;231;FLOAT;1;False;30;FLOAT;1;False;2;COLOR;0;FLOAT2;172
Node;AmplifyShaderEditor.RangedFloatNode;78;790.9924,357.9849;Inherit;False;Property;_Mask;Mask;13;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-962.7769,57.25125;Inherit;False;shieldDistortion;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;95;907.9969,473.7494;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;False;0;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;69;-596.602,20.6181;Inherit;False;1179.957;525.9631;Comment;6;64;65;66;63;68;70;Flow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;97;937.4359,555.9095;Inherit;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;False;0;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;66;-546.602,316.5813;Inherit;True;Property;_Flowmask;Flow mask;9;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;91;1072.538,737.5557;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCIf;96;1084.436,376.9095;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;-210.709,70.6181;Inherit;False;54;shieldDistortion;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;65;-543.3212,103.2429;Inherit;True;Property;_Flowtexture;Flow texture;8;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SinTimeNode;70;-182.2744,338.719;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1364.661,444.7962;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;63;38.31028,155.6996;Inherit;False;UI-Sprite Effect Layer;1;;3;789bf62641c5cfe4ab7126850acc22b8;18,204,1,74,1,191,0,225,0,242,0,237,0,249,1,186,0,177,0,182,0,229,0,92,1,98,0,234,0,126,0,129,1,130,0,31,2;18;192;COLOR;1,1,1,1;False;39;COLOR;1,1,1,1;False;37;SAMPLER2D;;False;218;FLOAT2;0,0;False;239;FLOAT2;0,0;False;181;FLOAT2;0,0;False;75;SAMPLER2D;;False;80;FLOAT;1;False;183;FLOAT2;0,0;False;188;SAMPLER2D;;False;33;SAMPLER2D;;False;248;FLOAT2;0,0;False;233;SAMPLER2D;;False;101;SAMPLER2D;;False;57;FLOAT4;0,0,0,0;False;40;FLOAT;0;False;231;FLOAT;1;False;30;FLOAT;1;False;2;COLOR;0;FLOAT2;172
Node;AmplifyShaderEditor.TexCoordVertexDataNode;60;1309.343,234.0606;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;359.3545,168.6622;Inherit;False;flow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;1528.961,278.7578;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;61;1684.701,232.2271;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;1603.809,125.7472;Inherit;False;68;flow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;1833.878,121.7999;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;75;1695.601,-63.15022;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;2010.132,91.13684;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;81;2183.778,116.4806;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;2375.709,127.1362;Float;False;True;-1;2;ASEMaterialInspector;0;6;_Custom Shader UI/UITest;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;41;0;40;0
WireConnection;43;0;41;0
WireConnection;43;1;44;0
WireConnection;46;0;43;0
WireConnection;47;192;48;0
WireConnection;47;37;49;0
WireConnection;47;181;58;0
WireConnection;47;75;50;0
WireConnection;47;80;51;0
WireConnection;54;0;47;0
WireConnection;96;0;78;0
WireConnection;96;3;95;0
WireConnection;96;4;78;0
WireConnection;96;5;97;0
WireConnection;90;0;96;0
WireConnection;90;1;91;0
WireConnection;63;192;64;0
WireConnection;63;37;65;0
WireConnection;63;33;66;0
WireConnection;63;248;70;4
WireConnection;68;0;63;0
WireConnection;77;0;60;2
WireConnection;77;1;90;0
WireConnection;61;0;77;0
WireConnection;62;0;55;0
WireConnection;62;1;61;0
WireConnection;76;0;75;0
WireConnection;76;1;62;0
WireConnection;81;0;76;0
WireConnection;1;0;81;0
ASEEND*/
//CHKSM=C2507E6CAF8DB5B102DADB58C6FFAA1B3C75404C