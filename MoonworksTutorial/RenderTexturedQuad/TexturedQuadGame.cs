using MoonWorks;
using MoonWorks.Graphics;
using System.Numerics;
using System.Runtime.InteropServices;
using Buffer = MoonWorks.Graphics.Buffer;

namespace RenderTexturedQuad;

[StructLayout(LayoutKind.Sequential)]
public struct PositionTextureVertex(Vector2 position, Vector2 texCoord) : IVertexType
{
    public Vector2 Position = position;
    public Vector2 TexCoord = texCoord;

    public static VertexElementFormat[] Formats =>
    [
        VertexElementFormat.Float2,
        VertexElementFormat.Float2
    ];


    public static uint[] Offsets { get; } = [0, 8];
}

public class TexturedQuadGame : Game
{
    private Texture _texture;
    private readonly Sampler _sampler;
    private readonly Buffer _indexBuffer;
    private readonly Buffer _vertexBuffer;
    private readonly GraphicsPipeline _pipeline;

    public TexturedQuadGame(
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
           "Content/Shaders/TexturedQuad.vert.hlsl",
           "main",
           ShaderCross.ShaderFormat.HLSL,
           ShaderStage.Vertex);

        var fragmentShader = ShaderCross.Create(
            GraphicsDevice,
            RootTitleStorage,
            "Content/Shaders/TexturedQuad.frag.hlsl",
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

        pipelineCreateInfo.VertexInputState = VertexInputState.CreateSingleBinding<PositionTextureVertex>();
        _pipeline = GraphicsPipeline.Create(GraphicsDevice, pipelineCreateInfo);

        ReadOnlySpan<PositionTextureVertex> vertexData = [
            new PositionTextureVertex(new Vector2(-1,  1), new Vector2(0, 0)),
            new PositionTextureVertex(new Vector2( 1,  1), new Vector2(4, 0)),
            new PositionTextureVertex(new Vector2( 1, -1), new Vector2(4, 4)),
            new PositionTextureVertex(new Vector2(-1, -1), new Vector2(0, 4)),
        ];

        ReadOnlySpan<ushort> indexData = [
            0, 1, 2,
            0, 2, 3,
        ];

        var pngPath = "Content/Textures/ravioli.png";
        var resourceUploader = new ResourceUploader(GraphicsDevice);

        _indexBuffer = resourceUploader.CreateBuffer(indexData, BufferUsageFlags.Index);
        _vertexBuffer = resourceUploader.CreateBuffer(vertexData, BufferUsageFlags.Vertex);

        _texture = resourceUploader.CreateTexture2DFromCompressed(
            RootTitleStorage,
            pngPath,
            TextureFormat.R8G8B8A8Unorm,
            TextureUsageFlags.Sampler);

        _sampler = Sampler.Create(GraphicsDevice, SamplerCreateInfo.PointClamp);

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
            renderPass.BindIndexBuffer(_indexBuffer, IndexElementSize.Sixteen);
            renderPass.BindFragmentSamplers(new TextureSamplerBinding(_texture, _sampler));
            renderPass.DrawIndexedPrimitives(6, 1, 0, 0, 0);
            cmdbuf.EndRenderPass(renderPass);
        }
        GraphicsDevice.Submit(cmdbuf);
    }
}
