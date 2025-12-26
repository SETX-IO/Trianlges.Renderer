cbuffer vertexBuffer : register(b1) {
    float4x4 ProjectionMatrix;
}

struct Attributes
{
    float2 pos : POSITION;
    float4 color : COLOR0;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 pos : SV_POSITION;
    float4 color : COLOR0;
    float2 uv : TEXCOORD0;
};

Varyings vert(Attributes In)
{
    Varyings Out;

    Out.pos = mul(ProjectionMatrix, float4(In.pos.xy, 0, 1));
    Out.color = In.color;
    Out.uv = In.uv;

    return Out;
}

sampler sampler0;
Texture2D texture0;

float4 frag(Varyings In) : SV_Target
{
    float out_color = In.color * texture0.Sample(sampler0, In.uv);
    return out_color;
}

