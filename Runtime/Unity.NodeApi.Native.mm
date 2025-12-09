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
    id callback = [^{cb(data);} copy];
    CADisplayLink *link = [[NSClassFromString(@"NSScreen") mainScreen] displayLinkWithTarget:callback selector:@selector(invoke)];
    [link addToRunLoop:[NSRunLoop mainRunLoop] forMode:NSRunLoopCommonModes];
  }
}
