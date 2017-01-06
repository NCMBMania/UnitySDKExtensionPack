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
        public static int GetInt(string keyName, int defalutValue = 0, ConnectionEventHandler failueEventHandler = null)
        {
            if (IsServerDataEnabled(keyName, failueEventHandler))
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

        public static string GetString(string keyName, string defalutValue = "", ConnectionEventHandler failueEventHandler = null)
        {
            if (IsServerDataEnabled(keyName, failueEventHandler))
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

        public static float GetFloat(string keyName, float defalutValue = 0f, ConnectionEventHandler failueEventHandler = null)
        {
            if (IsServerDataEnabled(keyName, failueEventHandler))
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

        public static void Save(EventHandler successEventHandler, ConnectionEventHandler failureEventHandler = null)
        {
            //まずローカルセーブ//
            SaveLocalTimeStamp();
            PlayerPrefs.Save();

            //ネット接続があるか？//
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (failureEventHandler != null) failureEventHandler(new ConnectionEventArgs("ネットワークに接続していません。"));

                return;
            }

            //セーブ//
            NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
                {
                    if (e != null)
                    {
                        if (failureEventHandler != null)
                        {
                            string errorMessage = string.Empty;

                            if (e.ErrorCode == NCMBException.INCORRECT_HEADER)
                            {
                                errorMessage = "別の端末からログインされています。";
                            }
                            else if (string.IsNullOrEmpty(e.ErrorCode))
                            {
                                errorMessage = "通信が不安定です。";
                            }
                            else
                            {
                                Debug.Log("error: " + e.ErrorCode + " " + e.ErrorMessage);
                            }

                            failureEventHandler(new ConnectionEventArgs(errorMessage));
                        }
                    }
                    else
                    {
                        if (successEventHandler != null) successEventHandler(null, EventArgs.Empty);
                    }
                }
            );
        }

        private static bool IsServerDataEnabled(string keyName, ConnectionEventHandler failueEventHandler = null)
        {

            //ログインしてる？//
            if (NCMBUser.CurrentUser == null)
            {
                if (failueEventHandler != null) failueEventHandler(new ConnectionEventArgs("ログインしていません。"));
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
                if (failueEventHandler != null) failueEventHandler(new ConnectionEventArgs("キー" + keyName + "は存在しません。"));
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

    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs(string errorMessage = "")
        {
            this.errorMessage = errorMessage;
        }

        public readonly string errorMessage;
    }

    public delegate void ConnectionEventHandler(ConnectionEventArgs eventArgs);
}