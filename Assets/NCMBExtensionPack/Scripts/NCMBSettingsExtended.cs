/*
Copyright (c) 2016-2017 Takaaki Ichijo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
        /// ウィザードからNCMBSettingsを生成する//
        /// </summary>
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

        public string GetApplicationKey()
        {
            return applicationKey;
        }

        public string GetClientKey()
        {
            return clientKey;
        }
    }
}

[Serializable]
public class APIKey //Json化後にクラス名を含んでresources.assetsに入ります。難読化をする場合は、クラス名を変更してください//
{
    [SerializeField]
    public string applicationKey, clientKey;
}