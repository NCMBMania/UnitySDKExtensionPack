using NCMB;
using System;
using UnityEngine;

namespace NCMBExtension
{
    public class NCMBSettingsExtended : NCMBSettings
    {
        public bool useAPIKeyFile = true;
        public string apiKeyFilePath = "Bin/NCMBAPIKey";

        public override void Awake()
        {
            if (useAPIKeyFile && !string.IsNullOrEmpty(apiKeyFilePath))
            {
                APIKey ncmbKeyPair = new APIKey();
                TextAsset textAssets = Resources.Load(apiKeyFilePath) as TextAsset;

                if (textAssets == null)
                {
                    Debug.LogError("APIキーファイルが存在しません。NCMBメニューからAPIキーファイルを作成して下さい。");
                }
                else
                {
                    string strdata = FileAESCrypter.DecryptFromBytes(textAssets.bytes);
                    ncmbKeyPair = JsonUtility.FromJson<APIKey>(strdata);

                    applicationKey = ncmbKeyPair.applicationKey;
                    clientKey = ncmbKeyPair.clientKey;
                }
            }

            base.Awake();
        }

        /// <summary>
        /// ウィザードからNCMBSettings生むためのもの//
        /// </summary>
        /// <param name="applicationKey"></param>
        /// <param name="clientKey"></param>
        /// <param name="usePush"></param>
        /// <param name="useAnalytics"></param>
        /// <param name="androidSenderId"></param>
        /// <param name="responseValidation"></param>
        public void Initialize(string applicationKey, string clientKey, bool usePush, bool useAnalytics, string androidSenderId, bool responseValidation)
        {
            this.applicationKey = applicationKey;
            this.clientKey = clientKey;
            this.usePush = usePush;
            this.useAnalytics = useAnalytics;
            this.androidSenderId = androidSenderId;
            this.responseValidation = responseValidation;
        }

        public void EnableToUseAPIKeyFile(string apiKeyFilePath)
        {
            this.applicationKey = string.Empty;
            this.clientKey = string.Empty;

            this.apiKeyFilePath = apiKeyFilePath;
            this.useAPIKeyFile = true;
        }

        public void DisableToUseAPIKeyFile()
        {
            this.apiKeyFilePath = string.Empty;
            this.useAPIKeyFile = false;
        }
    }
}

[Serializable]
public class APIKey //←Json化後にクラス名を含んでresources.assetsに入るので、難読化の場合はクラス名を変更してください//
{
    [SerializeField]
    public string applicationKey, clientKey;
}