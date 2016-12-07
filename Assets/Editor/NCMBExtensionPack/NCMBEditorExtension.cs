using NCMB;
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// NCMBログイン中にエディターの再生モードを停止した際、ユーザーキャッシュデータを削除する//
/// </summary>
/// 
[InitializeOnLoad]//エディタ起動時に実行される//
public static class NCMBEditorExtension
{
    private static bool startPlaying = false;

    static NCMBEditorExtension()
    {
        EditorApplication.update += (() =>
         {
             if (startPlaying)
             {
                 if (EditorApplication.isPlaying == false)
                 {
                     string filePath = Application.persistentDataPath + "/" + "currentUser";
                    
                     if (File.Exists(filePath))
                     {
                         File.Delete(filePath);
                     }

                     Debug.Log("NCMBログイン中にエディターの再生モードが停止されたため、currentUserキャッシュファイルの削除を行いました。");
                     startPlaying = false;
                 }
             }
             else
             {
                 if (EditorApplication.isPlayingOrWillChangePlaymode == true)
                 {
                     startPlaying = true;
                     //Debug.Log("StartPlaying");
                 }
             }
         });
    }
    
}