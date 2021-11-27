using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    [SerializeField] private int totalRacers = 8;
    public static int TotalRacers { get => _instance.totalRacers; }

    [SerializeField] private int laps = 2;
    public static int Laps { get => _instance.laps; }

    private PlayerSelection[] players;
    public static PlayerSelection[] Players { get => _instance.players; }

    [Header("Transitions")]
    [SerializeField] private Canvas transitionCanvas;
    [SerializeField] private RectTransform transition;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private Ease transitionEaseIn = Ease.InSine;
    [SerializeField] private Ease transitionEaseOut = Ease.OutSine;

    private MusicLoop music;
    private SoundController sound;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        this.music = GetComponentInChildren<MusicLoop>();
        this.sound = GetComponentInChildren<SoundController>();
    }

    // SCENES

    public static void LoadScene(Scenes scene) => _instance.StartCoroutine(_instance.LoadSceneLocal(scene));

    public static void SetPlayersAndPlay(IEnumerable<PlayerSelection> players, Scenes scene)
    {
        _instance.players = players.ToArray();
        LoadScene(scene);
    }

    private IEnumerator LoadSceneLocal(Scenes scene)
    {
        this.transitionCanvas.enabled = true;

        var offset = (Screen.width / 2) + transition.rect.width;
        this.transition.localPosition = Vector3.right * offset;
        this.transition.DOLocalMove(Vector3.zero, this.transitionTime).SetEase(this.transitionEaseIn);
        yield return new WaitForSeconds(this.transitionTime);

        var task = SceneManager.LoadSceneAsync((int)scene);
        while (!task.isDone)
            yield return null;

        if (scene.IsRace())
            yield return RaceManager.StartRaceSetup();

        yield return new WaitForSeconds(this.waitTime);

        this.transition.DOLocalMove(Vector3.left * offset, this.transitionTime).SetEase(this.transitionEaseOut);
        yield return new WaitForSeconds(this.transitionTime);

        if (scene.IsRace())
            RaceManager.StartRace();

        this.transitionCanvas.enabled = false;
    }

    // MUSIC

    public static void UpdateMusic(MusicTrack track, bool play = true) => _instance.music.UpdateTrack(track, play);

    public static void PlayMusic() => _instance.music.Play();

    // SFX

    public static Guid PlaySound(AudioClip clip) => _instance.sound.PlayClip(clip);
    public static IEnumerator PlaySoundAsCoroutine(AudioClip clip) => _instance.sound.PlayClipAsCoroutine(clip);
    public static void StopSound(Guid id) => _instance.sound.StopSound(id);
    public static void PlayStep() => _instance.sound.PlayStep();
}
