using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class EnterManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float floatingDistance;
    public float floatingLoopLength;
    
    private Vector3 hintTextAnchorPoint;
    private GameManager gameManager;

    private bool firstSchemeChange;

    void Start()
    {
        gameManager = GameManager.Instance;
        hintTextAnchorPoint = hintText.transform.position;

        UpdateHintMessage();
    }

    void Update()
    {
        Vector3 pos = hintTextAnchorPoint;
        pos.y += floatingDistance * Mathf.Sin(Time.fixedTime * Mathf.PI * 2 / floatingLoopLength) / 2;
        hintText.transform.position = pos;
    }

    private void UpdateHintMessage()
    {
        string scheme = gameManager.currentControlScheme;
        if (scheme == "Gamepad")
        {
            hintText.text = "Press button A to start...";
        }
        else if (scheme == "Keyboard")
        {
            hintText.text = "Press space to start...";
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

    public void OnGameStart(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            gameManager.StartGame();
        }
    }
}
