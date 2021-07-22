using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

public class SoundManager : MonoBehaviour
{
    //Singleton instance
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }    
    
    private Transform cameraTransform;
    private GameObject lastOneShotAudioCreated;
    private List<GameObject> lastOneShotAudioCreatedList = new List<GameObject>();

    private float lastTimeAudioPlayed;
    private string lastAudioPlayed;

    private List<string> randomAudioPlayed = new List<string>();

    private List<GameObject> unusedAudioSourcePool = new List<GameObject>();
    private List<GameObject> audioSourceInUsePool = new List<GameObject>();

    private Dictionary<string, List<AudioClip>> queuedUpSound = new Dictionary<string, List<AudioClip>>();

    public static bool reportError = true;  //Put this a false to deactive error reporting

    void Awake()
    {
        if (Camera.main == null)
        {
            Debug.LogError("There is no main camera in the scene!");
            return;
        }
        cameraTransform = Camera.main.transform;
    }
    
    private void DoPlayAudio(AudioClip audioClip, float pitch, float volume)
    {
        if (Time.time - lastTimeAudioPlayed > 0.4f || audioClip.name != lastAudioPlayed)
        {
            GameObject soundGameObject = PlayClip(audioClip, volume);
            soundGameObject.transform.parent = cameraTransform;
            soundGameObject.transform.localPosition = Vector3.zero;
            soundGameObject.GetComponent<AudioSource>().spatialBlend = 0f;
            soundGameObject.GetComponent<AudioSource>().pitch = pitch;
            lastAudioPlayed = audioClip.name;
            lastTimeAudioPlayed = Time.time;
        }
    }

    public void RefreshCamera()
    {
        if (Camera.main == null)
        {
            Debug.LogError("There is no main camera in the scene!");
            return;
        }
        cameraTransform = Camera.main.transform;
    }

    public void SetCamera(Camera cam)
    {
        cameraTransform = cam.transform;
    }

    void OnDestroy()
    {
        instance = null;

        unusedAudioSourcePool = new List<GameObject>();
        audioSourceInUsePool = new List<GameObject>();
    }

    public void PlaySoundEvent(AudioClip audio)
    {
        PlaySound(audio);
    }

    public GameObject PlaySoundReturnGameObject(AudioClip pAudioClip, Vector3 position)
    {
        AudioClip soundClip = pAudioClip;
        return ExecuteSoundGetGameObject(soundClip, true, position);
    }

    public float PlaySoundWithDelay(AudioClip pSoundClip, float delay)
    {
        StartCoroutine(WaitBeforePlayingSound(pSoundClip, delay));
        return pSoundClip.length;
    }

    public float PlaySound(AudioClip pSoundClip, float volume)
    {
        return ExecuteSoundWithVolume(pSoundClip, false, Vector3.zero, volume);
    }

    public float PlaySoundWithPitch(AudioClip pSoundClip, float fPitch)
    {
        return ExecuteSoundWithPitch(pSoundClip, fPitch);
    }

    private IEnumerator WaitBeforePlayingSound(AudioClip pSoundClip, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ExecuteSound(pSoundClip, false, Vector3.zero);
    }

    public float PlaySound(AudioClip pSoundClip)
    {
        if (pSoundClip == null)
        {
            Debug.LogError("Tryed to play a null sound");
            return 0;
        }
        return ExecuteSound(pSoundClip, false, Vector3.zero);
    }

    public float PlaySoundNo3D(AudioClip pSoundClip)
    {
        return ExecuteSoundNo3D(pSoundClip);
    }

    public float PlaySoundBypassTimeException(AudioClip pSoundClip)
    {
        if (pSoundClip == null)
        {
            Debug.LogError("SoundManager : Couldn't find audio resource");
            return 0;
        }
        else
        {
            return ExecuteSound(pSoundClip, false, Vector3.zero, true);
        }
    }

    public float PlaySoundBypassTimeException(AudioClip pSoundClip, float volume)
    {
        if (pSoundClip == null)
        {
            Debug.LogError("SoundManager : Couldn't find audio resource");
            return 0;
        }
        else
        {
            return ExecuteSoundWithVolume(pSoundClip, false, Vector3.zero, volume, true);
        }
    }

    public float PlaySoundAt(AudioClip pSoundClip, Vector3 pSoundPosition)
    {
        return ExecuteSound(pSoundClip, true, pSoundPosition);
    }

    private GameObject ExecuteSoundGetGameObject(AudioClip pSoundClip, bool pIs3DSound, Vector3 p3DSoundPosition)
    {
        GameObject soundGameObject = null;
        if (Time.time - lastTimeAudioPlayed > 0.4f || pSoundClip.name != lastAudioPlayed)
        {
            if (pIs3DSound)
            {

                soundGameObject = PlayClipAt(pSoundClip, p3DSoundPosition);
            }
            else
            {
                soundGameObject = PlayClipAt(pSoundClip, Vector3.zero);
                soundGameObject.transform.parent = cameraTransform;
                soundGameObject.transform.localPosition = Vector3.zero;

            }
        }
        return soundGameObject;
    }

    private float ExecuteSoundNo3D(AudioClip pSoundClip, float volume = 1)
    {
        if (Time.time - lastTimeAudioPlayed > 0.4f || pSoundClip.name != lastAudioPlayed)
        {
            GameObject soundGameObject = PlayClip(pSoundClip, volume);
            soundGameObject.transform.parent = cameraTransform;
        }
        return pSoundClip.length;
    }

    private float ExecuteSound(AudioClip pSoundClip, bool pIs3DSound, Vector3 p3DSoundPosition)
    {
        if (Time.time - lastTimeAudioPlayed > 0.4f || pSoundClip.name != lastAudioPlayed)
        {
            if (pIs3DSound)
            {
                PlayClipAt(pSoundClip, p3DSoundPosition);
            }
            else
            {
                GameObject soundGameObject = PlayClipAt(pSoundClip, Vector3.zero);
                soundGameObject.GetComponent<AudioSource>().spatialBlend = 0f;
                soundGameObject.transform.parent = cameraTransform;
                soundGameObject.transform.localPosition = Vector3.zero;
            }
        }
        return pSoundClip.length;
    }

    private float ExecuteSound(AudioClip pSoundClip, bool pIs3DSound, Vector3 p3DSoundPosition, bool bypassRepeatedSoundException = false)
    {
        if ((bypassRepeatedSoundException) || (Time.time - lastTimeAudioPlayed > 0.4f || pSoundClip.name != lastAudioPlayed))
        {
            if (pIs3DSound)
            {
                PlayClipAt(pSoundClip, p3DSoundPosition);
            }
            else
            {
                GameObject soundGameObject = PlayClipAt(pSoundClip, Vector3.zero);
                soundGameObject.transform.parent = cameraTransform;
                soundGameObject.transform.localPosition = Vector3.zero;

            }
        }
        return pSoundClip.length;
    }

    private float ExecuteSoundWithVolume(AudioClip pSoundClip, bool pIs3DSound, Vector3 p3DSoundPosition, float volume, bool bypassRepeatedSoundException = false)
    {
        if ((Time.time - lastTimeAudioPlayed > 0.4f || pSoundClip.name != lastAudioPlayed) || (bypassRepeatedSoundException))
        {
            if (pIs3DSound)
            {
                PlayClipAt(pSoundClip, p3DSoundPosition);
            }
            else
            {
                GameObject soundGameObject = PlayClipWithVolumeAt(pSoundClip, Vector3.zero, volume);
                soundGameObject.transform.parent = cameraTransform;
                soundGameObject.transform.localPosition = Vector3.zero;
            }
        }

        return pSoundClip.length;
    }

    private float ExecuteSoundWithPitch(AudioClip pSoundClip, float fPitch)
    {
        if (Time.time - lastTimeAudioPlayed > 0.4f || pSoundClip.name != lastAudioPlayed)
        {
            GameObject soundGameObject = PlayClipWithSpeed(pSoundClip, fPitch);
            soundGameObject.transform.parent = cameraTransform;
            soundGameObject.transform.localPosition = Vector3.zero;
        }
        return pSoundClip.length;
    }

    private GameObject PlayClipAt(AudioClip clip, Vector3 pos)
    {
        GameObject tempGO = GetAudioSource();
        AudioSource aSource = tempGO.GetComponent<AudioSource>();
        aSource.enabled = true;

        tempGO.transform.position = pos;

        aSource.spatialBlend = 1f;
        aSource.clip = clip;

        lastAudioPlayed = clip.name;
        lastTimeAudioPlayed = Time.time;

        aSource.minDistance = 10;

        aSource.Play();

        StartCoroutine(PoolAudioSource(tempGO, clip.length + 1.5f));
        lastOneShotAudioCreated = tempGO;
        lastOneShotAudioCreatedList.Add(lastOneShotAudioCreated);
        return tempGO;
    }

    private GameObject PlayClipWithSpeed(AudioClip clip, float pPitch)
    {
        GameObject tempGO = GetAudioSource();
        AudioSource aSource = tempGO.GetComponent<AudioSource>();
        aSource.clip = clip;
        aSource.pitch = pPitch;
        aSource.spatialBlend = 1f;

        lastAudioPlayed = clip.name;
        lastTimeAudioPlayed = Time.time;

        aSource.minDistance = 10;

        aSource.Play();

        StartCoroutine(PoolAudioSource(tempGO, clip.length));
        lastOneShotAudioCreated = tempGO;
        lastOneShotAudioCreatedList.Add(lastOneShotAudioCreated);
        return tempGO;
    }

    private GameObject PlayClipWithVolumeAt(AudioClip clip, Vector3 pos, float volume)
    {
        GameObject tempGO = GetAudioSource();
        AudioSource aSource = tempGO.GetComponent<AudioSource>();
        tempGO.transform.position = pos;
        aSource.clip = clip;
        aSource.spatialBlend = 1f;

        lastAudioPlayed = clip.name;
        lastTimeAudioPlayed = Time.time;

        aSource.minDistance = 10;
        aSource.volume = volume;

        aSource.Play();
        StartCoroutine(PoolAudioSource(tempGO, clip.length));
        lastOneShotAudioCreated = tempGO;
        lastOneShotAudioCreatedList.Add(lastOneShotAudioCreated);
        return tempGO;
    }

    public GameObject LastOneShotAudioGameObject
    {
        get
        {
            return lastOneShotAudioCreated;
        }
    }

    public List<GameObject> LastOneShotAudioGameObjectList
    {
        get
        {
            return lastOneShotAudioCreatedList;
        }
    }

    public void StopAllSounds()
    {
        foreach (AudioSource aus in FindObjectsOfType<AudioSource>()) aus.Stop();
    }

    public void StopSound()
    {
        if (lastOneShotAudioCreated != null)
        {
            lastOneShotAudioCreated.GetComponent<AudioSource>().Stop();
        }
    }

    public void StopSoundFadingVolume()
    {
        AudioListener.volume = 0;
    }
    
    private GameObject PlayClipWithVolumeAt(AudioClip clip, Vector3 pos, float volume, AudioMixerGroup mixerGroup)
    {
        GameObject tempGO = GetAudioSource();
        AudioSource aSource = tempGO.GetComponent<AudioSource>();
        tempGO.transform.position = pos;

        aSource.clip = clip;
        aSource.spatialBlend = 1f;

        aSource.outputAudioMixerGroup = mixerGroup;

        aSource.minDistance = 10;
        aSource.volume = volume;
        aSource.Play();

        StartCoroutine(PoolAudioSource(tempGO, clip.length));
        lastOneShotAudioCreated = tempGO;
        lastOneShotAudioCreatedList.Add(lastOneShotAudioCreated);
        return tempGO;
    }

    public float PlaySound(AudioClip pSoundClip, float volume, AudioMixerGroup mixerGroup)
    {
        return ExecuteSoundWithVolume(pSoundClip, false, Vector3.zero, volume, mixerGroup);
    }

    private GameObject PlayClip(AudioClip clip, float volume)
    {
        GameObject tempGO = GetAudioSource();
        AudioSource aSource = tempGO.GetComponent<AudioSource>();

        if (clip != null)
        {
            aSource.clip = clip;

            aSource.PlayOneShot(clip);
            aSource.volume = volume;
            StartCoroutine(PoolAudioSource(tempGO, clip.length * 2.0f));
        }
        else
        {
            StartCoroutine(PoolAudioSource(tempGO, 2.0f));
        }

        lastOneShotAudioCreated = tempGO;
        lastOneShotAudioCreatedList.Add(lastOneShotAudioCreated);
        return tempGO;
    }

    private GameObject PlayClipAt(AudioClip clip, Vector3 pos, AudioMixerGroup mixerGroup)
    {
        GameObject tempGO = GetAudioSource();
        AudioSource aSource = tempGO.GetComponent<AudioSource>();

        tempGO.transform.position = pos;

        if (clip != null)
        {
            aSource.clip = clip;
            aSource.outputAudioMixerGroup = mixerGroup;
            aSource.minDistance = 10;
            aSource.spatialBlend = 1f;

            aSource.PlayOneShot(clip);
            StartCoroutine(PoolAudioSource(tempGO, clip.length + 1.0f));
        }
        else
        {
            Debug.LogError("CLIP MISSING");
            StartCoroutine(PoolAudioSource(tempGO, 1.0f));
        }

        lastOneShotAudioCreated = tempGO;
        lastOneShotAudioCreatedList.Add(lastOneShotAudioCreated);
        return tempGO;
    }

    public IEnumerator PlaySoundWithDelayAtStart(AudioClip soundClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return ExecuteSound(soundClip, false, Vector3.zero);
    }

    public void FadeOutAudioSource(AudioSource audioSource, float fadeTime)
    {
        StartCoroutine(FadeOutRoutine(audioSource, fadeTime));
    }

    private IEnumerator FadeOutRoutine(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator PoolAudioSource(GameObject audioSourceGO, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnAudioSourceToPool(audioSourceGO);
    }

    public void ReturnAudioSourceToPool(GameObject audioSourceGO)
    {
        if (audioSourceInUsePool.Contains(audioSourceGO))
        {
            audioSourceInUsePool.Remove(audioSourceGO);
            unusedAudioSourcePool.Add(audioSourceGO);
        }
    }
    
    public GameObject GetAudioSource()
    {
        GameObject audioSource;
        if (unusedAudioSourcePool.Count > 0)
        {
            audioSource = unusedAudioSourcePool[unusedAudioSourcePool.Count - 1];
            audioSourceInUsePool.Add(audioSource);
            unusedAudioSourcePool.Remove(audioSource);

            if (audioSource == null || audioSource.GetComponent<AudioSource>().enabled == false)
            {
                audioSource = new GameObject("PooledAudioSource");
                audioSource.AddComponent<AudioSource>();
                audioSourceInUsePool.Add(audioSource);
            }
        }
        else
        {
            audioSource = new GameObject("PooledAudioSource");
            audioSource.AddComponent<AudioSource>();
            audioSourceInUsePool.Add(audioSource);
        }

        audioSource.GetComponent<AudioSource>().pitch = 1f;
        audioSource.GetComponent<AudioSource>().volume = 1f;
        audioSource.GetComponent<AudioSource>().spatialBlend = 0;

        return audioSource;
    }

    public void EnableSound()
    {
    }
}