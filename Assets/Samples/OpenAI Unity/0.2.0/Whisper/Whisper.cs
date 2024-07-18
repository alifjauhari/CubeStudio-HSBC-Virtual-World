using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        public static Whisper Instance;

        [SerializeField] private Button recordButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private TMP_Dropdown dropdown;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 5;
        private bool isForceStop;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();

        [SerializeField] private PostRequest postRequest;

        public UnityEvent startRecording;
        public UnityEvent stopRecording;
        public UnityEvent recorded;
        public UnityEvent answer;
        public UnityEvent error;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            isForceStop = false;
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            stopButton.onClick.AddListener(EndForceRecording);

            recordButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);

            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private void StartRecording()
        {
            startRecording?.Invoke();
            message.text = "...";

            recordButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            isRecording = true;
            //recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            
            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            #endif
        }

        private async void EndRecording()
        {
            message.text = "加载中...";
            //message.text = "Transcripting...";
            stopRecording?.Invoke();

            #if !UNITY_WEBGL
            Microphone.End(null);
            #endif
            
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "zh" //chinese, english ganti en
            };
            var res = await openai.CreateAudioTranscription(req);

            progressBar.fillAmount = 0;
            message.text = res.Text;
            //recordButton.enabled = true;
            recorded?.Invoke();

            AskAI();
            recordButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
        }

        public void AskAI()
        {
            postRequest.PostRequestVoice(message.text);
        }

        private void EndForceRecording()
        {
            isForceStop = true;
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;
                
                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    isForceStop = false;
                    EndRecording();
                    return;
                }

                if (isForceStop)
                {
                    isRecording = false;
                    time = 0;
                    isForceStop = false;
                    EndRecording();
                }
            }
        }
    }
}
