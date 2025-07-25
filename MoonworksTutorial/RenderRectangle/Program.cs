using RenderRectangle;
using MoonWorks;

internal class Program
{
    static void Main()
    {
        var windowCreateInfo = new WindowCreateInfo(
             "Rectangle Example Game",
             800,
             600,
             ScreenMode.Windowed
         );

        var framePacingSettings = FramePacingSettings.CreateLatencyOptimized(60);

        var game = new BasicRectangleGame(
            new AppInfo("Rectangle Example Game", "RectangleExampleGame"),
            windowCreateInfo,
            framePacingSettings);

        game.Run();
    }
}