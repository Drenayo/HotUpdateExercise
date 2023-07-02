using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class MD5Utility
{

    public static string GetMD5(string filePath)
    {
        // 文件以流的形式打开
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            // 声明一个MD5对象，用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            // 得到文件的MD5码，16个字节的数组
            byte[] md5Info = md5.ComputeHash(file);
            // 关闭文件流
            file.Close();


            // 把字节数组转为十六进制字符串
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
