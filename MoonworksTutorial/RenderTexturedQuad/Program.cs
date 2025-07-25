using MoonWorks;
using RenderTexturedQuad;

internal class Program
{
    static void Main()
    {
        var windowCreateInfo = new WindowCreateInfo(
             "Textured Quad Example Game",
             800,
             600,
             ScreenMode.Windowed
         );

        var framePacingSettings = FramePacingSettings.CreateLatencyOptimized(60);

        var game = new TexturedQuadGame(
            new AppInfo("Textured Quad Game", "TexturedQuadGame"),
            windowCreateInfo,
            framePacingSettings);

        game.Run();
    }
}