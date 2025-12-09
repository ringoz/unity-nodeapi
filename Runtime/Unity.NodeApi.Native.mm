#include <Foundation/NSBundle.h>
#include <AppKit/NSApplication.h>
#include <AppKit/NSScreen.h>
#include <QuartzCore/CADisplayLink.h>
#include <objc/runtime.h>

static NSBundle *mainBundleFix;

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

extern "C" void NSBundle_fixup(const char *path)
{
  @autoreleasepool
  {
    mainBundleFix = [NSBundle bundleWithPath:[NSString stringWithUTF8String:path]];
    [NSBundle fixup];

    [[NSClassFromString(@"NSApplication") sharedApplication] setActivationPolicy:NSApplicationActivationPolicyRegular];    
  }
}

extern "C" void display_link(void (*cb)(void *), void *data)
{
  @autoreleasepool
  {
    [[NSBundle bundleWithPath:@"/System/Library/Frameworks/AppKit.framework"] load];
    CADisplayLink *link = [[NSClassFromString(@"NSScreen") mainScreen] displayLinkWithTarget:[^{cb(data);} copy] selector:@selector(invoke)];
    [link addToRunLoop:[NSRunLoop mainRunLoop] forMode:NSRunLoopCommonModes];
  }
}
