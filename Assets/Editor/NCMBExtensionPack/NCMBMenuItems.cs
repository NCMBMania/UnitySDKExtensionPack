using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
///  WebView呼び出し参考：　http://qiita.com/kyusyukeigo/items/71db22676c6f4743913e
/// </summary>

namespace NCMB
{
    public class NCMBMenuItems : EditorWindow
    {
        private static BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        [MenuItem("NCMB/管理画面を開く")]
        private static void OpenConsole()
        {
            GetUnityEditorWebViewinfo().Invoke(null, new object[]{
            "NCMB管理画面",
            "https://console.mb.cloud.nifty.com/",
            200, 530, 1280, 720
        });
        }

        [MenuItem("NCMB/管理画面をブラウザで開く")]
        private static void OpenConsoleWithBrowser()
        {
            Application.OpenURL("https://console.mb.cloud.nifty.com/");
        }

        [MenuItem("NCMB/ドキュメントをブラウザで開く")]
        private static void OpenDocumentWithBrowser()
        {
            Application.OpenURL("http://mb.cloud.nifty.com/doc/current/index.html#/Unity");
        }

        private static MethodInfo GetUnityEditorWebViewinfo()
        {
#if UNITY_5_4_OR_NEWER
            Type type = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.Web.WebViewEditorWindowTabs");
            MethodInfo methodInfo = type.GetMethod("Create", Flags);
            return methodInfo.MakeGenericMethod(type);
#else
            Type type = Types.GetType("UnityEditor.Web.WebViewEditorWindow", "UnityEditor.dll");
            MethodInfo methodInfo = type.GetMethod("Create", Flags);
            return methodInfo.MakeGenericMethod(typeof(NCMBMenuItems));
#endif
        }
    }
}