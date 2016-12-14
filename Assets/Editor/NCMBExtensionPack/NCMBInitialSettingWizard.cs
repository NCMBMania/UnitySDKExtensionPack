using NCMB;
using UnityEditor;
using UnityEngine;

namespace NCMBExtension
{
    public class NCMBInitialSettingWizard : ScriptableWizard
    {
        private static NCMBInitialSettingWizard Instance;

        [Tooltip("NCMB管理画面の「アプリ設定」から取得する、アプリケーションキーを入力します")]
        [SerializeField]
        public string applicationKey = "";

        [Tooltip("NCMB管理画面の「アプリ設定」から取得する、クライアントキーを入力します")]
        public string clientKey = "";

        [Tooltip("プッシュ通知を利用する場合にオンにします")]
        public bool usePush = false;

        [Tooltip("プッシュの開封通知を利用する場合にオンにします")]
        public bool useAnalytics = false;

        [Tooltip("Androidでプッシュ通知を行う場合、Google Developers ConsoleのSender IDを入力します")]
        public string androidSenderId = "";

        [Tooltip("レスポンスが改ざんされていないか判定する機能を有効にします")]
        public bool responseValidation = false;

        [Tooltip("APIキーを暗号化して外部ファイルに保存するオプションです")]
        public bool useAPIKeyFile = true;

        [Tooltip("APIキーファイルを保存するパスです")]
        public string apiKeyFilePath = "Bin/NCMBAPIKey";

        [MenuItem("NCMB/初期設定ウィザードを開く", false, 0)]
        private static void Open()
        {
            if(Instance == null)
            {
                Instance = DisplayWizard<NCMBInitialSettingWizard>("NCMB初期設定ウィザード", "NCMBのオブジェクトを生成する");

                Instance.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/NCMBExtensionPack/Textures/icon.png");
            }

        }

        private void OnWizardCreate()
        {
            NCMBSettingsExtended ncmbSettingsEx = FindObjectOfType<NCMBSettingsExtended>();

            if (ncmbSettingsEx == null)
            {
                GameObject ncmbSettingObject = new GameObject("NCMBSettingsExtended");
                ncmbSettingsEx = ncmbSettingObject.AddComponent<NCMBSettingsExtended>();
            }

            ncmbSettingsEx.Initialize(applicationKey, clientKey, usePush, useAnalytics, androidSenderId, responseValidation);

            if (useAPIKeyFile)
            {
                if (string.IsNullOrEmpty(apiKeyFilePath))
                {
                    Debug.Log("ファイルパスが空です");
                    return;
                }

                NCMBGenerateAPIKeyFile.GenerateAPIKeyFile(applicationKey, clientKey, apiKeyFilePath);
                ncmbSettingsEx.EnableToUseAPIKeyFile(apiKeyFilePath);
            }
            else
            {
                ncmbSettingsEx.DisableToUseAPIKeyFile();
            }

            //NCMB Managerもなかったら作る//
            if (FindObjectOfType<NCMBManager>() == null)
            {
                new GameObject("NCMBManager").AddComponent<NCMBManager>();
            }
        }
    }
}