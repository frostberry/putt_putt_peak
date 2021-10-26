using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState : int
{
    MainMenu,
    InGame,
    Pause,
    Win,
    About
}

[System.Serializable]
public class StatePack
{
    public int state;
    public GameObject[] objects;
}

public class StateManager : MonoBehaviour
{
    public float transitionDuration;
    public Transform transition;
    public CamManager cam;
    public BallManager ball;
    public float transitionStartX;
    public float transitionEndX;
    public StatePack[] statePacks;
    public GameObject audioSources;
    public AudioSource click;

    private GameState state;
    private GameState nextState;
    private float timer;
    private bool transitioning;
    private bool canSwap;
    private float inGameTimer;
    private bool isInGameTimer;
    private bool audioOn;

    private Dictionary<int, GameObject[]> statePacksDict;

    private void Start()
    {
        statePacksDict = new Dictionary<int, GameObject[]>();
        for (int i = 0; i < statePacks.Length; i++)
        {
            statePacksDict[statePacks[i].state] = statePacks[i].objects;
        }

        timer = 0f;
        inGameTimer = 0f;
        isInGameTimer = false;
        transitioning = false;
        canSwap = false;
        audioOn = true;

        SwapObjects();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= transitionDuration / 2f && canSwap)
        {
            SwapObjects();
            canSwap = false;
        }
        if (timer >= 0f)
        {
            transition.position = cam.gameObject.transform.position +
                Vector3.forward +
                Vector3.right * Mathf.Lerp(transitionStartX, transitionEndX, timer / transitionDuration);
        }
        else
        {
            transition.position = cam.gameObject.transform.position +
                Vector3.right * transitionStartX;
        }
        if (state == GameState.InGame)
        {
            inGameTimer += Time.deltaTime;
        }
        transitioning = timer >= 0f;
    }

    private void SwapObjects()
    {
        if (nextState == GameState.InGame)
        {
            if (state == GameState.MainMenu)
            {
                ball.Reset(Vector3.zero);
            }
            cam.SetPosition();
            cam.StartFollowing(); 
        }
        else
        {
            cam.StopFollowing();
        }

        state = nextState;
        foreach (KeyValuePair<int, GameObject[]> statePack in statePacksDict)
        {
            for (int i = 0; i < statePack.Value.Length; i++)
            {
                statePack.Value[i].SetActive((GameState)statePack.Key == state);
                if (statePack.Value[i].CompareTag("Timer"))
                {
                    statePack.Value[i].SetActive(isInGameTimer &&
                        (GameState)statePack.Key == state);
                }
            }
        }
    }

    public void SetState(int state)
    {
        nextState = (GameState)state;
        timer = transitionDuration;
        canSwap = true;
        click.Play();
    }

    public GameState GetState()
    {
        return state;
    }

    public bool GetTransitioning()
    {
        return transitioning;
    }

    public float GetInGameTimer()
    {
        return inGameTimer;
    }

    public void ResetInGameTimer()
    {
        inGameTimer = 0f;
    }

    public void ToggleTimer()
    {
        isInGameTimer = !isInGameTimer;
    }

    public bool GetIsInGameTimer()
    {
        return isInGameTimer;
    }

    public void ToggleAudioOn()
    {
        audioOn = !audioOn;
        audioSources.SetActive(audioOn);
    }

    public bool GetAudioOn()
    {
        return audioOn;
    }
}
