using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FxController : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> clips;
    [SerializeField]
    public AudioSource source;

    Dictionary<FX, AudioClip> fxClips;
    // Start is called before the first frame update
    void Start()
    {
        fxClips = new Dictionary<FX, AudioClip>();
        fxClips.Add(FX.Victory, clips[0]);
        fxClips.Add(FX.Defeat, clips[1]);
        fxClips.Add(FX.Boulder, clips[2]);
        fxClips.Add(FX.Good, clips[3]);
        fxClips.Add(FX.Bad, clips[4]);
    }

    // Update is called once per frame
    void Update()
    {

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
        if (level.Length>0) { level[0].GetComponent<AudioSource>().volume = 0.3f; }
        source.Play();
        StartCoroutine(WaitForVictory(nextLevel));
        return;
    }
    public void Defeat(ResetScene reset)
    {
        source.clip = fxClips[FX.Defeat];
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { level[0].GetComponent<AudioSource>().volume = 0.3f; }
        source.Play();
        StartCoroutine(WaitForDefeat(reset));
        return;
    }

    private IEnumerator WaitForVictory(string nextLevel)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { level[0].GetComponent<AudioSource>().volume = 1.0f; }
        SceneManager.LoadScene(nextLevel);
    }

    public IEnumerator WaitForDefeat(ResetScene reset)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { level[0].GetComponent<AudioSource>().volume = 1.0f; }
        reset.Reset();
    }
}
