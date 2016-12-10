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