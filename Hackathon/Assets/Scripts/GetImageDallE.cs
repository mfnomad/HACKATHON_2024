using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GetImageDallE : MonoBehaviour
{
    private string apiKey = "";
    private string apiUrl = "https://api.openai.com/v1/images/generations";

    void Start()
    {
        StartCoroutine(GenerateImageFromDallE("Pixelart de un desierto"));
        //StartCoroutine(DownloadImage("https://image.tmdb.org/t/p/w500/9n2tJBplPbgR2ca05hS5CKXwP2c.jpg"));
    }

    IEnumerator GenerateImageFromDallE(string prompt)
    {
        DalleJSON dalleJSON = new DalleJSON();
        dalleJSON.model = "dall-e-2";
        dalleJSON.prompt = prompt;
        dalleJSON.size = "1024x1024";
        dalleJSON.n = 1;

        string jsonBody = JsonUtility.ToJson(dalleJSON);
        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                Debug.LogError("Response: " + webRequest.downloadHandler.text);
            }
            else
            {
                var jsonResponse = JsonUtility.FromJson<AIAPIResponse>(webRequest.downloadHandler.text);
                if (jsonResponse.data != null && jsonResponse.data.Length > 0)
                {
                    string imageUrl = jsonResponse.data[0].url; 
                    Debug.Log(imageUrl);
                    StartCoroutine(DownloadImage(imageUrl));
                }
                else
                {
                    Debug.LogError("No data available.");
                }
            }
        }
    }

    IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(imageRequest.error);
        }
        else
        {
            Texture2D downloadedImage = DownloadHandlerTexture.GetContent(imageRequest);
            Debug.Log("Image downloaded and applied!");

            // Aplicar la textura como un Sprite
            GameObject square = GameObject.Find("square");
            if (square != null)
            {
                SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = Sprite.Create(downloadedImage, new Rect(0, 0, downloadedImage.width, downloadedImage.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError("No SpriteRenderer component found on 'square' GameObject.");
                }
            }
            else
            {
                Debug.LogError("GameObject 'square' not found.");
            }
        }
    }

    [Serializable]
    public class AIAPIResponse
    {
        public int created;
        public Data[] data;
    }

    [Serializable]
    public class Data
    {
        public string url;
    }

    [Serializable]
    public class DalleJSON
    {
        public string model;
        public string prompt;
        public string size;
        public int n;
    }
}
