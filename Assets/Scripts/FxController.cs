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


    // Start is called before the first frame update
    void Start()
    {
        fxClips = new Dictionary<FX, AudioClip>();
        fxClips.Add(FX.Victory, fxClipsList[0]);
        fxClips.Add(FX.Defeat, fxClipsList[1]);
        fxClips.Add(FX.Boulder, fxClipsList[2]);
        fxClips.Add(FX.Good, fxClipsList[3]);
        fxClips.Add(FX.Bad, fxClipsList[4]);
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { StartCoroutine(FadeIn(level[0].GetComponent<AudioSource>(), 1f)); }
    }

    public void GoodClick()
    {
        source.clip = fxClips[FX.Good];
        source.PlayOneShot(fxClips[FX.Good], 0.9f);
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
    public void Victory(string nextLevel)
    {
        source.clip = fxClips[FX.Victory];
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) {StartCoroutine(FadeOut(level[0].GetComponent<AudioSource>(), 0.1f)); }
        source.Play();
        StartCoroutine(WaitForVictory(nextLevel));
        return;
    }
    public void Defeat()
    {
        source.clip = fxClips[FX.Defeat];
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { StartCoroutine(FadeOut(level[0].GetComponent<AudioSource>(), 0.1f)); }
        source.Play();
        return;
    }

    private IEnumerator WaitForVictory(string nextLevel)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        SceneManager.LoadScene(nextLevel);
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
