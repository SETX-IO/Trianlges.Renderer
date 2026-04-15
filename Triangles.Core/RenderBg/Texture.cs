using System.Runtime.CompilerServices;
using Triangles.Core.Renderer;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Triangles.Core.RenderBg;

public class Texture : IDisposable
{
    protected ID3D12Resource Resource;
    protected Device Device;
    
    public Texture(Device device, uint width, uint height, Format format = Format.R8G8B8A8_UNorm)
    {
        Device = device;
        
        ResourceDescription textureInfo = new ResourceDescription
        {
            MipLevels = 1,
            Format = format,
            Flags = ResourceFlags.None,
            Width = width,
            Height = height,
            DepthOrArraySize = 1,
            SampleDescription = new SampleDescription(1, 0),
            Dimension = ResourceDimension.Texture2D,
            Layout = TextureLayout.Unknown
        };

        Resource = Device.LDevice.CreateCommittedResource<ID3D12Resource>(new HeapProperties(HeapType.Default), HeapFlags.None, textureInfo, ResourceStates.CopyDest);
    }
    
    protected Texture() {}

    private byte[] GenerateTexture()
    {
        uint rowPitch = 256 * 4;
        uint cellPitch = rowPitch >> 3;
        uint cellHeight = 256 >> 3;
        uint textureSize = rowPitch * 256;

        byte[] data = new byte[textureSize];

        for (uint i = 0; i < textureSize; i += 4)
        {
            uint x = i % rowPitch;
            uint y = i / rowPitch;
            uint n = x / cellPitch;
            uint j = y / cellHeight;

            if (n % 2 == j % 2)
            {
                data[i] = 0x00;     // R
                data[i + 1] = 0x00; // G
                data[i + 2] = 0x00; // B
                data[i + 3] = 0xff; // A
            }
            else
            {
                data[i] = 0xff;     // R
                data[i + 1] = 0xff; // G
                data[i + 2] = 0xff; // B
                data[i + 3] = 0xff; // A
            }
        }

        return data;
    }

    public unsafe void CreateView(CommandBufferPool cmdPool, ShaderVisibility bindShader = ShaderVisibility.Pixel, uint bindSlot = 0)
    {
        UpLoadHeap heap = new UpLoadHeap(Device.DescriptionHeapPool.GetHeap(DescriptionHeapType.CbvSrvUav));
        
        ID3D12Resource textureUpLoadHeap = Device.LDevice.CreateCommittedResource<ID3D12Resource>(new HeapProperties(HeapType.Upload), HeapFlags.None,
            ResourceDescription.Buffer(Resource.GetRequiredIntermediateSize(0, 1)), ResourceStates.GenericRead);

        byte[] textureData = GenerateTexture();
        SubresourceData data;
        fixed (void* ptr = textureData)
            data = new SubresourceData(ptr, 256 * 4, 256 * 4 * 256);
            // data.pData = textureDataPtr;

        ICommandBuffer cmd = cmdPool.Get("Copy Data");
        
        cmd.CmdBuffer.CommandList.UpdateSubresources(Resource, textureUpLoadHeap, 0, 0, 1, &data);
        cmd.ResourceBarrier(Resource, ResourceStates.CopyDest, ResourceStates.PixelShaderResource);
        
        ShaderResourceViewDescription srvInfo = new ShaderResourceViewDescription
        {
            Shader4ComponentMapping = ShaderComponentMapping.Default,
            Format = Format.R8G8B8A8_UNorm,
            ViewDimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new Texture2DShaderResourceView { MipLevels = 1 },
        };

        heap.CreateSrv(Resource, srvInfo, bindShader, bindSlot);
        
        Device.ExecuteCommandBuffer(cmd);
    }
    
    public void Dispose()
    {
        Resource.Dispose();
    }
}

public class RenderTargetTexture : Texture
{
    public CpuDescriptorHandle ResourceHandel;
    
    public RenderTargetTexture(ID3D12Resource resource, CpuDescriptorHandle handle, Device device)
    {
        Resource = resource;
        ResourceHandel = handle;
        Device = device;
    }
    
    public void Lock(ICommandBuffer cmdBuffer)
    {
        cmdBuffer.ResourceBarrier(Resource);
    }

    public void Unlock(ICommandBuffer cmdBuffer)
    {
        cmdBuffer.ResourceBarrier(Resource);
    }
}