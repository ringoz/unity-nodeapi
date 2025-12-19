using UnityEditor;
using UnityEditor.Build;

class UnityNodeApiBuild : IPreprocessBuildWithContext, IPostprocessBuildWithContext
{
  string m_il2cppArgs;
  string m_emsdkArgs;

  public int callbackOrder { get => 0; }

  public void OnPreprocessBuild(BuildCallbackContext ctx)
  {
    m_il2cppArgs = PlayerSettings.GetAdditionalIl2CppArgs();
    m_emsdkArgs = PlayerSettings.WebGL.emscriptenArgs;

    switch (ctx.Report.summary.platform)
    {
      case BuildTarget.StandaloneWindows64:
        PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs + " --linker-flags=\"Packages/net.ringoz.unity.nodeapi/Runtime/lib/fcontext.lib\"");
        break;
      case BuildTarget.StandaloneOSX:
        PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs + " --linker-flags=\"-lfcontext -LPackages/net.ringoz.unity.nodeapi/Runtime/lib\"");
        break;
      case BuildTarget.WebGL:
        PlayerSettings.WebGL.emscriptenArgs = m_emsdkArgs + " -sEXPORTED_FUNCTIONS=['_malloc','_free','_napi_register_wasm_v1','_node_api_module_get_api_version_v1'] -sEXTRA_EXPORTED_RUNTIME_METHODS=['emnapiInit']";
        break;
    }
  }

  public void OnPostprocessBuild(BuildCallbackContext ctx)
  {
    PlayerSettings.SetAdditionalIl2CppArgs(m_il2cppArgs);
    PlayerSettings.WebGL.emscriptenArgs = m_emsdkArgs;
  }
}
