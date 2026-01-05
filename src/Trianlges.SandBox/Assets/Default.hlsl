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
};

struct Varyings
{
    float4 position : SV_POSITION;
};

Varyings vert(Attributes In)
{
    Varyings Out;

    matrix viewProj = mul(view, proj);
    
    Out.position = float4(In.position, 1);
    Out.position = mul(Out.position, module);
    Out.position = mul(Out.position, viewProj);
    
    return Out;
}

float4 frag(Varyings In) : SV_Target
{
    return float4(0.8f, 0.8f, 0.8f, 1);
}
