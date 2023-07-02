using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Events;

public class UpdateFileMgr : MonoBehaviour
{
    // 服务端已经解析的信息列表
    public Dictionary<string, ABInfo> remoteABInfo = new Dictionary<string, ABInfo>();


    private static UpdateFileMgr instance;
    public static UpdateFileMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("UPdateMgr");
                instance = obj.AddComponent<UpdateFileMgr>();
            }
            return instance;
        }
    }

    /// <summary>
    /// 下载并解析服务端AB包信息
    /// </summary>
    public async void DownLoadABCompareFile(UnityAction<bool> overCallBack)
    {
        //Debug.Log("下载的文件存放路径 ： " + Application.persistentDataPath);
        int reDownLoadMaxNum = 5;
        bool isOver = false;
        string fullName = Application.persistentDataPath + "/ABCompareInfo.txt";
        while (reDownLoadMaxNum > 0)
        {
            await Task.Run(() =>
            {
                isOver = DownLoadFile("ABCompareInfo.txt", fullName);
            });

            if (isOver)
                break;
            else
                --reDownLoadMaxNum;
        }

        if (!(reDownLoadMaxNum > 0))
        {
            overCallBack(false);
            return;
        }

        // 获取文件中的字符串数据，进行拆分
        string infos = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo.txt");
        // AB包信息拆分
        string[] strs = infos.Split('|');
        string[] info = null;
        for (int i = 0; i < strs.Length; i++)
        {
            // AB包详细信息再拆分
            info = strs[i].Split(' ');
            // 记录到字典中
            remoteABInfo.Add(info[0], new ABInfo(info[0], info[1], info[2]));
        }
        overCallBack(true);
    }

    /// <summary>
    /// 下载服务端的AB包
    /// </summary>
    public async void DownLoadABFile(UnityAction<bool> overCallBack, UnityAction<int, int, string> updatePro)
    {
        List<string> downLoadList = new List<string>();
        List<string> tempList = new List<string>();
        string localPath = Application.persistentDataPath + "/";
        bool isOver = false; // 当前文件是否下载成功
        int reDownLoadMaxNum = 5; // 最大重试下载次数
        int downLoadOverNum = 0; // 当前已经下载的包数量
        int downLoadMaxMum = 0;// 当前的包总量

        // 遍历字典，将包名存入另一个列表
        foreach (string name in remoteABInfo.Keys)
        {
            // 配置文件无需加入待下载列表(有后缀名的都不加入待下载列表)
            if (name.IndexOf('.') == -1)
                downLoadList.Add(name);
        }
        downLoadMaxMum = downLoadList.Count;

        // 判断待下载列表是否不空 && 重新下载次数是否未满
        while (downLoadList.Count > 0 && reDownLoadMaxNum > 0)
        {
            for (int i = 0; i < downLoadList.Count; i++)
            {
                isOver = false;
                await Task.Run(() =>
                {
                    // 下载文件
                    isOver = DownLoadFile(downLoadList[i], localPath + downLoadList[i]);
                });

                if (isOver)
                {
                    // 委托 把进度数据传给调用外部
                    updatePro(++downLoadOverNum, downLoadMaxMum, downLoadList[i]);
                    // 下载成功的包名，记录下来
                    tempList.Add(downLoadList[i]);
                }
            }
            // 从downLoadList移除成功下载的包名
            for (int i = 0; i < tempList.Count; i++)
                downLoadList.Remove(tempList[i]);

            // 每个文件都有五次重新下载的机会，都不行就异常 报错网络问题
            if (!isOver)
                --reDownLoadMaxNum;
        }

        overCallBack(downLoadList.Count == 0);
    }

    /// <summary>
    /// 从服务器下载文件
    /// </summary>
    /// <param name="fileName">文件名字</param>
    /// <param name="localPath">要下载到本地的存放路径</param>
    public bool DownLoadFile(string fileName, string localPath)
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
            // 操作命令 下载
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            // 指定传输的类型 2进制
            req.UseBinary = true;

            // 下载文件
            // ftp流对象
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();

            using (FileStream file = File.Create(localPath))
            {
                // 每次下载2048字节
                byte[] bytes = new byte[2048];
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                // 循环下载一个文件中的所有字节
                while (contentLength != 0)
                {
                    file.Write(bytes, 0, contentLength);
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }

                file.Close();
                downLoadStream.Close();
                return true;
                //Debug.Log(fileName + "文件下载成功");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(fileName + "文件下载失败" + ex.Message);
            return false;
        }

    }

    private void OnDestroy()
    {
        instance = null;
    }
    public class ABInfo
    {
        public string name;
        public string size;
        public string md5;

        public ABInfo(string name, string size, string md5)
        {
            this.name = name;
            this.size = size;
            this.md5 = md5;
        }
    }
}
