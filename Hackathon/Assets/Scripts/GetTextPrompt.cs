using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GetTextPrompt : MonoBehaviour
{
    public TMP_Text hint;

    public void get()
    {
        // Set hint to inactive
        hint.gameObject.SetActive(false);
        string url = "https://www.airbnb.com/health";
        StartCoroutine(GetGameLevelCoroutine(url));   
    }

    IEnumerator GetGameLevelCoroutine(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        string response = request.downloadHandler.text;
        //AIAPIResponse response = JsonUtility.FromJson<AIAPIResponse>(jsonResponseBody);

        if (response != null)
        {
            
            //string[] clues = response.text.Split('\n');
            // Aquí tenemos que parsear el texto para obtener cada una de las 3 pistas

            //Vector3 randomPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
            hint.text = "Esta es la pista: " + response;
            Debug.Log(response);

        }
        else
        {
            Debug.LogWarning("Invalid response received.");
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

    private class AIAPIResponse
    {
        public string text;
    }
}
