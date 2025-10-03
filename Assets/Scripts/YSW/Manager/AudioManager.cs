using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string bgmVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    private readonly Dictionary<string, AudioClip> loadedClips = new();
    private Coroutine bgmFadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        PlayBGM("BGM_Loop");
    }
    public void Update()
    {
    }

    #region Addressables Load
    public async Task<AudioClip> LoadClipAsync(string addressKey)
    {
        if (loadedClips.TryGetValue(addressKey, out var cachedClip))
            return cachedClip;

        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(addressKey);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedClips[addressKey] = handle.Result;
            return handle.Result;
        }

        Debug.LogError($"[AudioManager] Failed to load clip: {addressKey}");
        return null;
    }

    public void UnloadClip(string addressKey)
    {
        if (loadedClips.TryGetValue(addressKey, out var clip))
        {
            Addressables.Release(clip);
            loadedClips.Remove(addressKey);
        }
    }
    #endregion

    #region SFX
    public async void PlaySFX(string addressKey)
    {
        var clip = await LoadClipAsync(addressKey);
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip audioClip)
    {        
        if (audioClip != null)
            sfxSource.PlayOneShot(audioClip);
    }

    public async void PlaySFXLoop(string addressKey)
    {
        var clip = await LoadClipAsync(addressKey);
        if (clip != null)
        {
            sfxSource.clip = clip;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void PlaySFXLoop(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            sfxSource.clip = audioClip;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopSFX()
    {
        sfxSource.Stop();
        sfxSource.loop = false;
    }
    #endregion

    #region BGM
    public async void PlayBGM(string addressKey, bool loop = true)
    {
        var clip = await LoadClipAsync(addressKey);
        if (clip != null)
        {
            StopBGM();
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void PlayBGM(AudioClip audioClip, bool loop = true)
    {
        if (audioClip != null)
        {
            StopBGM();
            bgmSource.clip = audioClip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.loop = false;
    }

    public async void FadeInBGM(string addressKey, float duration, bool loop = true)
    {
        var clip = await LoadClipAsync(addressKey);
        if (clip == null) return;

        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeInCoroutine(clip, duration, loop));
    }

    public void FadeInBGM(AudioClip audioClip, float duration, bool loop = true)
    {
        if (audioClip == null) return;
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeInCoroutine(audioClip, duration, loop));
    }

    public void FadeOutBGM(float duration)
    {
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    public void FadeOutBGM(AudioClip audioClip, float duration)
    {
        if (audioClip == null) return;
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }
    #endregion

    #region Volume Control via AudioMixer
    public void SetBGMVolume(float volume) => audioMixer.SetFloat(bgmVolumeParam, Mathf.Log10(volume) * 20);
    public void SetSFXVolume(float volume) => audioMixer.SetFloat(sfxVolumeParam, Mathf.Log10(volume) * 20);
    #endregion

    #region Fade Coroutines
    private System.Collections.IEnumerator FadeInCoroutine(AudioClip clip, float duration, bool loop)
    {
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        bgmSource.volume = 1f;
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = 1f;
    }
    #endregion
}
