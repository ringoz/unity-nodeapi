#include <codegen/il2cpp-codegen-metadata.h>
#include <utils/Exception.h>
#include <os/Image.h>
#include <Cpp/Baselib_DynamicLibrary.h>
#include <fcontext.h>

extern "C" char *GC_approx_sp(void);
extern "C" char *GC_get_main_stack_base(void);
extern "C" char *GC_stackbottom;
typedef void (* GC_push_other_roots_proc)(void);
extern "C" void GC_set_push_other_roots(GC_push_other_roots_proc);
extern "C" GC_push_other_roots_proc GC_get_push_other_roots(void);
extern "C" void GC_push_all_stack_sections(char *lo, char *hi, void *);
typedef void *(* GC_fn_type)(void *);
extern "C" void *GC_call_with_alloc_lock(GC_fn_type fn, void *client_data);

typedef struct uv_loop_s { void* data; } uv_loop_t;
typedef struct uv_idle_s { void* data; uv_loop_t* loop; } uv_idle_t;
typedef void (*uv_idle_cb)(uv_idle_t* handle);
static int (*uv_idle_init)(uv_loop_t*, uv_idle_t* idle);
static int (*uv_idle_start)(uv_idle_t* idle, uv_idle_cb cb);

typedef struct napi_env__* napi_env;
typedef enum { napi_ok } napi_status;
static napi_status (*napi_get_uv_event_loop)(napi_env env, struct uv_loop_s** loop);

using namespace il2cpp;

extern "C" void UnityFiberProc()
{
  std::string imagePath = os::Image::GetImageName();
  imagePath = imagePath.substr(0, imagePath.find_last_of(IL2CPP_DIR_SEPARATOR));
#if defined(__APPLE__)
  imagePath += "/UnityPlayer.dylib";
#elif defined(_WIN32)
  imagePath += "\\UnityPlayer.dll";
#endif

  auto errorState = Baselib_ErrorState_Create();
  auto playerLib = Baselib_DynamicLibrary_Open(imagePath.c_str(), &errorState);
  if (Baselib_ErrorState_ErrorRaised(&errorState))
  {
    printf("error: %s\n", utils::Exception::FormatBaselibErrorState(errorState).c_str());
    abort();
  }

#if defined(__APPLE__)
  extern int PlayerMain(int argc, const char *argv[]);
  auto playerProc = (decltype(&PlayerMain))Baselib_DynamicLibrary_GetFunction(playerLib, "_Z10PlayerMainiPPKc", &errorState);
#elif defined(_WIN32)
  extern int UnityMain2(void *hInstance, const wchar_t *customDataFolder, wchar_t *lpCmdLine, int nShowCmd);
  auto playerProc = (decltype(&UnityMain2))Baselib_DynamicLibrary_GetFunction(playerLib, "UnityMain2", &errorState);
#endif

  if (!playerProc)
  {
    printf("error: %s\n", utils::Exception::FormatBaselibErrorState(errorState).c_str());
    abort();
  }

#if defined(__APPLE__)
  imagePath = imagePath.substr(0, imagePath.find_last_of(IL2CPP_DIR_SEPARATOR));
  imagePath = imagePath.substr(0, imagePath.find_last_of(IL2CPP_DIR_SEPARATOR));
  imagePath = imagePath.substr(0, imagePath.find_last_of(IL2CPP_DIR_SEPARATOR));
  extern void NSBundle_fixup(const char *path);
  NSBundle_fixup(imagePath.c_str());
  
  const char *argv[] = { os::Image::GetImageName(), "-logfile", "-" };
  playerProc(1, argv);
#elif defined(_WIN32)
  imagePath = imagePath.substr(0, imagePath.find_last_of(IL2CPP_DIR_SEPARATOR));
  imagePath += "\\unode-module_Data";
  auto dataPath = utils::StringUtils::Utf8ToNativeString(imagePath);

  extern IMPORTED_SYMBOL int SetProcessDpiAwarenessContext(int);
  SetProcessDpiAwarenessContext(-4);

  extern IMPORTED_SYMBOL void *GetModuleHandleW(wchar_t *);
  playerProc(GetModuleHandleW(nullptr), dataPath.c_str(), L"-logfile -", 5);
#endif

  Baselib_DynamicLibrary_Close(playerLib);
}

static char *GC_stackbottom_alt;
static char *GC_stacktop_alt;

static const auto GC_push_other_roots = GC_get_push_other_roots();
static void MY_push_other_roots(void)
{
  GC_push_all_stack_sections(GC_stacktop_alt, GC_stackbottom_alt, nullptr);
  GC_push_other_roots();
}

static fcontext_t MY_fiber;
static fcontext_t MY_fiber_alt;

static IL2CPP_NO_INLINE void node_jump(uv_idle_t *context)
{
  auto ctx = (fcontext_t *)GC_call_with_alloc_lock([](void *context) -> void *
  {
    auto GC_stacktop = GC_approx_sp();
    std::swap(GC_stacktop, GC_stacktop_alt);
    std::swap(GC_stackbottom, GC_stackbottom_alt);
    std::swap(MY_fiber, MY_fiber_alt);
    return jump_fcontext(MY_fiber, nullptr).ctx;
  }, nullptr);
  
  if (MY_fiber_alt)
    MY_fiber_alt = ctx;
}

extern "C" EXPORTED_SYMBOL void *napi_register_module_v1(napi_env env, void *exports)
{
  fcontext_stack_t s = create_fcontext_stack(1024 * 1024);
  MY_fiber_alt = make_fcontext(s.sptr, s.ssize, [](fcontext_transfer_t t)
  {
    MY_fiber_alt = t.ctx;
    UnityFiberProc();
    MY_fiber = nullptr;
    node_jump({});
  });

  GC_stackbottom_alt = (char *)s.sptr;
  GC_stackbottom = GC_get_main_stack_base();
  GC_set_push_other_roots(MY_push_other_roots);

  extern void display_link(void (*cb)(uv_idle_t *), uv_idle_t *data);
  display_link(&node_jump, nullptr);
  
  atexit([]{ quick_exit(0); });
  node_jump({});

  extern void app_activate(void);
  app_activate();

  auto errorState = Baselib_ErrorState_Create();
  auto progHandle = Baselib_DynamicLibrary_OpenProgramHandle(&errorState);
  napi_get_uv_event_loop = (decltype(napi_get_uv_event_loop))Baselib_DynamicLibrary_GetFunction(progHandle, "napi_get_uv_event_loop", &errorState);
  uv_idle_init = (decltype(uv_idle_init))Baselib_DynamicLibrary_GetFunction(progHandle, "uv_idle_init", &errorState);
  uv_idle_start = (decltype(uv_idle_start))Baselib_DynamicLibrary_GetFunction(progHandle, "uv_idle_start", &errorState);
  if (Baselib_ErrorState_ErrorRaised(&errorState))
  {
    printf("error: %s\n", utils::Exception::FormatBaselibErrorState(errorState).c_str());
    return nullptr;
  }
  Baselib_DynamicLibrary_Close(progHandle);

  uv_loop_t *loop;
  napi_get_uv_event_loop(env, &loop);

  static uv_idle_t idler;
  uv_idle_init(loop, &idler);
  uv_idle_start(&idler, &node_jump);

  extern const Il2CppMethodPointer g_ReversePInvokeWrapperPointers[];
  extern const Il2CppCodeGenModule g_Unity_NodeApi_CodeGenModule;

  auto func = g_ReversePInvokeWrapperPointers[g_Unity_NodeApi_CodeGenModule.reversePInvokeWrapperIndices[0].index];
  return reinterpret_cast<decltype(&napi_register_module_v1)>(func)(env, exports);
}
