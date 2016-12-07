using NCMB;
using UnityEditor;
using UnityEngine;

namespace NCMBExtension
{
    public class NCMBWizard : ScriptableWizard
    {
        private static NCMBWizard ncmbWizard;

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

        [Tooltip("アプリキーを暗号化して外部ファイルに保存するオプションです")]
        public bool useExternalCryptedKeyFile = true;

        [MenuItem("NCMB/アプリキー設定ウィザードを開く")]
        private static void Open()
        {
            DisplayWizard<NCMBWizard>("NCMB Wizard", "Create GameObject");
        }

        private void OnWizardCreate()
        {
            NCMBSettingsExtended.CreateAndInitiarize(applicationKey, clientKey, usePush, useAnalytics, androidSenderId, responseValidation, useExternalCryptedKeyFile);

            //NCMB Managerもなかったら作る//
            if (GameObject.FindObjectOfType<NCMBManager>() == null)
            {
                new GameObject("NCMBManager").AddComponent<NCMBManager>();
            }
        }
    }
}