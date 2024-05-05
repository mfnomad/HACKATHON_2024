using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro;
using System.Text.RegularExpressions;

public class GetTextPrompt : MonoBehaviour
{
    public TMP_Text hint;
    private string apiKey = "";
    private string url = "https://api.openai.com/v1/chat/completions";

    public void get()
    {
        // Set hint to inactive
        hint.gameObject.SetActive(false);

        StartCoroutine(GetGameLevelCoroutine(url));   
    }

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

                // Deserializa la respuesta
                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(responseText);

                // Verifica si hay choices y messages disponibles
                if (response.choices != null && response.choices.Length > 0 && response.choices[0].message != null)
                {
                    string content = response.choices[0].message.content;
                    Debug.Log("Content: " + content);
                    parseString(content);
                    hint.text = content;  // Asigna el contenido extraído al texto de la UI
                }
                else
                {
                    Debug.LogWarning("Invalid response received.");
                }
            }
        }        
    }  
    
    // Start is called before the first frame update
    void Start()
    {
        get();
    }

    void parseString(string text)
    {
        string ExtractText(string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.Singleline);  
            Match match = regex.Match(text);
            return match.Success ? match.Groups[1].Value : "No Match Found";
        }

        string promptUno = ExtractText(@"promptuno (.*) promptuno");
        string promptDos = ExtractText(@"promptdos (.*) promptdos");
        string promptTres = ExtractText(@"prompttres (.*) prompttres");
        int numCorrecta = int.Parse(ExtractText(@"numcorrecta (\d+) numcorrecta"));  
        string pistaHaiku = ExtractText(@"pistahaiku (.*?) pistahaiku");

        Debug.Log($"Prompt Uno: {promptUno}");
        Debug.Log($"Prompt Dos: {promptDos}");
        Debug.Log($"Prompt Tres: {promptTres}");
        Debug.Log($"Número Correcta: {numCorrecta}");
        Debug.Log($"Pista Haiku: {pistaHaiku}");
    }

    [Serializable]
    private class AIAPIResponse
    {
        public string text;
    }

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
}
