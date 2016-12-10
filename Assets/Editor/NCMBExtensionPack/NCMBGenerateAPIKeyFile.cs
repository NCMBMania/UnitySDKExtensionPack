using UnityEditor;
using UnityEngine;

namespace NCMBExtension
{
    public class NCMBGenerateAPIKeyFile : ScriptableWizard
    {
        private static NCMBInitialSettingWizard ncmbWizard;

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
            DisplayWizard<NCMBGenerateAPIKeyFile>("APIキーの再作成", "Generate File");
        }

        private void OnWizardCreate()
        {
            GenerateAPIKeyFile(applicationKey, clientKey, apiKeyFilePath);

            NCMBSettingsExtended ncmbSettingsEx = FindObjectOfType<NCMBSettingsExtended>();

            if (ncmbSettingsEx == null)
            {
                Debug.Log("NCMBSettingsExtendedが存在しません。ウィザードから作成して下さい。");
            }
            else
            {
                ncmbSettingsEx.EnableToUseAPIKeyFile(apiKeyFilePath);
            }
        }

        public static void GenerateAPIKeyFile(string applicationKey, string clientKey, string apiKeyFilePath)
        {
            APIKey apiKey = new APIKey();
            apiKey.applicationKey = applicationKey;
            apiKey.clientKey = clientKey;

            string jsondata = JsonUtility.ToJson(apiKey);
            string path = "/Resources/" + apiKeyFilePath + ".bytes";

            FileAESCrypter.EncryptToFile(jsondata, Application.dataPath + path);

            AssetDatabase.ImportAsset("Assets" + path);
        }
    }
}