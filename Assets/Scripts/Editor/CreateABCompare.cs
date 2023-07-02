using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateABCompare
{

    [MenuItem("AB包工具/创建AB包资源对比文件")]
    public static void CreateABCompareFile()
    {
        // 获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(GlobalData.LOCAL_AB_PATH);
        // 获取该目录下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();

        // 用于存储信息的 字符串
        string abCompareInfo = string.Empty;

        // 遍历所有文件信息
        foreach (FileInfo info in fileInfos)
        {
            //根据后缀来判断是否为AB包文件或配置信息
            if (info.Extension.Equals(string.Empty) || info.Extension.Equals(".txt"))
            {
                // 拼接AB包的信息
                abCompareInfo += info.Name + " " + info.Length + " " + MD5Utility.GetMD5(info.FullName) + '|';
            }

        }
        // 移除最后一个 | 分割符号，为了以后判定方便
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);

        // 创建一个文本文件，存入已经生成的内容
        File.WriteAllText(GlobalData.LOCAL_AB_PATH + "ABCompareInfo.txt", abCompareInfo);
        Debug.Log("创建AB包资源对比文件成功！");

        // 刷新Unity资源管理器，因为这里是使用C#创建的，Unity无法自动刷新显示出来，所以为了避免手动刷新，这里设置一下
        AssetDatabase.Refresh();
    }
}
