/*
Copyright (c) 2016-2017 Takaaki Ichijo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using NCMB;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
///  WebView呼び出し参考：　http://qiita.com/kyusyukeigo/items/71db22676c6f4743913e
/// </summary>

namespace NCMBExtension
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

        [MenuItem("NCMB/ログイン中のユーザー名を確認")]
        private static void CheckCurrentUser()
        {
            if (NCMBUser.CurrentUser != null && EditorApplication.isPlaying)
            {
                string name = NCMBUser.CurrentUser.UserName;
                string objectId = NCMBUser.CurrentUser.ObjectId;

                Debug.Log("UserName: " + name + ", ObjectId: " + objectId);
            }
            else
            {
                Debug.Log("Not Logged in");
            }
        }
    }
}