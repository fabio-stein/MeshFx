using GlyphFX.Desktop;
using GlyphFX.Examples;

var bridge = new DesktopNativeRequestBridge();
var exampleApp = new ExampleHandlers(bridge);
exampleApp.RunExample();