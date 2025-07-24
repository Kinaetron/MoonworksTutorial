using MoonWorks;
using MoonWorks.Graphics;

namespace RenderColoredTriangle;
public class ColoredTriangleGame : Game
{
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
    }

    protected override void Update(TimeSpan delta)
    {
    }

    protected override void Draw(double alpha)
    {
    }
}
