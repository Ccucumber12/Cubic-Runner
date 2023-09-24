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

    public OnGameStartEvent onGameStart;
    public OnTimerTickEvent onTimerTick;
    public OnTimeUpEvent onTimeUp;
    public OnAppleCountChangedEvent onAppleCountChanged;

    public int timer { get; private set; }
    public int appleCount { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        timer = gameLength;
        appleCount = 0;
        onGameStart.Invoke();
        StartCoroutine(timerCountDown());
    }

    public void appleCollected()
    {
        appleCount++;
        onAppleCountChanged.Invoke(appleCount);
    }

    public void appleFired()
    {
        appleCount--;
        onAppleCountChanged.Invoke(appleCount);
    }

    private IEnumerator timerCountDown()
    {
        yield return new WaitForSeconds(1);
        while (timer > 0)
        {
            timer -= 1;
            TimerTick();
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
        onTimeUp.Invoke();
        SceneManager.LoadScene("EndScene");
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            SceneManager.LoadScene("EnterScene");
        }
    }

    [System.Serializable]
    public class OnGameStartEvent : UnityEvent { }

    [System.Serializable]
    public class OnTimerTickEvent : UnityEvent { }

    [System.Serializable]
    public class OnTimeUpEvent : UnityEvent { }

    [System.Serializable]
    public class OnAppleCountChangedEvent : UnityEvent<int> { }
}
