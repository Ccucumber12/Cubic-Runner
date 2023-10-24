using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    private static DialogueBox _instance;

    public static DialogueBox Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Dialogue Box is NULL");
            return _instance;
        }
    }

    [Range(0, 1)] public float canvasAlpha;
    public float characterDelay;
    public float displayTime;

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI tmpText;

    private string dialogueText;

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
        canvasGroup = GetComponent<CanvasGroup>();
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup.alpha = 0;
    }

    public void StartDialogue(string text)
    {
        StopAllCoroutines();
        dialogueText = text;
        StartCoroutine(DialogueAnimation());
    }

    private IEnumerator DialogueAnimation()
    {
        canvasGroup.alpha = canvasAlpha;
        tmpText.text = "";
        foreach (char c in dialogueText)
        {
            yield return new WaitForSeconds(characterDelay);
            tmpText.text += c;
        }
        yield return new WaitForSeconds(displayTime);
        canvasGroup.alpha = 0;
    }
}
