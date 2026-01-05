cbuffer ViewProj : register(b0)
{
    matrix view;
    matrix proj;
}

cbuffer Module : register(b1)
{
    matrix module;
}

struct Attributes
{
    float3 position : POSITION;
    float3 color : COLOR0;
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

    matrix viewProj = mul(view, proj);
    
    Out.position = float4(In.position, 1);
    Out.position = mul(Out.position, module);
    Out.position = mul(Out.position, viewProj);

    Out.uv = In.uv;

    return Out;
}

Texture2D tex : register(t0);
SamplerState samLineear : register(s0);

float4 frag(Varyings In) : SV_Target
{
    return tex.Sample(samLineear, In.uv);
}
