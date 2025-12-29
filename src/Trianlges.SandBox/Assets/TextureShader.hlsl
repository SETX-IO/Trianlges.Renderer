cbuffer ModuleViewProj : register(b0)
{
    matrix module;
    matrix view;
    matrix proj;
}

struct Attributes
{
    float3 position : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

Varyings vert(Attributes In)
{
    Varyings Out;

    Out.position = float4(In.position, 1);
    Out.position = mul(Out.position, module);
    Out.position = mul(Out.position, view);
    Out.position = mul(Out.position, proj);

    Out.uv = In.uv;

    return Out;
}

Texture2D tex : register(t0);
SamplerState samLineear : register(s0);

float4 frag(Varyings In) : SV_Target
{
    return tex.Sample(samLineear, In.uv);
}
