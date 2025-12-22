cbuffer ModuleViewProj : register(b0)
{
    matrix mvp;
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
    
    Out.position = float4(In.position, 1);
    Out.position = mul(Out.position, mvp);
    
    Out.color = float4(In.color, 1.0f);
    
    return Out;
}

float4 frag(Varyings In) : SV_Target
{
    return In.color;
}