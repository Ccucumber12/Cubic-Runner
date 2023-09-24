using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public float lineDelay;
    public float floatingDistance;
    public float floatingLoopLength;

    [Header("References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI appleCountText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hintText;
    public Image line;

    GameManager gameManager;
    private Vector3 hintTextAnchorPoint;


    void Start()
    {
        gameManager = GameManager.Instance;

        timeText.text = timeText.text.Substring(0, timeText.text.Length - 3) + gameManager.timer.ToString("D3");
        appleCountText.text = appleCountText.text.Substring(0, appleCountText.text.Length - 3) + gameManager.appleCount.ToString("D3");
        int score = gameManager.timer + gameManager.appleCount;
        scoreText.text = scoreText.text.Substring(0, scoreText.text.Length - 3) + score.ToString("D3");

        timeText.alpha = 0;
        appleCountText.alpha = 0;
        scoreText.alpha = 0;
        hintText.alpha = 0;

        Color color = line.color;
        color.a = 0;
        line.color = color;

        StartCoroutine(DisplayAnimation());

        hintTextAnchorPoint = hintText.transform.position;
    }

    void Update()
    {
        Vector3 pos = hintTextAnchorPoint;
        pos.y += floatingDistance * Mathf.Sin(Time.fixedTime * Mathf.PI * 2 / floatingLoopLength) / 2;
        hintText.transform.position = pos;
    }

    private IEnumerator DisplayAnimation()
    {
        yield return new WaitForSeconds(lineDelay);
        timeText.alpha = 1;
        yield return new WaitForSeconds(lineDelay);
        appleCountText.alpha = 1;
        yield return new WaitForSeconds (lineDelay);
        
        Color color = line.color;
        color.a = 1;
        line.color = color;

        yield return new WaitForSeconds(lineDelay);
        scoreText.alpha = 1;
        yield return new WaitForSeconds(lineDelay);
        hintText.alpha = 1;
    }

    public void OnControlChanged(PlayerInput input)
    {
        string scheme = input.currentControlScheme;
        if (scheme == "Gamepad")
        {
            hintText.text = "Press start to return...";
        }
        else if (scheme == "Keyboard")
        {
            hintText.text = "Press R to return...";
        }
    }

    public void OnReturnToEnterScene(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            SceneManager.LoadScene("EnterScene");
        }
    }
}
