/*
Copyright (c) 2016-2017 Takaaki Ichijo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// stringおよびbyte[]をAES暗号化して外部ファイルへ保存・読み込みする//
/// 参考: http://qiita.com/tempura/items/ad154d1269882ceda0f4
/// </summary>

public class FileAESCrypter
{
    private static readonly string EncryptKey = "m09dfs92hiwhd2lx";

    public static void EncryptToFile(string data, string savePath)
    {
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            string initialVector;
            byte[] bodyBytes;

            EncryptAes(bytes, out initialVector, out bodyBytes);

            byte[] initialVectorBytes = Encoding.UTF8.GetBytes(initialVector);

            if (!File.Exists(savePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }

            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(initialVectorBytes.Length);
                bw.Write(initialVectorBytes);

                bw.Write(bodyBytes.Length);
                bw.Write(bodyBytes);

                bw.Close();
            }
        }
    }

    public static string DecryptFromBytes(byte[] readedBytes)
    {
        byte[] initialVectorBytes = null;
        byte[] bodyBytes = null;

        Stream stream = new MemoryStream(readedBytes);
        using (BinaryReader br = new BinaryReader(stream))
        {
            int length = br.ReadInt32();
            initialVectorBytes = br.ReadBytes(length);

            length = br.ReadInt32();
            bodyBytes = br.ReadBytes(length);
        }

        byte[] bytes;
        string initialVector = Encoding.UTF8.GetString(initialVectorBytes);
        DecryptAes(bodyBytes, initialVector, out bytes);

        return Encoding.UTF8.GetString(bytes);
    }

    public static bool DecryptFromFile(out string data, string savePath)
    {
        byte[] initialVectorBytes = null;
        byte[] bodyBytes = null;

        if (!File.Exists(savePath))
        {
            data = string.Empty;
            return false;
        }

        using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader br = new BinaryReader(fs))
        {
            int length = br.ReadInt32();
            initialVectorBytes = br.ReadBytes(length);

            length = br.ReadInt32();
            bodyBytes = br.ReadBytes(length);
        }

        byte[] bytes;
        string initialVector = Encoding.UTF8.GetString(initialVectorBytes);
        DecryptAes(bodyBytes, initialVector, out bytes);

        data = Encoding.UTF8.GetString(bytes);
        return true;
    }

    public static void EncryptAes(byte[] src, out string initialVector, out byte[] dst)
    {
        initialVector = Guid.NewGuid().ToString("N").Substring(0, EncryptKey.Length);
        dst = null;
        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;

            byte[] key = Encoding.UTF8.GetBytes(EncryptKey);
            byte[] vec = Encoding.UTF8.GetBytes(initialVector);

            using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, vec))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(src, 0, src.Length);
                cs.FlushFinalBlock();
                dst = ms.ToArray();
            }
        }
    }

    public static void DecryptAes(byte[] src, string initialVector, out byte[] dst)
    {
        dst = new byte[src.Length];
        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;

            byte[] key = Encoding.UTF8.GetBytes(EncryptKey);
            byte[] vec = Encoding.UTF8.GetBytes(initialVector);

            using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, vec))
            using (MemoryStream ms = new MemoryStream(src))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                cs.Read(dst, 0, dst.Length);
            }
        }
    }
}