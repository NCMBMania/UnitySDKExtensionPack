/*
Copyright (c) 2016-2017 Takaaki Ichijo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEditor;
using UnityEngine;
using System.IO;
using NCMB;

/// <summary>
/// NCMBログイン中にエディターの再生モードを停止した際、ユーザーキャッシュデータを削除する//
/// NCMBUser._logOutEvent() と同じことをエディター終了時に行う。
/// </summary>
/// 
namespace NCMBExtension
{
    [InitializeOnLoad]//エディタ起動時に実行される//
    public static class NCMBEditorExtension
    {
        static NCMBEditorExtension()
        {
            EditorApplication.playmodeStateChanged += (() =>
            {
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    Debug.Log("再生停止");
                    string filePath = Application.persistentDataPath + "/" + "currentUser";

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Debug.Log("NCMBログイン中にエディターの再生モードが停止されたため、currentUserキャッシュファイルの削除を行いました。");
                    }
                }
            });
        }
    }
}