using GlyphFX.Core;
using GlyphFX.Desktop;

var bridge = new DesktopNativeRequestBridge();

var windowManager = new WindowManager(bridge);
windowManager.RunLoop();