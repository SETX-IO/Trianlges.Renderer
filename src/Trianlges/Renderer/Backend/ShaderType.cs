namespace Trianlges.Renderer.Backend;

public enum ShaderType
{
    Vertex,
    Hull,
    Domain,
    Geometry,
    Pixel,
    Compute
}

public enum ShaderVersion
{
    V1 = 1, 
    V2, 
    V3, 
    V4, 
    V5
}