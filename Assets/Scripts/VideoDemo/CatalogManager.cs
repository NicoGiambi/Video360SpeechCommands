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

        String[] videoNames;

        UnityEngine.Video.VideoPlayer[] videos;

        private void Start()
        {
            mainCamera.transform.localScale = new Vector3(1, 1, 1);

            string uri = "http://localhost:8000/Desktop/Anime/Excel%20Saga/";
            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();
            Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();

                MatchCollection matches = regex.Matches(result);
                if (matches.Count == 0)
                {
                    Debug.Log("parse failed.");
                    return;
                }

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

            var imgs = 0;
            foreach (RawImage img in Viewport.GetComponentsInChildren<RawImage>())
            {
                imgs++;
            }

            videos = new UnityEngine.Video.VideoPlayer[imgs];

            var t = 0;

            foreach (Text txt in Viewport.GetComponentsInChildren<Text>())
            {
                txt.text = videoNames[t];
                t++;
            }

            var i = 0;

            foreach (RawImage img in Viewport.GetComponentsInChildren<RawImage>())
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

                // videoPlayer.Pause();

                // Start playback. This means the VideoPlayer may have to prepare (reserve
                // resources, pre-load a few frames, etc.). To better control the delays
                // associated with this preparation one can use videoPlayer.Prepare() along with
                // its prepareCompleted event.
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

        public void OnVideoSelect(int id)
        {
            mainCamera.transform.localScale = new Vector3(0, 0, 0);
            catalog.SetActive(false);
            player.SetActive(true);
            mytext.text = videos[id].url;
        }
    }
}
