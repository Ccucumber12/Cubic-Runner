using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnterManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float floatingDistance;
    public float floatingLoopLength;
    
    private Vector3 hintTextAnchorPoint;

    void Start()
    {
        hintTextAnchorPoint = hintText.transform.position;
    }

    void Update()
    {
        Vector3 pos = hintTextAnchorPoint;
        pos.y += floatingDistance * Mathf.Sin(Time.fixedTime * Mathf.PI * 2 / floatingLoopLength) / 2;
        hintText.transform.position = pos;
    }

    public void OnControlChanged(PlayerInput input)
    {
        string scheme = input.currentControlScheme;
        if (scheme == "Gamepad")
        {
            hintText.text = "Press A to start...";
        }
        else if (scheme == "Keyboard")
        {
            hintText.text = "Press space to start...";
        }
    }

    public void OnGameStart(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            SceneManager.LoadScene("DevelopScene");
        }
    }
}
