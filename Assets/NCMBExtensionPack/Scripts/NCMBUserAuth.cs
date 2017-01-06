/*
Copyright (c) 2016-2017 Takaaki Ichijo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using NCMB;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// コールバック付きログイン・サインイン//
/// パスワードを自動発行するサインイン//
/// 自動発行したパスワードをpersistentDataPathに保存//
/// </summary>

namespace NCMBExtension
{
    public class NCMBUserAuth : MonoBehaviour
    {
        private static string logindataFileName = "Logindata.bin";
        private static string loginDataFilePath;

        public void Awake()
        {
            //PC  C:/Users/xxxx/AppData/LocalLow/CompanyName/ProductName//
            loginDataFilePath = Application.persistentDataPath + "/" + logindataFileName;

#if UNITY_iOS
            //iOS var/mobile/Containers/Data/Application/xxxxxxxx - xxxx - xxxx - xxxx - xxxxxxxxxxxx/Library/
            loginDataFilePath = Application.persistentDataPath + "/../Library/" + logindataFileName;

            //iCloud バックアップ不要ファイルに設定する//
            UnityEngine.iOS.Device.SetNoBackupFlag(loginDataFilePath);
#endif
        }

        public void Login(string userName, string password, EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            //ネット接続があるか？//
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);
                //if (failureEventHandler != null) failureEventHandler(new ConnectionEventArgs("ネットワークに接続していません。"));

                Debug.Log("ネットワークに接続していません。");

                return;
            }

            // ユーザー名とパスワードでログイン
            NCMBUser.LogInAsync(userName, password, (NCMBException e) =>
            {
                if (e != null)
                {
                    Debug.Log("ログインに失敗: " + e.ErrorMessage);

                    if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);
                }
                else
                {
                    Debug.Log("ログインに成功！");

                    if (successEventHandler != null) successEventHandler(this, EventArgs.Empty);
                }
            });
        }

        public bool IsLoginDataExist()
        {
            return File.Exists(loginDataFilePath);
        }

        public string GetUserNameFromLoginData()
        {
            string strdata;
            FileAESCrypter.DecryptFromFile(out strdata, loginDataFilePath);

            NCMBLoginData logindata = JsonUtility.FromJson<NCMBLoginData>(strdata);

            return logindata.userName;
        }

        public string GetPasswordFromLoginData()
        {
            string strdata;
            FileAESCrypter.DecryptFromFile(out strdata, loginDataFilePath);

            NCMBLoginData logindata = JsonUtility.FromJson<NCMBLoginData>(strdata);

            return logindata.password;
        }

        public void DeleteLoginData()
        {
            File.Delete(loginDataFilePath);
        }

        public void DeleteCurrentAccount(EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            //ネット接続があるか？//
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);

                Debug.Log("ネットワークに接続していません。");

                return;
            }

            //ユーザーを削除します//
            NCMBUser.CurrentUser.DeleteAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);
                }
                else
                {
                    if (successEventHandler != null) successEventHandler(this, EventArgs.Empty);
                }
            });
        }

        public void AutoSignin(EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            string generatedUserName = Guid.NewGuid().ToString("N").Substring(0, 8);
            AutoSignin(generatedUserName, successEventHandler, failureEventHandler);
        }

        public void AutoSignin(string userName, EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            string generatedPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            Debug.Log("new pass" + generatedPassword);

            //ログイン成功した場合にLoginDataの保存処理を行う//
            successEventHandler += new EventHandler((sender, e) => SaveLoginData(userName, generatedPassword));

            Signin(userName, generatedPassword, successEventHandler, failureEventHandler);
        }

        private void SaveLoginData(string userName, string password)
        {
            NCMBLoginData logindata = new NCMBLoginData();
            logindata.userName = userName;
            logindata.password = password;

            string strdata = JsonUtility.ToJson(logindata);

            FileAESCrypter.EncryptToFile(strdata, loginDataFilePath);
        }

        public void AutoLogin(EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            string strdata;
            FileAESCrypter.DecryptFromFile(out strdata, loginDataFilePath);

            NCMBLoginData logindata = JsonUtility.FromJson<NCMBLoginData>(strdata);

            Login(logindata.userName, logindata.password, successEventHandler, failureEventHandler);
        }

        public void Signin(string userName, string password, EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            //ネット接続があるか？//
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);

                Debug.Log("ネットワークに接続していません。");

                return;
            }

            //NCMBUserのインスタンス作成
            NCMBUser user = new NCMBUser();

            //ユーザ名とパスワードの設定
            user.UserName = userName;
            user.Password = password;

            //会員登録を行う
            user.SignUpAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);
                    Debug.Log("新規登録に失敗: " + e.ErrorMessage);
                }
                else
                {
                    if (successEventHandler != null) successEventHandler(this, EventArgs.Empty);
                    Debug.Log("新規登録に成功");
                }
            });
        }

        public void Logout(EventHandler successEventHandler = null, EventHandler failureEventHandler = null)
        {
            //ネット接続があるか？//
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);

                Debug.Log("ネットワークに接続していません。");

                return;
            }

            NCMBUser.LogOutAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    if (failureEventHandler != null) failureEventHandler(this, EventArgs.Empty);
                    Debug.Log("ログアウトに失敗！");
                }
                else
                {
                    if (successEventHandler != null) successEventHandler(this, EventArgs.Empty);
                    Debug.Log("ログアウトに成功！");
                }
            });
        }
    }

    [Serializable]
    public class NCMBLoginData
    {
        [SerializeField]
        public string userName, password;
    }
}