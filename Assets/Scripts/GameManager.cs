using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Game manager is NULL");
            return _instance;
        }
    }

    public int gameLength;
    public float playerDeathAnimationLength;
    public string gameScene;

    public OnGameStartEvent onGameStart;
    public OnTimerTickEvent onTimerTick;
    public OnTimeUpEvent onTimeUp;
    public OnAppleCountChangedEvent onAppleCountChanged;
    public OnPlayerDiedEvent onPlayerDied;
    public OnReachedGoalEvent onReachedGoal;

    public string currentControlScheme { get; private set; }
    public bool gamePause { get; private set; }
    public int timer { get; private set; }
    public int appleCount { get; private set; }
    public bool playerAlive { get; private set; }

    private IEnumerator timerCountDownCoroutine;

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        currentControlScheme = "Keyboard";
    }

    private void Start()
    {
        timerCountDownCoroutine = TimerCountDown();
    }

    private void Update()
    {
        Debug.Log(currentControlScheme);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene);

        timer = gameLength;
        appleCount = 0;
        gamePause = false;
        playerAlive = true;

        onGameStart.Invoke();
        StartCoroutine(timerCountDownCoroutine);
    }

    public void ReturnToEnterScene()
    {
        SceneManager.LoadScene("EnterScene");
    }

    public void AppleCollected()
    {
        appleCount++;
        onAppleCountChanged.Invoke(appleCount);
    }

    public void AppleFired()
    {
        appleCount--;
        onAppleCountChanged.Invoke(appleCount);
    }

    public void ControlsChanged(string newControlScheme)
    {
        currentControlScheme = newControlScheme;
    }

    private IEnumerator TimerCountDown()
    {
        yield return new WaitForSeconds(1);
        while (timer > 0)
        {
            if (!gamePause)
            {
                timer -= 1;
                TimerTick();
            }
            yield return new WaitForSeconds(1);
        }
        TimeUp();
    }

    private void TimerTick()
    {
        onTimerTick.Invoke();
    }

    private void TimeUp()
    {
        playerAlive = false;
        gamePause = true;

        onTimeUp.Invoke();
        StartCoroutine(LoadEndScene(playerDeathAnimationLength));
    }

    private IEnumerator LoadEndScene(float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("EndScene");
    }

    public void Restart(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            SceneManager.LoadScene("EnterScene");
        }
    }

    public void GamePause()
    {
        gamePause = true;
    }

    public void PlayerDied()
    {
        playerAlive = false;
        gamePause = true;
        StopCoroutine(timerCountDownCoroutine);
        StartCoroutine(LoadEndScene(playerDeathAnimationLength));
        onPlayerDied.Invoke();
    }
    public void ReachedGoal()
    {
        gamePause = true;
        StopCoroutine(timerCountDownCoroutine);
        SceneManager.LoadScene("EndScene");
    }

    public void OnControlChanged(PlayerInput input)
    {
        ControlsChanged(input.currentControlScheme);
    }


    [System.Serializable]
    public class OnGameStartEvent : UnityEvent { }

    [System.Serializable]
    public class OnTimerTickEvent : UnityEvent { }

    [System.Serializable]
    public class OnAppleCountChangedEvent : UnityEvent<int> { }

    [System.Serializable]
    public class OnTimeUpEvent : UnityEvent { }

    [System.Serializable]
    public class OnPlayerDiedEvent : UnityEvent { }

    [System.Serializable]
    public class OnReachedGoalEvent : UnityEvent { }
}
