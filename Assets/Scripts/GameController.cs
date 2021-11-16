using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    private PlayerSelection[] players;
    public static PlayerSelection[] Players { get => _instance.players; }

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
    }

    public static void SetPlayersAndPlay(PlayerSelection[] players, Scenes level)
    {
        _instance.players = players;
        SceneManager.LoadScene((int)level);
    }
}
