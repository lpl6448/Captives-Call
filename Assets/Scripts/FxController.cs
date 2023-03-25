using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FxController : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> fxClipsList;
    [SerializeField]
    private List<AudioClip> musicClipsList;
    [SerializeField]
    public AudioSource source;

    Dictionary<FX, AudioClip> fxClips;
    Dictionary<FX, AudioClip> musicClips;


    // Start is called before the first frame update
    void Start()
    {
        fxClips = new Dictionary<FX, AudioClip>();
        fxClips.Add(FX.Bad, fxClipsList[0]);
        fxClips.Add(FX.Boulder, fxClipsList[1]);
        fxClips.Add(FX.HighClick, fxClipsList[2]);
        fxClips.Add(FX.Gate, fxClipsList[3]);
        fxClips.Add(FX.LowClick, fxClipsList[4]);
        fxClips.Add(FX.Hit, fxClipsList[5]);
        fxClips.Add(FX.Water, fxClipsList[6]);
        musicClips = new Dictionary<FX, AudioClip>();
        musicClips.Add(FX.Victory, musicClipsList[0]);
        musicClips.Add(FX.Defeat, musicClipsList[1]);
        musicClips.Add(FX.Shanty, musicClipsList[2]);
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { StartCoroutine(FadeIn(level[0].GetComponent<AudioSource>(), 1f)); }
    }

    public void GoodClick()
    {
        source.clip = fxClips[FX.LowClick];
        source.PlayOneShot(fxClips[FX.LowClick], 0.9f);
        return;
    }
    public void BadClick()
    {
        source.clip = fxClips[FX.Bad];
        source.PlayOneShot(fxClips[FX.Bad], 0.9f);
        return;
    }
    public void Boulder()
    {
        source.clip = fxClips[FX.Boulder];
        source.PlayOneShot(fxClips[FX.Boulder]);
        return;
    }
    public void Victory()
    {
        source.clip = musicClips[FX.Victory];
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) {StartCoroutine(FadeOut(level[0].GetComponent<AudioSource>(), 0.1f)); }
        source.Play();
        return;
    }
    public void Defeat()
    {
        source.clip = musicClips[FX.Defeat];
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { StartCoroutine(FadeOut(level[0].GetComponent<AudioSource>(), 0.1f)); }
        source.Play();
        return;
    }

    public static IEnumerator FadeOut(AudioSource source, float fadeTime)
    {
        float startVolume = source.volume;
        while (source.volume>0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public static IEnumerator FadeIn(AudioSource source, float fadeTime)
    {
        source.volume = 0f;
        while (source.volume < 1)
        {
            source.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
