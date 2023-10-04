using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewUpdateTest : MonoBehaviour
{
    public string abName;
    public string resName;
    public string resName2;
    public Transform modelTran;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private AssetBundle ab;

    public void Btn_UpdateRes()
    {
        if(!ab)
        ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abName);
        foreach (Transform item in modelTran)
        {
            Destroy(item.gameObject);
        }
        try
        {
            Instantiate(ab.LoadAsset(resName, typeof(GameObject)) as GameObject, modelTran);
        }
        catch (System.Exception)
        {
            Instantiate(ab.LoadAsset(resName2, typeof(GameObject)) as GameObject, modelTran);
            throw;
        }
        
    }
}
