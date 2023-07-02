using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTest : MonoBehaviour
{
    void Start()
    {
        UpdateFileMgr.Instance.DownLoadABCompareFile((isdownLoaded) =>
        {
            if (isdownLoaded)
            {
                Debug.Log("获取服务端AB包信息成功！");

                // 执行下载AB包逻辑
                UpdateFileMgr.Instance.DownLoadABFile((isOver) =>
                {
                    if (isOver)
                        Debug.Log("所有AB包都已经下载完毕！");
                    else
                        Debug.Log("下载失败，网络异常！");
                }, (downLoadedNumber, total, fileName) =>
                {
                    Debug.Log($"当前进度【{downLoadedNumber}/{total},{fileName}】");
                });
            }
            else
                Debug.Log("AB包信息下载失败！");
        });
    }
}
