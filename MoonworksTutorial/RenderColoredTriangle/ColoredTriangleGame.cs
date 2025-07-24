using MoonWorks;
using MoonWorks.Graphics;
using System.Numerics;
using System.Runtime.InteropServices;
using Buffer = MoonWorks.Graphics.Buffer;

namespace RenderColoredTriangle;

[StructLayout(LayoutKind.Sequential)]
public struct PositionColorVertex(Vector2 position, Color color) : IVertexType
{
    public Vector2 Position = position;
    public Vector4 Color = color.ToVector4();

    public static VertexElementFormat[] Formats =>
    [
        VertexElementFormat.Float2,
        VertexElementFormat.Float4,
    ];

    public static uint[] Offsets { get; } = [0, 8];
}

public class ColoredTriangleGame : Game
{
    private readonly Buffer _vertexBuffer;
    private readonly GraphicsPipeline _pipeline;

    public ColoredTriangleGame(
       AppInfo appInfo,
       WindowCreateInfo windowCreateInfo,
       FramePacingSettings framePacingSettings
       ) : base(
           appInfo,
           windowCreateInfo,
           framePacingSettings,
           ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC)
    {
        ShaderCross.Initialize();

        var vertexShader = ShaderCross.Create(
           GraphicsDevice,
           RootTitleStorage,
           "Content/Shaders/Vertex.vert.hlsl",
           "main",
           ShaderCross.ShaderFormat.HLSL,
           ShaderStage.Vertex);

        var fragmentShader = ShaderCross.Create(
            GraphicsDevice,
            RootTitleStorage,
            "Content/Shaders/Color.frag.hlsl",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Fragment);

        var pipelineCreateInfo = new GraphicsPipelineCreateInfo
        {
            TargetInfo = new GraphicsPipelineTargetInfo
            {
                ColorTargetDescriptions = [
                  new ColorTargetDescription
                    {
                        Format = MainWindow.SwapchainFormat,
                        BlendState = ColorTargetBlendState.Opaque
                    }
              ]
            },
            DepthStencilState = DepthStencilState.Disable,
            MultisampleState = MultisampleState.None,
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            VertexInputState = VertexInputState.Empty,
            VertexShader = vertexShader,
            FragmentShader = fragmentShader
        };

        pipelineCreateInfo.VertexInputState = VertexInputState.CreateSingleBinding<PositionColorVertex>();
        _pipeline = GraphicsPipeline.Create(GraphicsDevice, pipelineCreateInfo);

        ReadOnlySpan<PositionColorVertex> vertexData = [
           new PositionColorVertex(new Vector2(-0.5f, -0.5f), Color.Red),
           new PositionColorVertex(new Vector2(0.5f, -0.5f), Color.Green),
           new PositionColorVertex(new Vector2(0.0f, 0.5f), Color.Blue)
        ];

        var resourceUploader = new ResourceUploader(GraphicsDevice);
        _vertexBuffer = resourceUploader.CreateBuffer(vertexData, BufferUsageFlags.Vertex);

        resourceUploader.Upload();
        resourceUploader.Dispose();
    }

    protected override void Update(TimeSpan delta)
    {
    }

    protected override void Draw(double alpha)
    {
        var cmdbuf = GraphicsDevice.AcquireCommandBuffer();
        var swapchainTexture = cmdbuf.AcquireSwapchainTexture(MainWindow);
        if (swapchainTexture != null)
        {
            var renderPass = cmdbuf.BeginRenderPass(
                new ColorTargetInfo(swapchainTexture, Color.Black)
            );
            renderPass.BindGraphicsPipeline(_pipeline);
            renderPass.BindVertexBuffers(_vertexBuffer);
            renderPass.DrawPrimitives(3, 1, 0, 0);
            cmdbuf.EndRenderPass(renderPass);
        }
        GraphicsDevice.Submit(cmdbuf);
    }
}
