using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class EndManager : MonoBehaviour
{
    public float scoreLineDelay;
    public float failedCharDelay;
    public float floatingDistance;
    public float floatingLoopLength;

    [Header("References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI appleCountText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hintText;
    public Image line;
    public TextMeshProUGUI failedText;

    GameManager gameManager;
    private Vector3 hintTextAnchorPoint;
    private bool firstSchemeChange;
    private bool isAnimationFinished;
    private string failedString;


    void Start()
    {
        gameManager = GameManager.Instance;

        timeText.alpha = 0;
        appleCountText.alpha = 0;
        scoreText.alpha = 0;
        hintText.alpha = 0;
        Color color = line.color;
        color.a = 0;
        line.color = color;

        failedText.alpha = 0;
        failedString = failedText.text;

        if (gameManager.playerAlive)
            StartCoroutine(SuccessAnimation());
        else
            StartCoroutine(FailureAnimation());

        hintTextAnchorPoint = hintText.transform.position;

        UpdateHintMessage();
    }

    void Update()
    {
        Vector3 pos = hintTextAnchorPoint;
        pos.y += floatingDistance * Mathf.Sin(Time.fixedTime * Mathf.PI * 2 / floatingLoopLength) / 2;
        hintText.transform.position = pos;
    }

    private IEnumerator FailureAnimation()
    {
        failedText.text = "";
        failedText.alpha = 1;

        foreach (char c in failedString)
        {
            yield return new WaitForSeconds(failedCharDelay);
            failedText.text += c;
        }
        yield return new WaitForSeconds(scoreLineDelay);
        hintText.alpha = 1;

        isAnimationFinished = true;
    }

    private IEnumerator SuccessAnimation()
    {
        timeText.text = timeText.text.Substring(0, timeText.text.Length - 3) + gameManager.timer.ToString("D3");
        appleCountText.text = appleCountText.text.Substring(0, appleCountText.text.Length - 3) + gameManager.appleCount.ToString("D3");
        int score = gameManager.timer + gameManager.appleCount;
        scoreText.text = scoreText.text.Substring(0, scoreText.text.Length - 3) + score.ToString("D3");

        yield return new WaitForSeconds(scoreLineDelay);
        timeText.alpha = 1;
        yield return new WaitForSeconds(scoreLineDelay);
        appleCountText.alpha = 1;
        yield return new WaitForSeconds (scoreLineDelay);

        Color color = line.color;
        color.a = 1;
        line.color = color;

        yield return new WaitForSeconds(scoreLineDelay);
        scoreText.alpha = 1;
        yield return new WaitForSeconds(scoreLineDelay);
        hintText.alpha = 1;

        isAnimationFinished = true;
    }

    private void UpdateHintMessage()
    {
        string scheme = gameManager.currentControlScheme;
        if (scheme == "Gamepad")
        {
            hintText.text = "Press button A to return...";
        }
        else if (scheme == "Keyboard")
        {
            hintText.text = "Press space to return...";
        }
    }

    public void OnControlChanged(PlayerInput input)
    {
        if (!firstSchemeChange)
        {
            // load scene will automatically change scheme to keyboard, thus ignore the first scheme change.
            firstSchemeChange = true;
            return;
        }
        gameManager.ControlsChanged(input.currentControlScheme);
        UpdateHintMessage();
    }

    public void OnReturnToEnterScene(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (!isAnimationFinished)
            {
                StopAllCoroutines();
                if (gameManager.playerAlive)
                {
                    timeText.text = timeText.text.Substring(0, timeText.text.Length - 3) + gameManager.timer.ToString("D3");
                    appleCountText.text = appleCountText.text.Substring(0, appleCountText.text.Length - 3) + gameManager.appleCount.ToString("D3");
                    int score = gameManager.timer + gameManager.appleCount;
                    scoreText.text = scoreText.text.Substring(0, scoreText.text.Length - 3) + score.ToString("D3");

                    timeText.alpha = 1;
                    appleCountText.alpha = 1;
                    scoreText.alpha = 1;

                    Color color = line.color;
                    color.a = 1;
                    line.color = color;
                }
                else
                {
                    failedText.text = failedString;
                    failedText.alpha = 1;
                }
                hintText.alpha = 1;
                isAnimationFinished = true;
            }
            else
            {
                gameManager.ReturnToEnterScene();
            }
        }
    }
}
