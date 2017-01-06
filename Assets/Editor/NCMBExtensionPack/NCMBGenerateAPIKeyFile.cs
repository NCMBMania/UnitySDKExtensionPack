/*
Copyright (c) 2016-2017 Takaaki Ichijo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEditor;
using UnityEngine;

namespace NCMBExtension
{
    public class NCMBGenerateAPIKeyFile : ScriptableWizard
    {
        private static NCMBGenerateAPIKeyFile Instance;

        [Tooltip("NCMB管理画面の「アプリ設定」から取得する、アプリケーションキーを入力します")]
        [SerializeField]
        public string applicationKey = "";

        [Tooltip("NCMB管理画面の「アプリ設定」から取得する、クライアントキーを入力します")]
        public string clientKey = "";

        [Tooltip("APIキーファイルを保存するパスです")]
        public string apiKeyFilePath = "Bin/NCMBAPIKey";

        [MenuItem("NCMB/APIキーファイルを再作成する", false, 0)]
        private static void Open()
        {
            if (Instance == null)
            {
                Instance = DisplayWizard<NCMBGenerateAPIKeyFile>("NCMB APIキーファイル再作成", "APIキーファイルを生成", "シーン内のNCMBSettingsExtentedからキーを読み込む");

                Instance.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/NCMBExtensionPack/Textures/icon.png");
            }
        }

        private void OnWizardCreate()
        {
            GenerateAPIKeyFile(applicationKey, clientKey, apiKeyFilePath);

            NCMBSettingsExtended ncmbSettingsEx = FindObjectOfType<NCMBSettingsExtended>();

            if (ncmbSettingsEx == null)
            {
                Debug.Log("NCMBSettingsExtendedが存在しません。初期設定ウィザードから作成して下さい。");
            }
            else
            {
                ncmbSettingsEx.EnableToUseAPIKeyFile(apiKeyFilePath);
            }
        }

        private void OnWizardOtherButton()
        {
            NCMBSettingsExtended ncmbSettingsEx = FindObjectOfType<NCMBSettingsExtended>();

            if (ncmbSettingsEx != null)
            {
                applicationKey = ncmbSettingsEx.GetApplicationKey();
                clientKey = ncmbSettingsEx.GetClientKey();
            }
            else
            {
                Debug.Log("NCMBSettingsExtended が存在しません。");
            }
        }

        public static void GenerateAPIKeyFile(string applicationKey, string clientKey, string apiKeyFilePath)
        {
            if (applicationKey.Length != 64 || clientKey.Length != 64)
            {
                Debug.Log("APIキーの値が不正です。");
            }
            else
            {
                APIKey apiKey = new APIKey();
                apiKey.applicationKey = applicationKey;
                apiKey.clientKey = clientKey;

                string jsondata = JsonUtility.ToJson(apiKey);
                string path = "/Resources/" + apiKeyFilePath + ".bytes";

                FileAESCrypter.EncryptToFile(jsondata, Application.dataPath + path);

                AssetDatabase.ImportAsset("Assets" + path);
                Debug.Log("APIキーの生成を行いました。");
            }
        }
    }
}