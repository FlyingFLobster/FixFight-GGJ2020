#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//This shader was sourced from a tutorial article at: https://mysteriousspace.com/2019/01/05/pixel-shaders-in-monogame-a-tutorial-of-sorts-for-2019/
//Explicit permission for using the shader is given in the article.
sampler inputTexture;

float4 MainPS(float2 textureCoordinates: TEXCOORD0): COLOR0
{
	float4 color = tex2D(inputTexture, textureCoordinates);
	color.rgb = 1.0f;
	return color;
}

technique Techninque1
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 MainPS();
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
	}
};