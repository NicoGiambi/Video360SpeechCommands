//-----------------------------------------------------------------------
// <copyright file="VideoControlsManager.cs" company="Google Inc.">
// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace Assets.Scripts

{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Video;

    /// <summary>Controls for the Video Player.</summary>
    public class MyVideoControlsManager : MonoBehaviour
    {
        private GameObject pauseSprite;
        private GameObject playSprite;
        private Slider videoScrubber;
        private Slider volumeSlider;
        private GameObject volumeWidget;
        // private GameObject settingsPanel;
        private GameObject bufferedBackground;
        private Vector3 basePosition;
        private Text videoPosition;
        private Text videoDuration;
        private Text framePos;
        private Text frameMax;
        private float volumeUnit;
        private Button[] tracks = new Button[11];
        private float[] trackTimes = new float[11];
        private string[] tracksTitles = new string[11];
        [SerializeField]
        Text url;

        /// <summary>Gets or sets the player.</summary>
        /// <value>The player.</value>
        public VideoPlayer Player { get; set; }

        /// <summary>Raises the volume up event.</summary>
        public void OnVolumeUp()
        {
            if (Player.canSetDirectAudioVolume)
                for (ushort track = 0; track <= Player.audioTrackCount; track++)
                    if (Player.GetDirectAudioVolume(track) < 1)
                        Player.SetDirectAudioVolume(track, Player.GetDirectAudioVolume(track) + volumeUnit);
        }

        /// <summary>Raises the volume down event.</summary>
        public void OnVolumeDown()
        {
            if (Player.canSetDirectAudioVolume)
                for (ushort track = 0; track <= Player.audioTrackCount; track++)
                    if (Player.GetDirectAudioVolume(track) > 0)
                        Player.SetDirectAudioVolume(track, Player.GetDirectAudioVolume(track) - volumeUnit);
        }

        /// <summary>Raises the toggle volume event.</summary>
        public void OnToggleVolume()
        {
            bool visible = !volumeWidget.activeSelf;
            volumeWidget.SetActive(visible);

            // close settings if volume opens.
            // settingsPanel.SetActive(settingsPanel.activeSelf && !visible);
        }

        /// <summary>Raises the toggle settings event.</summary>
        public void OnToggleSettings()
        {
            // bool visible = !settingsPanel.activeSelf;
            // settingsPanel.SetActive(visible);

            // close settings if volume opens.
            // volumeWidget.SetActive(volumeWidget.activeSelf && !visible);
        }

        /// <summary>Raises the play pause event.</summary>
        public void OnPlayPause()
        {
            bool isPaused = Player.isPaused;
            if (isPaused)
            {
                Player.Play();
            }
            else
            {
                Player.Pause();
            }

            pauseSprite.SetActive(isPaused);
            playSprite.SetActive(!isPaused);
            CloseSubPanels();
        }

        public void OnSlowDown()
        {
            Player.playbackSpeed *= 0.75f;
            CloseSubPanels();
        }

        public void OnSpeedUp()
        {
            Player.playbackSpeed *= 1.25f;
            CloseSubPanels();
        }

        private void getTrackTitles() 
        {
            tracksTitles[0] = "1. Waltzer";
            tracksTitles[1] = "2. Salsa";
            tracksTitles[2] = "3. Samba";
            tracksTitles[3] = "4. Cha";
            tracksTitles[4] = "5. Miao";
            tracksTitles[5] = "6. Regga";
            tracksTitles[6] = "7. Rock";
            tracksTitles[7] = "8. Punk";
            tracksTitles[8] = "9. Metal";
            tracksTitles[9] = "10. Pop";
            tracksTitles[10] = "11. Yes";
        }

        public void SelectTrack(int el)
        {
            for (int i = 0; i < trackTimes.Length; i++)
            {
                trackTimes[i] = (float)Player.frameCount / trackTimes.Length * i;
                //Debug.Log("<color=red>" + trackTimes[i].ToString() + "</color>");
            }

            Player.frame = (int)trackTimes[el];

        }


        /// <summary>Sets the volume to the given value.</summary>
        /// <param name="val">The new Volume value.</param>
        public void OnVolumePositionChanged(float val)
        {
            if (Player.isPrepared)
            {
                Debug.Log("Setting current volume to " + val);
                if (Player.canSetDirectAudioVolume)
                {
                    //framePos.text = val.ToString();
                    for (ushort track = 0; track <= Player.audioTrackCount; track++)
                        Player.SetDirectAudioVolume(track, val);
                }
            }
        }

        /// <summary>Closes the sub panels.</summary>
        public void CloseSubPanels()
        {
            volumeWidget.SetActive(false);
            // settingsPanel.SetActive(false);
        }

        /// <summary>Fade this video canvas in or out.</summary>
        /// <param name="show">
        /// Value `true` if this video should appear, or `false` if it should fade.
        /// </param>
        public void Fade(bool show)
        {
            if (show)
            {
                StartCoroutine(DoAppear());
            }
            else
            {
                StartCoroutine(DoFade());
            }
        }

        private void Awake()
        {
            getTrackTitles();

            foreach (Button t in GetComponentsInChildren<Button>())
            {
                if (t.gameObject.name.StartsWith("Button"))
                {
                    int suffix = int.Parse(t.gameObject.name.Remove(0, 6));
                    tracks[suffix] = t;
                    
                    getTrackTitles();

                    foreach (Text o in tracks[suffix].GetComponentsInChildren<Text>())
                    {
                        o.text = tracksTitles[suffix];
                    }

                }
            }
            foreach (Text t in GetComponentsInChildren<Text>())
            {
                if (t.gameObject.name == "curpos_text")
                {
                    videoPosition = t;
                }
                else if (t.gameObject.name == "duration_text")
                {
                    videoDuration = t;
                }
                else if (t.gameObject.name == "curframe_text")
                {
                    framePos = t;
                }
                else if (t.gameObject.name == "maxframe_text")
                {
                    frameMax = t;
                }
            }

            foreach (RawImage raw in GetComponentsInChildren<RawImage>(true))
            {
                if (raw.gameObject.name == "playImage")
                {
                    playSprite = raw.gameObject;
                }
                else if (raw.gameObject.name == "pauseImage")
                {
                    pauseSprite = raw.gameObject;
                }
            }

            foreach (Slider s in GetComponentsInChildren<Slider>(true))
            {
                if (s.gameObject.name == "video_slider")
                {
                    videoScrubber = s;
                    videoScrubber.maxValue = 100;
                    videoScrubber.minValue = 0;
                    foreach (Image i in videoScrubber.GetComponentsInChildren<Image>())
                    {
                        if (i.gameObject.name == "BufferedBackground")
                        {
                            bufferedBackground = i.gameObject;
                        }
                    }
                }
                else if (s.gameObject.name == "volume_slider")
                {
                    volumeSlider = s;
                }
            }

            foreach (RectTransform obj in GetComponentsInChildren<RectTransform>(true))
            {
                if (obj.gameObject.name == "volume_widget")
                {
                    volumeWidget = obj.gameObject;
                }
                /*
                else if (obj.gameObject.name == "settings_panel")
                {
                    settingsPanel = obj.gameObject;
                }
                */
            }
        }

        private void Start()
        {
            foreach (MyScrubberEvents s in GetComponentsInChildren<MyScrubberEvents>(true))
            {
                s.ControlManager = this;
            }

            if (Player != null)
            {
                Player.url = url.text;
                Player.Prepare();
            }

        }

        private void Update()
        {
            getTrackTitles();

            if (!Player.isPrepared || Player.isPaused)
            {
                pauseSprite.SetActive(false);
                playSprite.SetActive(true);
            }
            else if (Player.isPrepared && !Player.isPaused)
            {
                pauseSprite.SetActive(true);
                playSprite.SetActive(false);
            }

            if (Player.isPrepared)
            {
                if (basePosition == Vector3.zero)
                {
                    basePosition = videoScrubber.handleRect.localPosition;
                }

                videoScrubber.maxValue = Player.frameCount;
                videoScrubber.value = Player.frame;

                float pct = 1;
                float sx = Mathf.Clamp(pct, 0, 1f);
                bufferedBackground.transform.localScale = new Vector3(sx, 1, 1);
                bufferedBackground.transform.localPosition =
                    new Vector3(basePosition.x - (basePosition.x * sx), 0, 0);

                // videoPosition.text = FormatTime((long)Player.time * 1000);
                // videoDuration.text = FormatTime((long)Player.length * 1000);
                // framePos.text = Player.frame.ToString();
                // frameMax.text = Player.frameCount.ToString();



                if (volumeSlider != null)
                {
                    volumeUnit = 0.1f;
                    volumeSlider.minValue = 0;
                    volumeSlider.maxValue = 1;
                    float maxVolume = 0;
                    for (ushort track = 0; track <= Player.audioTrackCount; track++)
                    {
                        float currentVolume = Player.GetDirectAudioVolume(track);
                        if (currentVolume > maxVolume)
                        {
                            maxVolume = currentVolume;
                        }
                    }
                    volumeSlider.value = maxVolume;
                }

            }
            else
            {
                videoScrubber.value = 0;
            }
        }

        private IEnumerator DoAppear()
        {
            CanvasGroup cg = GetComponent<CanvasGroup>();
            while (cg.alpha < 1.0)
            {
                cg.alpha += Time.deltaTime * 2;
                yield return null;
            }

            cg.interactable = true;
            yield break;
        }

        private IEnumerator DoFade()
        {
            CanvasGroup cg = GetComponent<CanvasGroup>();
            while (cg.alpha > 0)
            {
                cg.alpha -= Time.deltaTime;
                yield return null;
            }

            cg.interactable = false;
            CloseSubPanels();
            yield break;
        }

        private string FormatTime(long ms)
        {
            int sec = (int)(ms / 1000L);
            int mn = sec / 60;
            sec = sec % 60;
            int hr = mn / 60;
            mn = mn % 60;
            if (hr > 0)
            {
                return string.Format("{0:00}:{1:00}:{2:00}", hr, mn, sec);
            }

            return string.Format("{0:00}:{1:00}", mn, sec);
        }
    }
}
