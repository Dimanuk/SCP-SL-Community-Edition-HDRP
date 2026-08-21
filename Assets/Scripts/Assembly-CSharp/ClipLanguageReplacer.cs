public class ClipLanguageReplacer : global::UnityEngine.MonoBehaviour
{
    [global::UnityEngine.SerializeField]
    public global::UnityEngine.AudioClip englishVersion;

    private string file;

    private global::System.Collections.IEnumerator Start()
    {
        global::UnityEngine.AudioSource asource = GetComponent<global::UnityEngine.AudioSource>();
        string path = (TranslationReader.TranslationPath + "/Custom Audio/" + asource.clip.name + ".ogg").Replace('\\', '/');
        if (global::System.IO.File.Exists(path))
        {
            if (asource.playOnAwake)
            {
                asource.Stop();
            }
            string clipname = asource.clip.name;
            using (global::UnityEngine.Networking.UnityWebRequest www = global::UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(path.StartsWith("/") ? "file://" : ("file:///" + path), Misc.GetAudioType(path)))
            {
                global::UnityEngine.Debug.Log("Audio Downloading");
                yield return www.SendWebRequest();
                global::UnityEngine.Debug.Log("Audio Downloaded: " + path);
                asource.clip = global::UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
            }
            asource.clip.name = clipname;
            if (asource.playOnAwake)
            {
                asource.Play();
            }
        }
        else
        {
            asource.clip = englishVersion;
        }
    }

    private void OnDestroy()
    {
        GetComponent<global::UnityEngine.AudioSource>().clip.UnloadAudioData();
    }
}
