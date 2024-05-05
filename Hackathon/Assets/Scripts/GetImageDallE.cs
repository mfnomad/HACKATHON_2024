using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GetImageDallE : MonoBehaviour
{
    private string apiKey = "";
    private string apiUrl = "https://api.openai.com/v1/images/generations";

    void Start()
    {
        StartCoroutine(GenerateImageFromDallE("A futuristic city skyline at sunset"));
    }

    IEnumerator GenerateImageFromDallE(string prompt)
    {
        var requestBody = new
        {
            model = "dall-e-2",
            prompt = prompt,
            n = 1,
            size = "256x256"
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
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
            }
            else
            {
                var jsonResponse = JsonUtility.FromJson<AIAPIResponse>(webRequest.downloadHandler.text);
                string imageUrl = jsonResponse.url;
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
            GameObject imageObject = new GameObject("Dall-E Image");
            imageObject.transform.position = new Vector3(0, 0, 0);
            SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(downloadedImage, new Rect(0, 0, downloadedImage.width, downloadedImage.height), new Vector2(0.5f, 0.5f));
        }
    }

    private class AIAPIResponse
    {
        public string url;
    }
}