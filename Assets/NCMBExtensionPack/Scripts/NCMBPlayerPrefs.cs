using NCMB;
using System;
using UnityEngine;

namespace NCMBExtension
{
    /// <summary>
    /// サーバー側が常に新しい設定//
    /// キーが存在する場合は値を取得する//
    ///存在しない場合はローカルPlayerPrefsの値を返す//
    ///ログインした時点でNCMBUserの中身が取得できている状態//
    ///任意のタイミングでサーバーとローカル間のデータ同期も可能//
    /// </summary>
    ///

    public class NCMBPlayerPrefs
    {
        public static int GetInt(string keyName, int defalutValue = 0, Action<string> errorMessageCallback = null)
        {
            if (IsServerDataEnabled(keyName, errorMessageCallback))
            {
                Debug.Log("Return server value");
                //サーバーに格納される際Int64なのでキャストして取得//
                return (int)Convert.ToInt64(NCMBUser.CurrentUser[keyName]);
            }

            Debug.Log("Return local value");
            return PlayerPrefs.GetInt(keyName, defalutValue);
        }

        public static void SetInt(string keyName, int value)
        {
            if (NCMBUser.CurrentUser != null)
            {
                //サーバーに格納される際Int64なので先にキャストしておく//
                NCMBUser.CurrentUser[keyName] = Convert.ToInt64(value);
            }
            PlayerPrefs.SetInt(keyName, value);
        }

        //指定キーのIntをローカルからサーバーにコピーする//
        public static void CopyIntLocalToServer(string keyName)
        {
            int value = 0;

            if (PlayerPrefs.HasKey(keyName))
            {
                value = PlayerPrefs.GetInt(keyName);
            }

            //サーバーに格納される際Int64なので先にキャストしておく//
            NCMBUser.CurrentUser[keyName] = Convert.ToInt64(value);
        }

        //指定キーのIntをサーバーからローカルにコピーする//
        public static void CopyIntServerToLocal(string keyName)
        {
            int value = 0;

            if (NCMBUser.CurrentUser != null)
            {
                //サーバーに格納される際Int64なのでキャストして取得//
                value = (int)Convert.ToInt64(NCMBUser.CurrentUser[keyName]);
            }

            PlayerPrefs.SetInt(keyName, value);
        }

        public static string GetString(string keyName, string defalutValue = "", Action<string> errorMessageCallback = null)
        {
            if (IsServerDataEnabled(keyName, errorMessageCallback))
            {
                Debug.Log("Return server value");
                return NCMBUser.CurrentUser[keyName].ToString();
            }

            Debug.Log("Return local value");
            return PlayerPrefs.GetString(keyName, defalutValue);
        }

        public static void SetString(string keyName, string value)
        {
            if (NCMBUser.CurrentUser != null)
            {
                NCMBUser.CurrentUser[keyName] = value;
            }
            PlayerPrefs.SetString(keyName, value);
        }

        //指定キーのStringをローカルからサーバーにコピーする//
        public static void CopyStringLocalToServer(string keyName)
        {
            string value = string.Empty;

            if (PlayerPrefs.HasKey(keyName))
            {
                value = PlayerPrefs.GetString(keyName);
            }

            NCMBUser.CurrentUser[keyName] = value;
        }

        //指定キーのFloatをサーバーからローカルにコピーする//
        public static void CopyStringServerToLocal(string keyName)
        {
            string value = string.Empty;

            if (NCMBUser.CurrentUser != null)
            {
                value = NCMBUser.CurrentUser[keyName].ToString();
            }

            PlayerPrefs.SetString(keyName, value);
        }

        public static float GetFloat(string keyName, float defalutValue = 0f, Action<string> errorMessageCallback = null)
        {
            if (IsServerDataEnabled(keyName, errorMessageCallback))
            {
                Debug.Log("Return server value");
                return (float)NCMBUser.CurrentUser[keyName];
            }

            Debug.Log("Return local value");
            return PlayerPrefs.GetFloat(keyName, defalutValue);
        }

        public static void SetFloat(string keyName, float value)
        {
            if (NCMBUser.CurrentUser != null)
            {
                NCMBUser.CurrentUser[keyName] = value;
            }
            PlayerPrefs.SetFloat(keyName, value);
        }

        //指定キーのFloatをローカルからサーバーにコピーする//
        public static void CopyFloatLocalToServer(string keyName)
        {
            float value = 0f;

            if (PlayerPrefs.HasKey(keyName))
            {
                value = PlayerPrefs.GetFloat(keyName);
            }

            NCMBUser.CurrentUser[keyName] = value;
        }

        //指定キーのFloatをサーバーからローカルにコピーする//
        public static void CopyFloatServerToLocal(string keyName)
        {
            float value = 0f;

            if (NCMBUser.CurrentUser != null)
            {
                value = (float)NCMBUser.CurrentUser[keyName];
            }

            PlayerPrefs.SetFloat(keyName, value);
        }

        public static void Save(Action saveSuccessCallback = null, Action saveFailCallback = null, Action<string> errorMessageCallback = null)
        {
            //まずローカルセーブ//
            SaveLocalTimeStamp();
            PlayerPrefs.Save();

            //前回のセッショントークンを取得//
            string previousToken = NCMBUser._getCurrentSessionToken();

            //セーブ//
            NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
            {
                if (e == null)
                {
                    if (saveSuccessCallback != null) saveSuccessCallback();
                }
                else
                {
                    if (saveFailCallback != null) saveFailCallback();

                    //通信後のセッショントークンを取得//
                    string currentToken = NCMBUser._getCurrentSessionToken();

                    if (previousToken != currentToken)
                    {
                        if (errorMessageCallback != null)
                        {
                            //途中で別の端末からログインされている可能性//
                            errorMessageCallback("Token Doesn't mach");
                        }
                    }
                }
            });
        }

        private static bool IsServerDataEnabled(string keyName, Action<string> errorMessageCallback = null)
        {
            //ログインしてる？//
            if (NCMBUser.CurrentUser == null)
            {
                if (errorMessageCallback != null) errorMessageCallback("Not Logged in");
                return false;
            }
            else if (!NCMBUser.CurrentUser.UpdateDate.HasValue)
            {
                //オンラインに記録した形跡は？//
                Debug.Log("Can't find server save time.");
                return false;
            }
            else if (!NCMBUser.CurrentUser.ContainsKey(keyName))
            {
                //キーはある？//
                if (errorMessageCallback != null) errorMessageCallback("Not cotains key" + keyName);
                return false;
            }
            else
            {
                //サーバーとローカル、どっちのデータが新しい？//
                DateTime serverSavedTime = NCMBUser.CurrentUser.UpdateDate.Value;

                serverSavedTime.UtcToLocal();

                DateTime localSavedTime = GetLocalSaveTimeStamp();
                return serverSavedTime > localSavedTime;
            }
        }

        public static bool HasKey(string key)
        {
            if (NCMBUser.CurrentUser.ContainsKey(key))
            {
                return true;
            }
            else if (PlayerPrefs.HasKey(key))
            {
                Debug.Log(key + "exsit on only local data");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DeleteServerKey(string key)
        {
            NCMBUser.CurrentUser.Remove(key);
            NCMBUser.CurrentUser.Save();
        }

        public static void DeleteLocalKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        private static void SaveLocalTimeStamp()
        {
            PlayerPrefs.SetString("SaveDateTime", DateTime.Now.ToBinary().ToString());
        }

        private static DateTime GetLocalSaveTimeStamp()
        {
            string datetimeString = PlayerPrefs.GetString("SaveDateTime");

            if (string.IsNullOrEmpty(datetimeString))
            {
                return new DateTime(0);
            }

            return DateTime.FromBinary(Convert.ToInt64(datetimeString));
        }
    }
}