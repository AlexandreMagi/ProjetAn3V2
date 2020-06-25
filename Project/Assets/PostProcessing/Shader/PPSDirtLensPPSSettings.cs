// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( PPSDirtLensPPSRenderer ), PostProcessEvent.AfterStack, "PPSDirtLens", true )]
public sealed class PPSDirtLensPPSSettings : PostProcessEffectSettings
{
	[Tooltip( "Lens dirt texture" )]
	public TextureParameter _Lensdirttexture = new TextureParameter {  };
	[Tooltip( "DirtMask" )]
	public TextureParameter _DirtMask = new TextureParameter {  };
	[Tooltip( "Fuzziness" )]
	public FloatParameter _Fuzziness = new FloatParameter { value = 0f };
	[Tooltip( "Range" )]
	public FloatParameter _Range = new FloatParameter { value = 0f };
}

public sealed class PPSDirtLensPPSRenderer : PostProcessEffectRenderer<PPSDirtLensPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "PPSDirtLens" ) );
		if(settings._Lensdirttexture.value != null) sheet.properties.SetTexture( "_Lensdirttexture", settings._Lensdirttexture );
		if(settings._DirtMask.value != null) sheet.properties.SetTexture( "_DirtMask", settings._DirtMask );
		sheet.properties.SetFloat( "_Fuzziness", settings._Fuzziness );
		sheet.properties.SetFloat( "_Range", settings._Range );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
