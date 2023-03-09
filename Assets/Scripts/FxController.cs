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
        source.PlayOneShot(fxClips[FX.Good], 0.6f);
        return;
    }
    public void BadClick()
    {
        source.clip = fxClips[FX.Bad];
        source.PlayOneShot(fxClips[FX.Bad], 0.6f);
        return;
    }
    public void Boulder()
    {
        source.clip = fxClips[FX.Boulder];
        source.PlayOneShot(fxClips[FX.Boulder], 0.8f);
        return;
    }
    public void Victory(string nextLevel)
    {
        source.clip = fxClips[FX.Victory];
        AudioSource level = GameObject.FindGameObjectsWithTag("LevelMusic")[0].GetComponent<AudioSource>();
        level.Pause();
        source.Play();
        StartCoroutine(WaitForVictory(nextLevel));
        return;
    }
    public void Defeat(ResetScene reset)
    {
        source.clip = fxClips[FX.Defeat];
        AudioSource level = GameObject.FindGameObjectsWithTag("LevelMusic")[0].GetComponent<AudioSource>();
        level.Pause();
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
        AudioSource level = GameObject.FindGameObjectsWithTag("LevelMusic")[0].GetComponent<AudioSource>();
        level.UnPause();
        SceneManager.LoadScene(nextLevel);
    }

    public IEnumerator WaitForDefeat(ResetScene reset)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        AudioSource level = GameObject.FindGameObjectsWithTag("LevelMusic")[0].GetComponent<AudioSource>();
        level.UnPause();
        reset.Reset();
    }
}
