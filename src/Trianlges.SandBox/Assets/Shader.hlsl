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
};

struct Varyings
{
    float4 position : SV_POSITION;
    float4 color : COLOR0;
};

Varyings vert(Attributes In)
{
    Varyings Out;

    matrix viewProj = mul(view, proj);
    
    Out.position = float4(In.position, 1);
    Out.position = mul(Out.position, module);
    Out.position = mul(Out.position, viewProj);

    Out.color = float4(In.color, 1.0f);

    return Out;
}

float4 frag(Varyings In) : SV_Target
{
    return In.color;
}
