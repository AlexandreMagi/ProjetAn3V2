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
	[Tooltip( "Screen" )]
	public TextureParameter _MainTex = new TextureParameter {  };
	[Tooltip( "PixelAmountX" )]
	public FloatParameter _PixelAmountX = new FloatParameter { value = 0f };
	[Tooltip( "Texture Sample 0" )]
	public TextureParameter _TextureSample0 = new TextureParameter {  };
	[Tooltip( "PixelAmountY" )]
	public FloatParameter _PixelAmountY = new FloatParameter { value = 0f };
}

public sealed class PPSDirtLensPPSRenderer : PostProcessEffectRenderer<PPSDirtLensPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "PPSDirtLens" ) );
		if(settings._MainTex.value != null) sheet.properties.SetTexture( "_MainTex", settings._MainTex );
		sheet.properties.SetFloat( "_PixelAmountX", settings._PixelAmountX );
		if(settings._TextureSample0.value != null) sheet.properties.SetTexture( "_TextureSample0", settings._TextureSample0 );
		sheet.properties.SetFloat( "_PixelAmountY", settings._PixelAmountY );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
