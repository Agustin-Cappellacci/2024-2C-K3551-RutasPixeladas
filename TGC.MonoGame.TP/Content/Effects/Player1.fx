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
float4x4 LightViewProjection; // Matriz para proyectar desde la luz

float3 lightPosition; // Colocar en una posición alta para simular luz desde arriba

texture ModelTexture;
sampler2D TextureSampler = sampler_state
{
    Texture = (ModelTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture ShadowMap;
sampler2D ShadowSampler = sampler_state
{
    Texture = (ShadowMap);
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Point;
    MagFilter = Point;
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
    float4 LightSpacePosition : TEXCOORD3; // Coordenadas en el espacio de la luz
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

    // Transformación al espacio de la luz
    output.LightSpacePosition = mul(worldPosition, LightViewProjection);

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Muestra el color de la textura
    float3 color = tex2D(TextureSampler, input.TextureCoordinate.xy).rgb;

    // Coordenadas de textura de sombras (normalización de -1 a 1 a 0 a 1)
    float2 shadowTexCoords = input.LightSpacePosition.xy / input.LightSpacePosition.w * 0.5 + 0.5;

    // Profundidad desde el ShadowMap
    float shadowDepth = tex2D(ShadowSampler, shadowTexCoords).r;
    float fragmentDepth = input.LightSpacePosition.z / input.LightSpacePosition.w;

    // Factor de sombra: si el fragmento está en sombra, reduce la iluminación
    float shadow = (fragmentDepth > shadowDepth + 0.005) ? 0.6 : 1.0; // Sombras más claras

    // Cálculo de iluminación difusa y aumento de intensidad
    float3 N = normalize(input.Normal);
    float3 L = normalize(lightPosition - input.WorldPos); // Dirección de la luz desde la posición en mundo
    float kd = saturate(0.4 + 1.0 * saturate(dot(N, L))); // Aumenta la intensidad de iluminación

    // Aplicación de iluminación y sombra
    return float4(color * kd * shadow * 1.5, 1); // Multiplica color final por 1.2 para más brillo
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
