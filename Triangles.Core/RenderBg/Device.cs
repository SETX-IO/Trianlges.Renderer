using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Triangles.Core.Renderer;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Feature = Vortice.Direct3D12.Feature;

namespace Triangles.Core.RenderBg;

public partial class Device
    {
        private readonly ID3D12Device _device;

        public ID3D12CommandQueue CommandQueue { get; }
        public uint RtvDescriptorSize => _device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
        public ID3D12Device LDevice => _device;
        public ID3D12RootSignature RootSignature { get; protected set; } = null!;
        public DescriptionHeapPool DescriptionHeapPool;

        public unsafe Device()
        {
            if (!D3D12.IsSupported())
                throw new COMException("You device not support Direct3D12.");

            IDXGIAdapter4 miniAdapter;
            
            for (short i = 0;; ++i)
            {
                IDXGIAdapter4 adapter = Dxgi.Factory.Value.EnumAdapterByGpuPreference<IDXGIAdapter4>((uint)i, GpuPreference.HighPerformance);
                if (D3D12.D3D12CreateDevice<ID3D12Device>(adapter, out _) == Result.Ok)
                {
                    miniAdapter = adapter;
                    break;
                }

                adapter.Dispose();
            }

            FeatureLevel[] featureLevels = 
            [
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_12_0,
                FeatureLevel.Level_12_1,
            ];

            FeatureDataFeatureLevels featureInfo;
            fixed (void* featureLevelsPtr = featureLevels)
            {
                featureInfo = new FeatureDataFeatureLevels
                {
                    NumFeatureLevels = (uint)featureLevels.Length,
                    FeatureLevelsRequested = new nint(featureLevelsPtr)
                };
            }
            
            D3D12.D3D12CreateDevice<ID3D12Device>(miniAdapter, out var device);
            device?.CheckFeatureSupport(Feature.FeatureLevels, ref featureInfo);

            _device = D3D12.D3D12CreateDevice<ID3D12Device>(miniAdapter, featureInfo.MaxSupportedFeatureLevel);
            _device.Name = "Triangles Render Device(Dx12)";

            CommandQueue = _device.CreateCommandQueue<ID3D12CommandQueue>(CommandListType.Direct);
            CommandQueue.Name = "Triangles Graphics Queue(Dx12)";

            DescriptionHeapPool = new DescriptionHeapPool(this);
            
            miniAdapter.Dispose();
        }

        public void ExecuteCommandBuffer(ICommandBuffer cmd)
        {
            cmd.CmdBuffer.CommandList.EndEvent();
            
            cmd.CloseCmd();
            CommandQueue.ExecuteCommandList(cmd.CmdBuffer.CommandList);
        }
        
        public void Dispose()
        {
            _device.Dispose();
            CommandQueue.Dispose();
            RootSignature.Dispose();
        }
    }