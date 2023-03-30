using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Windows;

public class CridesAPIEstadistics : MonoBehaviour
{ 
    //public double[] timeInStates = { 20, 30, 5 }; // FREE, BUSY, BROKEN
    //public string[] labels = { "FREE", "BUSY", "BROKEN" };

    private void Start()
    {
        //generatePlots(0, timeInStates, labels, "Test");
    }


    private void Update()
    {
       
    }

    public void generatePlots(int graphType, double[] stats, string[] labels, string imageTag)
    {
        StartCoroutine(GetTexture(graphType, stats, labels, imageTag));
    }

    IEnumerator GetTexture(int graphType, double[] stats, string[] labels, string imageTag)
    {
        //Prepare data for the API CALL:
        string type = "";
        string dataLabels = "[";
        string data = "[";
        switch (graphType)
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

        for (int i = 0; i < stats.Length; i++)
        {
            if (i == 0)
            {
                dataLabels += "'" + labels[i] + "'";
                data += "'" + stats[i] + "'";
            }
            else
            {
                dataLabels += ", '" + labels[i] + "'";
                data += ", '" + stats[i] + "'";
            }
        }

        dataLabels += "]";
        data  += "]";

        string URI = "https://quickchart.io/chart?c={type:'"+ type +"',data:{labels:"+dataLabels+",datasets:[{label:'State',data:"+data+"}]}}";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(URI);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            saveTexture(myTexture, imageTag);
        }

     
    }

    private void saveTexture(Texture2D texture, string imageTag)
    {
        byte[] bytesTexture = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + imageTag + ".png", bytesTexture);
    }

}
