using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Assets.Scripts;

public class VoiceController : MonoBehaviour {
    
    const string LANG_CODE = "en-US";

    [SerializeField]
    MyVideoControlsManager manager;
    [SerializeField]
    TextToSpeech tts;
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    Canvas videoControls;
    [SerializeField]
    Text text;
    [SerializeField]
    Button speech;


    bool isPlaying = true;
    string uiText = "";

    void Start() {
        Setup(LANG_CODE);

#if UNITY_ANDROID
        SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResult;
#endif

        SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.instance.onStartCallBack = OnSpeakStop;
        TextToSpeech.instance.onDoneCallback = OnSpeakStop;

        CheckPermission();
    }

    void CheckPermission() {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }
    #region Text To Speech
    
    public void StartSpeaking(string message) {
        TextToSpeech.instance.StartSpeak(message);
    }

    public void StopSpeaking() {
        TextToSpeech.instance.StopSpeak();
    }

    public void OnSpeakStart() {
        Debug.Log("Talking started ...");
    }

    public void OnSpeakStop() {
        Debug.Log("Talking stopped ...");
    }

    #endregion

    #region Speech To Text
    
    public void StartListening() {
        SpeechToText.instance.StartRecording();
    }

    public void StopListening() {
        SpeechToText.instance.StopRecording();
    }

    /*
    void OnFinalSpeechResult(string result) {
        uiText.text = result;
    }
    */

    public string[] getNumerals() {
        string[] nums = new string[11];
        nums[0] = "one";
        nums[1] = "two";
        nums[2] = "three";
        nums[3] = "four";
        nums[4] = "five";
        nums[5] = "six";
        nums[6] = "seven";
        nums[7] = "eight";
        nums[8] = "nine";
        nums[9] = "ten";
        nums[10] = "eleven";

        return nums;
    }

    void OnFinalSpeechResult(string result)
    {
        bool isControlActive = videoControls.gameObject.active;

        uiText = result;
        result = result.ToLower();

        if (result == "play")
        {
            if (isPlaying == false)
            {
                manager.OnPlayPause();
                uiText = "playing video";
                isPlaying = !isPlaying;
            }
            else
            {
                uiText = "currently playing";
            }
        }
        if (result == "stop")
        {
            if (isPlaying == true)
            {
                manager.OnPlayPause();
                uiText = "video paused";
                isPlaying = !isPlaying;
            }
            else
            {
                uiText = "video already paused";
            }
        }
        else if (result == "volume up")
        {
            manager.OnVolumeUp();
            uiText = "volume raised";
        }
        else if (result == "volume down")
        {
            manager.OnVolumeDown();
            uiText = "volume reduced";
        }
        else if (result == "slow down")
        {
            manager.OnSlowDown();
            uiText = "playback speed decreased";
        }
        else if (result == "speed up")
        {
            manager.OnSpeedUp();
            uiText = "playback speed increased";
        }
        else if (result.StartsWith("track") || result.StartsWith("truck") || result.StartsWith("drug"))
        {
            string suffix = result.Remove(0, 5 + 1);
            string[] nums = getNumerals();
            bool isTrackSelected = false;
            try
            {
                int trackNum = int.Parse(suffix);
                manager.SelectTrack(trackNum);
                uiText = "playing track " + suffix;
                isTrackSelected = true;
            }
            catch
            {
                isTrackSelected = false;
                Debug.Log("String not in numeric format");
            }
            if (!isTrackSelected)
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    if (suffix.Equals(nums[i]))
                    {
                        manager.SelectTrack(i);
                        uiText = "playing track " + suffix;
                        isTrackSelected = true;
                    }
                }
            }
            if (!isTrackSelected)
            {
                uiText = "No valid command found, please try again";
            }
        }
        else if (result == "show interface")
        {
            if (!isControlActive)
            {
                mainCamera.transform.localScale = new Vector3(1, 1, 1);
                videoControls.gameObject.SetActive(true);
                speech.gameObject.SetActive(false);
                uiText = "Displaying user interface";
            }
            else
            {
                uiText = "Interface already visible";
            }
        }
        else if (result == "hide interface" || result == "heidi interface")
        {
            if (isControlActive)
            {
                mainCamera.transform.localScale = new Vector3(0.000001f, 0.000001f, 0.000001f);
                videoControls.gameObject.SetActive(false);
                speech.gameObject.SetActive(true);
                uiText = "Hiding user interface";
            }
            else 
            {
                uiText = "Interface already hidden";
            }
        }
        else
        {
            //uiText = result;
            //text.text = result;
            uiText = "No valid command found, please try again";
        }

        tts.StartSpeak(uiText);
    }

    void OnPartialSpeechResult(string result) {
            
    }

    #endregion

    void Setup(string code) {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }
}
