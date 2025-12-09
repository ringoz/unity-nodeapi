#include <Foundation/NSBundle.h>
#include <AppKit/NSApplication.h>
#include <AppKit/NSScreen.h>
#include <QuartzCore/CADisplayLink.h>
#include <objc/runtime.h>

static NSBundle *mainBundleFix;
static auto appKit = [[NSBundle bundleWithPath:@"/System/Library/Frameworks/AppKit.framework"] load];

@implementation NSBundle(BundleFixup)

+(void)fixup
{
  Method originalMethod = class_getClassMethod(self, @selector(mainBundle));
  Method extendedMethod = class_getClassMethod(self, @selector(mainBundleFix));
  method_exchangeImplementations(originalMethod, extendedMethod);
}

+(NSBundle *)mainBundleFix
{
  return mainBundleFix;
}

@end

extern "C" void app_init(const char *path)
{
  @autoreleasepool
  {
    mainBundleFix = [NSBundle bundleWithPath:[NSString stringWithUTF8String:path]];
    [NSBundle fixup];
  }
}

extern "C" void app_activate(void)
{
  @autoreleasepool
  {
    NSApplication *app = [NSClassFromString(@"NSApplication") sharedApplication];
    [app setActivationPolicy:NSApplicationActivationPolicyRegular];    
    [app activateIgnoringOtherApps:YES];
  }
}

extern "C" void display_link(void (*cb)(void *), void *data)
{
  @autoreleasepool
  {
    CADisplayLink *link = [[NSClassFromString(@"NSScreen") mainScreen] displayLinkWithTarget:[^{cb(data);} copy] selector:@selector(invoke)];
    [link addToRunLoop:[NSRunLoop mainRunLoop] forMode:NSRunLoopCommonModes];
  }
}
