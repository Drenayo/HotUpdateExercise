using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;

public class UpLoadAB
{
    [MenuItem("AB包工具/上传AB包到服务器")]
    private static void UpLoadAllABFile()
    {
        // 获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(GlobalData.LOCAL_AB_PATH);
        // 获取该目录下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();

        // 遍历所有文件信息
        foreach (FileInfo info in fileInfos)
        {
            //根据后缀来判断是否为需要上传的 AB包文件或配置信息
            if (info.Extension.Equals(string.Empty) || info.Extension.Equals(".txt"))
            {
                // 这里已经获取了需要上传的文件名字，调用上传函数即可
                FtpUpLoadFile(info.Name, info.FullName);
            }

        }

    }

    /// <summary>
    /// 上传一个文件
    /// </summary>
    /// <param name="fileName">要上传的文件名</param>
    /// <param name="filePath">要上传的完整文件路径</param>
    private async static void FtpUpLoadFile(string fileName, string filePath)
    {
        await Task.Run(() =>
        {
            try
            {
                // 创建一个FTP连接，用于上传
                FtpWebRequest req = FtpWebRequest.Create(new System.Uri("ftp://111.67.192.246/Data/PC/" + fileName)) as FtpWebRequest;
                // 设置一个通信凭证 用于上传
                NetworkCredential n = new NetworkCredential("ftp6578387", "FTP+2002mty");
                req.Credentials = n;
                // 设置代理为null
                req.Proxy = null;
                // 请求完毕后，是否关闭控制连接
                req.KeepAlive = false;
                // 操作命令 上传
                req.Method = WebRequestMethods.Ftp.UploadFile;
                // 指定传输的类型 2进制
                req.UseBinary = true;

                // 上传文件
                // FTP的流对象
                Stream upLoadStream = req.GetRequestStream();
                // 读取文件
                using (FileStream file = File.OpenRead(filePath))
                {
                    // 一次上传2048个字节
                    byte[] bytes = new byte[2048];
                    // 返回值 代表读取了多少个字节
                    int contentLength = file.Read(bytes, 0, bytes.Length);

                    // 循环上传文件中的数据
                    while (contentLength != 0)
                    {
                        // 写入到上传流中
                        upLoadStream.Write(bytes, 0, contentLength);
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }

                    // 循环完毕
                    file.Close();
                    upLoadStream.Close();
                    Debug.Log(fileName + "文件上传成功！");
                }
            }
            catch (Exception ex)
            {
                Debug.Log(fileName + "文件上传失败！" + "_" + ex.Message);
            }
        });
    }
}
