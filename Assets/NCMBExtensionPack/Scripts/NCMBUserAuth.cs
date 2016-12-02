using NCMB;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// コールバック付きログイン・サインイン//
/// パスワードを自動発行するサインイン//
/// 自動発行したパスワードをpersistentDataPathに保存//
/// </summary>

//Application.persistentDataPath : C:/Users/xxxx/AppData/LocalLow/CompanyName/ProductName//
namespace NCMBExtended
{
    public class NCMBUserAuth : MonoBehaviour
    {
        private static string logindataFileName = "Logindata.bin";
        private static string loginDataFilePath;

        public void Awake()
        {
            loginDataFilePath = Application.persistentDataPath + "/" + logindataFileName;

#if UNITY_iOS
            //Application.temporaryCachePath
            // var/mobile/Containers/Data/Application/xxxxxxxx - xxxx - xxxx - xxxx - xxxxxxxxxxxx/Library/Caches
            // Strip "/Caches" from path 
            //loginDataFilePath = Application.temporaryCachePath.Substring(0, Application.temporaryCachePath.Length - 6);
            loginDataFilePath = Application.persistentDataPath + "/../Library/" + logindataFileName;
            //iCloud バックアップ不要ファイルに設定する//
            UnityEngine.iOS.Device.SetNoBackupFlag(loginDataFilePath);
#endif
        }

        public void Login(string userName, string password, Action callbackSucces = null, Action callbackFailed = null)
        {
            // ユーザー名とパスワードでログイン
            NCMBUser.LogInAsync(userName, password, (NCMBException e) =>
            {
                if (e != null)
                {
                    Debug.Log("ログインに失敗: " + e.ErrorMessage);

                    if (callbackFailed != null) callbackFailed();
                }
                else
                {
                    Debug.Log("ログインに成功！");

                    if (callbackSucces != null) callbackSucces();
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

        public void DeleteCurrentAccount(Action callbackSucces, Action callbackFailed)
        {
            //ユーザーを削除します//
            NCMBUser.CurrentUser.DeleteAsync((NCMBException e) =>
            {
                if (e == null)
                {
                    if (callbackSucces != null) callbackSucces();
                }
                else
                {
                    if (callbackFailed != null) callbackFailed();
                }
            });

        }
        public void AutoSignin(Action callbackSucces = null, Action callbackFailed = null)
        {
            string generatedUserName = Guid.NewGuid().ToString("N").Substring(0, 8);
            AutoSignin(generatedUserName, callbackSucces, callbackFailed);
        }

        public void AutoSignin(string userName, Action callbackSucces = null, Action callbackFailed = null)
        {
            string generatedPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            Debug.Log("new pass" + generatedPassword);

            //ログイン成功した場合にLoginDataの保存処理を行う//
            callbackSucces += () => SaveLoginData(userName, generatedPassword);

            Signin(userName, generatedPassword, callbackSucces, callbackFailed);
        }

        private void SaveLoginData(string userName, string password)
        {
            NCMBLoginData logindata = new NCMBLoginData();
            logindata.userName = userName;
            logindata.password = password;

            string strdata = JsonUtility.ToJson(logindata);


            FileAESCrypter.EncryptToFile(strdata, loginDataFilePath);
        }

        public void AutoLogin(Action callbackSucces = null, Action callbackFailed = null)
        {
            string strdata;
            FileAESCrypter.DecryptFromFile(out strdata, loginDataFilePath);

            NCMBLoginData logindata = JsonUtility.FromJson<NCMBLoginData>(strdata);

            Login(logindata.userName, logindata.password, callbackSucces, callbackFailed);
        }

        public void Signin(string userName, string password, Action callbackSucces = null, Action callbackFailed = null)
        {
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
                    if (callbackFailed != null) callbackFailed();
                    Debug.Log("新規登録に失敗: " + e.ErrorMessage);
                }
                else
                {
                    if (callbackSucces != null) callbackSucces();
                    Debug.Log("新規登録に成功");
                }
            });
        }

        public void Logout(Action callbackSucces = null, Action callbackFailed = null)
        {
            NCMBUser.LogOutAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    if (callbackFailed != null) callbackFailed();
                    Debug.Log("ログアウトに失敗！");
                }
                else
                {
                    if (callbackSucces != null) callbackSucces();
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