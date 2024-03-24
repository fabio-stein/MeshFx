using GlyphFX.Core;
using GlyphFX.Desktop;

var bridge = new DesktopNativeRequestBridge();
var windowManager = new WindowManager(bridge);

new GlyphAppBuilder()
    .WithWindowManager(windowManager)
    .WithWindowEventHandler(new WindowEventHandler())
    .Build()
    .Run();