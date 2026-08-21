using System;
using System.Collections.Generic;
using Mirror;
using Security;
using UnityEngine;

public class AmbientSoundPlayer : NetworkBehaviour
{
    [Serializable]
    public class AmbientClip
    {
        public AudioClip clip;
        public bool repeatable;
        public bool is3D;
        public bool played;
        [HideInInspector]
        public int index;
    }

    public GameObject audioPrefab;
    public int minTime = 30;
    public int maxTime = 60;
    public AmbientClip[] clips;

    private List<AmbientClip> list = new List<AmbientClip>();
    private RateLimit _ambientSoundRateLimit = new RateLimit(4, 3f, null);

    private void Start()
    {
        if (isLocalPlayer && isServer)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].index = i;
            }

            Invoke(nameof(GenerateRandom), 10f);
        }
    }

    [Server]
    private void GenerateRandom()
    {
        list.Clear();

        foreach (var clip in clips)
        {
            if (!clip.played)
            {
                list.Add(clip);
            }
        }

        if (list.Count == 0) return;

        int randomIdx = UnityEngine.Random.Range(0, list.Count);
        AmbientClip selected = list[randomIdx];

        if (!selected.repeatable)
        {
            clips[selected.index].played = true;
        }

        RpcPlaySound(selected.index);

        float nextTime = UnityEngine.Random.Range((float)minTime, (float)maxTime);
        Invoke(nameof(GenerateRandom), nextTime);
    }

    [ClientRpc]
    private void RpcPlaySound(int id)
    {
        if (_ambientSoundRateLimit.CanExecute(true))
        {
            PlaySound(id);
        }
    }

    private void PlaySound(int clipID)
    {
        if (clipID < 0 || clipID >= clips.Length) return;

        GameObject audioObj = Instantiate(audioPrefab);

        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        audioObj.transform.position = transform.position + randomOffset;

        AudioSource source = audioObj.GetComponent<AudioSource>();
        AmbientClip data = clips[clipID];

        source.clip = data.clip;
        source.spatialBlend = data.is3D ? 1f : 0f;
        source.Play();

        Destroy(audioObj, 10f);
    }
}