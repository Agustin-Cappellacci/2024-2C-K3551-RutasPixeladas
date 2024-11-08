#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 lightPosition; // Luz en el espacio del mundo

texture ModelTexture;
sampler2D TextureSampler = sampler_state
{
    Texture = (ModelTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 TextureCoordinate : TEXCOORD0;
    float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 TextureCoordinate : TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 Normal : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;

    // Transformación de posición en espacio de mundo
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Coordenadas de textura y normal en espacio de mundo
    output.TextureCoordinate = input.TextureCoordinate;
    output.WorldPos = worldPosition.xyz;
    output.Normal = mul(input.Normal, (float3x3) World);

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Muestra el color de la textura
    float3 color = tex2D(TextureSampler, input.TextureCoordinate.xy).rgb;

    // Cálculo de iluminación difusa
    float3 N = normalize(input.Normal);
    float3 L = normalize(lightPosition - input.WorldPos); // Dirección de la luz
    float kd = saturate(0.4 + 1.0 * saturate(dot(N, L))); // Aumenta la intensidad de iluminación

    // Aplicación de iluminación difusa
    return float4(color * kd * 1.5, 1); // Multiplica color final para más brillo
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
