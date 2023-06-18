using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Windows;

public class EstadisticsController : MonoBehaviour
{ 
    //public double[] timeInStates = { 20, 30, 5 }; // FREE, BUSY, BROKEN
    //public string[] etiquetes = { "FREE", "BUSY", "BROKEN" };
    /*
    private void Start()
    {
    }


    private void Update()
    {
       
    }

    public void GeneraEstadistic(int tipus, double[] estadistics, string[] etiquetes, string estadistic,string nomImatge)
    {
        StartCoroutine(GetTexture(tipus, estadistics, etiquetes, estadistic, nomImatge));
    }

    IEnumerator GetTexture(int tipus, double[] estadistics, string[] etiquetes, string estadistic,string nomImatge)
    {
        
        string type = "";
        string dataetiquetes = "[";
        string data = "[";
        switch (tipus)
        {
            case 0:
                type = "bar";
                break;
            case 1:
                type = "line";
                break;
            case 2:
                type = "pie";
                break;
            default:
                type = "bar";
                break;
        }

        for (int i = 0; i < estadistics.Length; i++)
        {
            if (i == 0)
            {
                dataetiquetes += "'" + etiquetes[i] + "'";
                data += "'" + estadistics[i] + "'";
            }
            else
            {
                dataetiquetes += ", '" + etiquetes[i] + "'";
                data += ", '" + estadistics[i] + "'";
            }
        }

        dataetiquetes += "]";
        data  += "]";

        string URI = "https://quickchart.io/chart?c={type:'"+ type +"',data:{labels:"+dataetiquetes+",datasets:[{label:'"+estadistic+"',data:"+data+"}]}}";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(URI);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            saveTexture(myTexture, nomImatge);
        }

     
    }

    private void saveTexture(Texture2D texture, string nomImatge)
    {
        byte[] bytesTexture = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + nomImatge + ".png", bytesTexture);
    }
    */
}
