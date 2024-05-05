using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;

public class IAController : MonoBehaviour
{
    private string apiKey = "";
    private string imageUrl = "https://api.openai.com/v1/images/generations";
    private string textUrl = "https://api.openai.com/v1/chat/completions";

    private string promptUno;
    private string promptDos;
    private string promptTres;
    private int numCorrecta;
    private string pistaHaiku;

    private TMP_Text hint;
    private RawImage image1;
    private RawImage image2;
    private RawImage image3;

    public int getNumCorrecta()
    {
        return numCorrecta;
    }

    public void getText(TMP_Text hint, RawImage image1, RawImage image2, RawImage image3)
    {
        Debug.Log("getText");
        this.hint = hint;
        this.image1 = image1;
        this.image2 = image2;
        this.image3 = image3;
        StartCoroutine(GetGameLevelCoroutine(textUrl));
    }

    // Function to get the prompts and the clue
    IEnumerator GetGameLevelCoroutine(string url)
    {
        PromptExterno promptExterno = new PromptExterno();
        PromptInterno interno = new PromptInterno();
        interno.content = "I want you to write 3 very random and creative prompts for generating DALL-E images in English. The 3 prompts should have something in common but result in different images. " +
            "I want each prompt to be less than 20 words. After that, write a Haiku referencing one of the 3 randomly selected prompts. " +
            "The Haiku will serve as a clue for a human to choose one of the 3 images, so it should reference the number of the prompt it pertains to. The clue should also not be obvious. " +
            "Follow the structure below and write nothing else:\n" +
            "promptuno (Here you write the prompt for the first image.) promptuno\n" +
            "promptdos (Here you write the prompt for the second image.) promptdos\n" +
            "prompttres (Here you write the prompt for the third image.) prompttres\n" +
            "numcorrecta (Here you put the number of the image the Haiku refers to) numcorrecta pistahaiku (Here you write the Haiku) pistahaiku";
        interno.role = "user";
        promptExterno.messages = new PromptInterno[] { interno };
        promptExterno.model = "gpt-4-turbo-2024-04-09";

        string jsonBody = JsonUtility.ToJson(promptExterno);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
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
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(responseText);

                if (response.choices != null && response.choices.Length > 0 && response.choices[0].message != null)
                {
                    string content = response.choices[0].message.content;
                    Debug.Log("Content: " + content);
                    parseString(content);
                }
                else
                {
                    Debug.LogWarning("Invalid response received.");
                }
            }
        }
    }

    // Parse prompt to get individual prompts and clue
    void parseString(string text)
    {
        string ExtractText(string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            Match match = regex.Match(text);
            return match.Success ? match.Groups[1].Value : "No Match Found";
        }

        promptUno = ExtractText(@"promptuno (.*) promptuno");
        StartCoroutine(GenerateImageFromDallE(promptUno, image1));
        promptDos = ExtractText(@"promptdos (.*) promptdos");
        StartCoroutine(GenerateImageFromDallE(promptDos, image2));
        promptTres = ExtractText(@"prompttres (.*) prompttres");
        StartCoroutine(GenerateImageFromDallE(promptTres, image3));
        numCorrecta = int.Parse(ExtractText(@"numcorrecta (\d+) numcorrecta"));
        pistaHaiku = ExtractText(@"pistahaiku (.*?) pistahaiku");
        hint.text = pistaHaiku;
    }

    // Get an image
    IEnumerator GenerateImageFromDallE(string prompt, RawImage ri)
    {
        DalleJSON dalleJSON = new DalleJSON();
        dalleJSON.model = "dall-e-2";
        dalleJSON.prompt = prompt;
        dalleJSON.size = "1024x1024";
        dalleJSON.n = 1;

        string jsonBody = JsonUtility.ToJson(dalleJSON);
        using (UnityWebRequest webRequest = new UnityWebRequest(imageUrl, "POST"))
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
                    StartCoroutine(DownloadImage(imageUrl, ri));
                }
                else
                {
                    Debug.LogError("No data available.");
                }
            }
        }
    }

    // Download the image
    IEnumerator DownloadImage(string imageUrl, RawImage ri)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(imageRequest.error);
        }
        else
        {
            Texture t = DownloadHandlerTexture.GetContent(imageRequest);
            ri.texture = t;
        }
    }

    // Classes for text request
    [Serializable]
    public class PromptExterno
    {
        public string model;
        public PromptInterno[] messages;
    }

    [Serializable]
    public class PromptInterno
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class OpenAIResponse
    {
        public string id;
        public string objectType;
        public int created;
        public string model;
        public Choice[] choices;
    }

    [Serializable]
    public class Choice
    {
        public int index;
        public Message message;
        public object logProbs;
        public string finish_reason;
    }

    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    // Classes for image request
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
