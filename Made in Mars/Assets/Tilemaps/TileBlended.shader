// Code originaly from Unity's built-in Sprites-Default.shader
// Modified by ComputerKim, modifications are commented

Shader "Sprites/TileBlended" // Name changed
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		// Custom properties
		_TransparencyRatio("Transparency Ratio", Range(0, 0.5)) = 0.1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment CustomSpriteFrag // Originally SpriteFrag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"
			
			// Custom properties
			float _TransparencyRatio;

			fixed4 CustomSpriteFrag(v2f IN) : SV_Target // Copy of SpriteFrag from UnitySprites.cginc with modification
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;

				// Apply transparency to edges
				float multiplier = 1 / _TransparencyRatio;
				if (IN.texcoord.x > 1 - _TransparencyRatio) c *= (1 - IN.texcoord.x) * multiplier;
				if (IN.texcoord.x < _TransparencyRatio) c *= IN.texcoord.x * multiplier;
				if (IN.texcoord.y > 1 - _TransparencyRatio) c *= (1 - IN.texcoord.y) * multiplier;
				if (IN.texcoord.y < _TransparencyRatio) c *= IN.texcoord.y * multiplier;
				//c *= sin(IN.texcoord.x + IN.texcoord.y * 3.14) * sin(IN.texcoord.y * 3.14);  // Could be used for hex?

				return c;
			}
		ENDCG
		}
	}
}