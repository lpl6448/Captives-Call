using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FxController : MonoBehaviour
{
    public static FxController Instance;

    [SerializeField]
    private List<AudioClip> fxClipsList;
    [SerializeField]
    private List<AudioClip> musicClipsList;
    [SerializeField]
    public AudioSource source;

    Dictionary<FX, AudioClip> fxClips;
    Dictionary<FX, AudioClip> musicClips;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Place clips from lists into dictionaries
        fxClips = new Dictionary<FX, AudioClip>
        {
            { FX.Bad, fxClipsList[0] },
            { FX.Boulder, fxClipsList[1] },
            { FX.HighClick, fxClipsList[2] },
            { FX.Plate, fxClipsList[3] },
            { FX.LowClick, fxClipsList[4] },
            { FX.Hit, fxClipsList[5] },
            { FX.Water, fxClipsList[6] }
        };
        musicClips = new Dictionary<FX, AudioClip>
        {
            { FX.Victory, musicClipsList[0] },
            { FX.Defeat, musicClipsList[1] },
            { FX.Shanty, musicClipsList[2] },
            { FX.Warlock, musicClipsList[3] },
            { FX.Wizard, musicClipsList[4] },
            { FX.Pickpocket, musicClipsList[5] },
            { FX.All, musicClipsList[6] }
        };
        //Get reference to level music
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0)
        {
            AudioSource levelMusic = level[0].GetComponent<AudioSource>();
            UpdateLevelMusic(levelMusic);
            StartCoroutine(FadeIn(levelMusic, 1f));
        }
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
    public void Plate()
    {
        source.clip = fxClips[FX.Plate];
        source.PlayOneShot(fxClips[FX.Plate]);
    }
    public void Shanty()
    {
        source.clip = musicClips[FX.Shanty];
        GameObject[] level = GameObject.FindGameObjectsWithTag("LevelMusic");
        if (level.Length > 0) { level[0].GetComponent<AudioSource>().volume = 0f; }
        source.PlayOneShot(musicClips[FX.Shanty]);
        if (level.Length > 0) { StartCoroutine(FadeIn(level[0].GetComponent<AudioSource>(), 8f)); }
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

    private void UpdateLevelMusic(AudioSource levelMusic)
    {
        List<PartyMember> members = Party.Instance.partyMembers;
        //Change the audio clip if the level has different character composition than before
        //Run four character switch if the party only has one member
        if (members.Count == 1)
        {
            switch (Party.Instance.currentMember)
            {
                case PartyMember.Warlock:
                    if (levelMusic.clip != musicClips[FX.Warlock])
                    {
                        levelMusic.clip = musicClips[FX.Warlock];
                        levelMusic.Play();
                    }
                    break;
                case PartyMember.Wizard:
                    if (levelMusic.clip != musicClips[FX.Wizard])
                    {
                        levelMusic.clip = musicClips[FX.Wizard];
                        levelMusic.Play();
                    }
                    break;
                case PartyMember.Pickpocket:
                    if (levelMusic.clip != musicClips[FX.Pickpocket])
                    {
                        levelMusic.clip = musicClips[FX.Pickpocket];
                        levelMusic.Play();
                    }
                    break;
                case PartyMember.Sailor:
                    //Implement when sailor theme is written
                    DefaultMusic(levelMusic);
                    break;
            }
            return;
        }
        //Check all of the possible multi-char party comps
        //Wizard+Pickpocket
        if (members[0]==PartyMember.Wizard && members[1]==PartyMember.Pickpocket)
        {
            //Implement when theme is written
            DefaultMusic(levelMusic);
            return;
        }
        //Wizard+Pickpocket+Sailor
        if (members[0] == PartyMember.Wizard && members[1] == PartyMember.Pickpocket && members[2] == PartyMember.Sailor)
        {
            //Implement when theme is written
            DefaultMusic(levelMusic);
            return;
        }
        //Full party
        if (levelMusic.clip != musicClips[FX.All])
        {
            levelMusic.clip = musicClips[FX.All];
            levelMusic.Play();
        }
        return;
    }

    /// <summary>
    /// Remove when all themes have been written
    /// </summary>
    private void DefaultMusic(AudioSource levelMusic)
    {
        if (levelMusic.clip != musicClips[FX.All])
        {
            levelMusic.clip = musicClips[FX.All];
            levelMusic.Play();
        }
    }
}
