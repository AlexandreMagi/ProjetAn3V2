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
	[Tooltip( "MaskMultiplier" )]
	public FloatParameter _MaskMultiplier = new FloatParameter { value = 0.1f };
	[Tooltip( "Range" )]
	public FloatParameter _Range = new FloatParameter { value = 0f };
	[Tooltip( "Fuzziness" )]
	public FloatParameter _Fuzziness = new FloatParameter { value = 0f };
}

public sealed class PPSDirtLensPPSRenderer : PostProcessEffectRenderer<PPSDirtLensPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "PPSDirtLens" ) );
		if(settings._Lensdirttexture.value != null) sheet.properties.SetTexture( "_Lensdirttexture", settings._Lensdirttexture );
		if(settings._DirtMask.value != null) sheet.properties.SetTexture( "_DirtMask", settings._DirtMask );
		sheet.properties.SetFloat( "_MaskMultiplier", settings._MaskMultiplier );
		sheet.properties.SetFloat( "_Range", settings._Range );
		sheet.properties.SetFloat( "_Fuzziness", settings._Fuzziness );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
