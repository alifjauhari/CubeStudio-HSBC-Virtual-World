using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Samples.Whisper;

public class PostRequest : MonoBehaviour
{
    [SerializeField] private string url = "https://hsbe-hk.isysedge.com/api/generate.response";
    public Text responseText; // UI Text untuk menampilkan response
    public TMP_InputField inputText; // UI Text untuk menampilkan response


    // Fungsi untuk mengirim POST request
    public void SendPostRequest()
    {
        StartCoroutine(PostRequestCoroutine(inputText.text));
    }

    public void PostRequestVoice(string voice)
    {
        StartCoroutine(PostRequestCoroutine(voice));
    }

    // Coroutine untuk menangani POST request
    public IEnumerator PostRequestCoroutine(string userQuestion)
    {
        PostData postData = new PostData
        {
            company = "HSBC Virtual Branch",
            database = "DBB6E1F4AC423E1A35A4EB8C5DAFDF43AB",
            role = "Virtual Customer Service Representative",
            question = userQuestion,
            history = new List<string>(),
            version = "v10"
        };

        // Mengubah dictionary ke format JSON
        string jsonPayload = JsonConvert.SerializeObject(postData);
        Debug.Log(jsonPayload);

        // Membuat UnityWebRequest untuk POST request
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Mengirim request dan menunggu response
        yield return request.SendWebRequest();

        // Mengecek apakah ada error
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            responseText.text = "Error: " + request.error; // Menampilkan error di UI
            Whisper.Instance.error?.Invoke();
        }
        else
        {
            // Menampilkan response dari server
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(request.downloadHandler.text);

            Debug.Log("Response: " + request.downloadHandler.text);
            responseText.text = myDeserializedClass.response; // Menampilkan response di UI
            Whisper.Instance.answer?.Invoke();
        }
    }

    [System.Serializable]
    public class PostData
    {
        public string company;
        public string database;
        public string role;
        public string question;
        public List<string> history;
        public string version;
    }

    [System.Serializable]
    public class Additional
    {
        public List<object> named_entities;
        public List<object> related_entities;
        public string rephrased_query;
        public bool faq_found;
        public object faq;
        public object faq_id;
        public double cost;
        public double time_elapsed;
    }

    [System.Serializable]
    public class Root
    {
        public string request_id;
        public string version;
        public string response;
        public string language;
        public string category;
        public Additional additional;
    }
}
