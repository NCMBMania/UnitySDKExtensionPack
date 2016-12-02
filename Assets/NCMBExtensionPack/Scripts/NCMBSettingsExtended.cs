using System;
using UnityEngine;
using NCMB;

namespace NCMBExtension
{
    public class NCMBSettingsExtended : NCMBSettings
    {
        public static string keyJsonFileName = "Bin/NCMBKey";
        public bool useNCMBKeyPairFile = false;

        public override void Awake()
        {
            if (useNCMBKeyPairFile)
            {
                NCMBKeyPair ncmbKeyPair = new NCMBKeyPair();
                TextAsset textAssets = Resources.Load("Bin/NCMBKey") as TextAsset;

                if(textAssets == null)
                {
                    Debug.LogError("ログイン用のAPIキーファイルが存在しません。アプリキー設定ウィザードからファイルを作成して下さい。");
                }else
                {
                    string strdata;

                    FileAESCrypter.DecryptFromBytes(out strdata, textAssets.bytes);
                    ncmbKeyPair = JsonUtility.FromJson<NCMBKeyPair>(strdata);

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
        /// <param name="isGenerateCryptedKeyPairFile"></param>
        public static void CreateAndInitiarize(string applicationKey, string clientKey, bool usePush, bool useAnalytics, string androidSenderId, bool responseValidation, bool isGenerateCryptedKeyPairFile)
        {
            NCMBSettingsExtended ncmbSettingsEx = FindObjectOfType<NCMBSettingsExtended>();

            if (ncmbSettingsEx == null)
            {
                GameObject ncmbSettingObject = new GameObject("NCMBSettingsExtended");
                ncmbSettingsEx = ncmbSettingObject.AddComponent<NCMBSettingsExtended>();
            }

            if (isGenerateCryptedKeyPairFile)
            {
                NCMBKeyPair ncmbKeyPair = new NCMBKeyPair();
                ncmbKeyPair.applicationKey = applicationKey;
                ncmbKeyPair.clientKey = clientKey;

                string jsondata = JsonUtility.ToJson(ncmbKeyPair);
                Debug.Log("jsondata" + jsondata);
                FileAESCrypter.EncryptToFile(jsondata, Application.dataPath + "/Resources/" + keyJsonFileName + ".bytes");
                ncmbSettingsEx.applicationKey = string.Empty;
                ncmbSettingsEx.clientKey = string.Empty;
            }
            else
            {
                ncmbSettingsEx.applicationKey = applicationKey;
                ncmbSettingsEx.clientKey = clientKey;
            }

            ncmbSettingsEx.usePush = usePush;
            ncmbSettingsEx.useAnalytics = useAnalytics;
            ncmbSettingsEx.androidSenderId = androidSenderId;
            ncmbSettingsEx.responseValidation = responseValidation;
        }
    }
}

[Serializable]
public class NCMBKeyPair
{
    [SerializeField]
    public string applicationKey, clientKey;
}