namespace Assets.Scripts
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>Controls for the Video Player.</summary>
    public class CatalogManager : MonoBehaviour
    {

        [SerializeField]
        Text mytext;
        [SerializeField]
        GameObject Viewport;
        [SerializeField]
        Camera mainCamera;
        [SerializeField]
        GameObject player;
        [SerializeField]
        GameObject catalog;

        string[] videoNames;
        bool noVideos = true;

        UnityEngine.Video.VideoPlayer[] videos;

        private void Start()
        {
            mainCamera.transform.localScale = new Vector3(1, 1, 1);

            // INSERT SERVER ENDPOINT FOLDER HERE
            // string uri = "http://localhost:8000/UnityProjects/Video360SpeechCommands/Assets/Videos/";
            string uri = "http://9ca8-137-204-11-201.ngrok.io/data01/video360/videos/";
            WebRequest request = WebRequest.Create(uri);
            request.Headers.Add("ngrok-skip-browser-warning", "ciao");
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch
            {
                response = null;
            }

            Regex regex = new Regex("<a href=\".*[.mp4]\">(?<name>.*[.mp4])</a>");

            if (response != null)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();

                    MatchCollection matches = regex.Matches(result);
                    if (matches.Count == 0)
                    {
                        Debug.Log("parse failed.");
                        disableObjects("No videos found");
                        return;
                    }
                    else
                    {
                        noVideos = false;
                        videoNames = new String[matches.Count];
                        var j = 0;
                        foreach (Match match in matches)
                        {
                            if (!match.Success) { continue; }
                            videoNames[j] = match.Groups["name"].ToString();
                            //Debug.Log(match.Groups["name"]);
                            j++;
                        }
                    }
                }

                if (!noVideos)
                {
                    var imgs = 0;
                    foreach (RawImage img in Viewport.GetComponentsInChildren<RawImage>())
                    {
                        imgs++;
                    }

                    videos = new UnityEngine.Video.VideoPlayer[imgs];

                    var t = 0;

                    foreach (Text txt in Viewport.GetComponentsInChildren<Text>())
                    {
                        if (t < videoNames.Length)
                            txt.text = videoNames[t];
                        else
                            txt.gameObject.SetActive(false);

                        t++;
                    }

                    var i = 0;

                    foreach (RawImage img in Viewport.GetComponentsInChildren<RawImage>())
                    {
                        if (i < videoNames.Length)
                        {

                            videos[i] = img.GetComponent<UnityEngine.Video.VideoPlayer>();
                            // Play on awake defaults to true. Set it to false to avoid the url set
                            // below to auto-start playback since we're in Start().
                            videos[i].playOnAwake = false;

                            // Set the video to play. URL supports local absolute or relative paths.
                            // Here, using absolute.
                            videos[i].url = uri + videoNames[i];

                            // Debug.Log(videos[i].url);

                            videos[i].SetDirectAudioMute(0, true);

                            videos[i].Prepare();

                            // Skip the first 100 frames.
                            videos[i].frame = (int)videos[i].frameCount / 2;

                            videos[i].Play();
                            
                            i++;
                        }
                        else
                        {
                            img.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    mytext.text = "No videos found";
                    mytext.color = Color.white;
                    mytext.fontSize = 100;
                    mytext.rectTransform.localScale = new Vector3((float)0.5, (float)0.5, 1);
                }
            }
            else 
            {
                disableObjects("No server found");
            }
        }

        private void Update()
        {
            // for (var i = 0; i < videos.Length; i++) 
            // {
                //for (var k = 0; (k < videos.Length && k != i); k++)
                //    videos[k].Pause();
               
            // }
        }

        private void disableObjects(string feedback) 
        {
            foreach (Text txt in Viewport.GetComponentsInChildren<Text>())
            {
                txt.gameObject.SetActive(false);
            }
            foreach (RawImage img in Viewport.GetComponentsInChildren<RawImage>())
            {
                img.gameObject.SetActive(false);
            }
            mytext.text = feedback;
            mytext.color = Color.white;
            mytext.fontSize = 100;
            mytext.rectTransform.localScale = new Vector3((float)0.5, (float)0.5, 1);
        }

        public void OnVideoSelect(int id)
        {

            mainCamera.transform.localScale = new Vector3(0, 0, 0);
            mytext.text = videos[id].url;
            catalog.SetActive(false);
            player.SetActive(true);
        }
    }
}
