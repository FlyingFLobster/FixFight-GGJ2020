#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Slightly modified version of the shader shown in a tutorial at: https://digitalerr0r.net/2009/04/22/xna-shader-programming-tutorial-9-post-process-wiggle/

float fTimer;
sampler ColorMapSampler : register(s0);

float4 MainPS(float2 Tex: TEXCOORD0) : COLOR0
{
     //Tex.x += sin(fTimer+Tex.x*10)*0.01f;
     Tex.x += cos(fTimer+Tex.x*10)*0.01f;
     Tex.y += cos(fTimer+Tex.y*10)*0.01f;

     float4 Color = tex2D(ColorMapSampler, Tex);
     return Color;
}

technique PostProcess
{
     pass Pass1
     {
          PixelShader = compile ps_3_0 MainPS();
     }
} 