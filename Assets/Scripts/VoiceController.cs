using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Assets.Scripts;

public class VoiceController : MonoBehaviour {

    // const string LANG_CODE = "en-US";
    string LANG_CODE = "it-IT";

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
        if (LANG_CODE == "en-US")
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
        if (LANG_CODE == "it-IT")
            nums[0] = "uno";
            nums[1] = "due";
            nums[2] = "tre";
            nums[3] = "quattro";
            nums[4] = "cinque";
            nums[5] = "sei";
            nums[6] = "sette";
            nums[7] = "otto";
            nums[8] = "nove";
            nums[9] = "dieci";
            nums[10] = "undici";
        return nums;
    }

    void OnFinalSpeechResult(string result)
    {
        bool isControlActive = videoControls.gameObject.active;

        uiText = result;
        result = result.ToLower();

        if ((result == "play" && LANG_CODE == "en-US") || (result == "avvia" && LANG_CODE == "it-IT"))
        {
            if (isPlaying == false)
            {
                manager.OnPlayPause();

                if (LANG_CODE == "en-US")
                    uiText = "playing video";
                else
                    uiText = "video avviato";

                isPlaying = true;
            }
            else
            {
                if (LANG_CODE == "en-US")
                    uiText = "currently playing";
                else
                    uiText = "video già in riproduzione";
            }
        }
        else if ((result == "stop" && LANG_CODE == "en-US") || (result == "pausa" && LANG_CODE == "it-IT"))
        {
            if (isPlaying == true)
            {
                manager.OnPlayPause();

                if (LANG_CODE == "en-US")
                    uiText = "video paused";
                else
                    uiText = "video in pausa";

                isPlaying = false;

            }
            else
            {
                if (LANG_CODE == "en-US")
                    uiText = "video already paused";
                else
                    uiText = "video già in pausa";
            }
        }
        else if ((result == "volume up" && LANG_CODE == "en-US") || (result == "alza il volume" && LANG_CODE == "it-IT"))
        {
            manager.OnVolumeUp();
            if (LANG_CODE == "en-US")
                uiText = "volume raised";
            else
                uiText = "volume aumentato";
        }
        else if ((result == "volume down" && LANG_CODE == "en-US") || (result == "abbassa il volume" && LANG_CODE == "it-IT"))
        {
            manager.OnVolumeDown();
            if (LANG_CODE == "en-US")
                uiText = "volume reduced";
            else
                uiText = "volume diminuito";
        }
        else if ((result == "slow down" && LANG_CODE == "en-US") || (result == "riduci la velocità" && LANG_CODE == "it-IT"))
        {
            manager.OnSlowDown();
            if (LANG_CODE == "en-US")
                uiText = "playback speed decreased";
            else
                uiText = "velocità di riproduzione diminuita";
        }
        else if ((result == "speed up" && LANG_CODE == "en-US") || (result == "aumenta la velocità" && LANG_CODE == "it-IT"))
        {
            manager.OnSpeedUp();
            if (LANG_CODE == "en-US")
                uiText = "playback speed increased";
            else
                uiText = "velocità di riproduzione aumentata";
        }
        else if (((result.StartsWith("track") || result.StartsWith("truck") || result.StartsWith("drug")) && LANG_CODE == "en-US") || (result.StartsWith("traccia") && LANG_CODE == "it-IT"))
        {
            string suffix = LANG_CODE == "en-US" ? result.Remove(0, 5 + 1) : result.Remove(0, 7 + 1);

            string[] nums = getNumerals();
            bool isTrackSelected = false;
            try
            {
                int trackNum = int.Parse(suffix) - 1;
                manager.SelectTrack(trackNum);

                if (LANG_CODE == "en-US")
                    uiText = "playing track " + suffix;
                else
                    uiText = "riproduci traccia " + suffix;

                isTrackSelected = true;
            }
            catch
            {
                isTrackSelected = false;
                Debug.Log("String not in numeric format");
            }
            if (!isTrackSelected)
            {
                suffix = suffix.ToLower();
                for (int i = 0; i < nums.Length; i++)
                {
                    if (suffix.Equals(nums[i]))
                    {
                        manager.SelectTrack(i);
                        if (LANG_CODE == "en-US")
                            uiText = "playing track " + suffix;
                        else
                            uiText = "riproduci traccia " + suffix;

                        isTrackSelected = true;
                    }
                }
            }
            if (!isTrackSelected)
            {
                if (LANG_CODE == "en-US")
                    uiText = "No valid track found, please try again";
                else
                    uiText = "La traccia " + suffix + " non esiste";
            }
        }
        else if ((result == "show interface" && LANG_CODE == "en-US") || (result == "mostra i comandi" && LANG_CODE == "it-IT"))
        {
            if (!isControlActive)
            {
                mainCamera.transform.localScale = new Vector3(1, 1, 1);
                videoControls.gameObject.SetActive(true);
                speech.gameObject.SetActive(false);

                if (LANG_CODE == "en-US")
                    uiText = "Displaying user interface";
                else
                    uiText = "comandi abilitati";
            }
            else
            {
                if (LANG_CODE == "en-US")
                    uiText = "Interface already visible";
                else
                    uiText = "comandi già abilitati";
            }
        }
        else if ((result == "hide interface" && LANG_CODE == "en-US") || (result == "nascondi i comandi" && LANG_CODE == "it-IT"))
        {
            if (isControlActive)
            {
                mainCamera.transform.localScale = new Vector3(0.000001f, 0.000001f, 0.000001f);
                videoControls.gameObject.SetActive(false);
                speech.gameObject.SetActive(true);
                if (LANG_CODE == "en-US")
                    uiText = "Hiding user interface";
                else
                    uiText = "comandi disabilitati";
            }
            else
            {
                if (LANG_CODE == "en-US")
                    uiText = "Interface already hidden";
                else
                    uiText = "comandi già disabilitati";
            }
        }
        // /*
        else if ((result == "switch language" && LANG_CODE == "en-US") || (result == "cambia lingua" && LANG_CODE == "it-IT"))
        {
            if (LANG_CODE == "en-US")
            {
                uiText = "Lingua impostata in italiano";
                LANG_CODE = "it-IT";
            }
            else
            {
                uiText = "Language set to English";
                LANG_CODE = "en-US";
            }

            Setup(LANG_CODE);
        }
        // */
        else
        {
            //uiText = result;
            //text.text = result;
            if (LANG_CODE == "en-US")
                uiText = "No valid command found, please try again";
            else
                uiText = "Il comando non è disponibile, ritenta.";
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
