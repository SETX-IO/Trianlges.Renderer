using Silk.NET.GLFW;

namespace Triangles.BaseApp;

public struct AppConfig
{
    public string Name { get; }
    public GraphicsApi Api { get; }
    public bool Resizable { get; }
    
    public AppConfig(string appName, GraphicsApi api = GraphicsApi.OpenGl, bool resizable = false)
    {
        Name = appName;
        Api = api;
        Resizable = resizable;

        Glfw glfwApi = GlfwProvider.GLFW.Value;

        glfwApi.Init();
        
        glfwApi.WindowHint(WindowHintClientApi.ClientApi, (ClientApi)api);
        glfwApi.WindowHint(WindowHintBool.Resizable, resizable);
        glfwApi.WindowHint(WindowHintBool.Visible, false);
    }
}