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
        StartCoroutine(GenerateImageFromDallE("a sad cat"));
    }

    IEnumerator GenerateImageFromDallE(string prompt)
    {
        DalleJSON dalleJSON = new DalleJSON();
        dalleJSON.model = "dall-e-2";
        dalleJSON.prompt = prompt;
        dalleJSON.size = "256x256";
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
                string imageUrl = jsonResponse.url;
                Debug.Log(imageUrl);
                StartCoroutine(DownloadImage(imageUrl));
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
    private class AIAPIResponse
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
