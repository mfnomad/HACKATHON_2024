using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro;

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
        interno.content = "Quiero que escribas 3 prompts para generar imágenes de DALL-E. Las 3 prompts deben tener alguna temática en común, pero resultar en imágenes diferentes. " +
            "Quiero que cada prompt sea de menos de 20 palabras. Después de eso escribe un Haiku referenciando a alguna de las 3 prompts seleccionada aleatoriamente. " +
            "El Haiku funcionará como pista para que un humano elija una de las 3 imágenes, entonces debe estar referenciado con el número de la prompt a la que se refiere. También es necesario que la pista no sea evidente. " +
            "Sigue la siguiente estructura y no escribas nada más:" +
            "1 (Aquí escribes la prompt para la primera imagen.)\n" +
            "2 (Aquí escribes la prompt para la segunda imagen.)\n" +
            "3 (Aquí escribes la prompt para la segunda imagen.)\n" +
            "(Aquí escribes el número de la imagen a la que hace referencia el Haiku) (Aquí escribes el Haiku)";
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

    // Update is called once per frame
    void Update()
    {
        
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
