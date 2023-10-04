using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    public string gamepadMessage;
    public string keyboardMessage;
    public float triggerCoolDown;
    public Transform spriteTransform;
    public ParticleSystem burstParticle;

    [Header("Trigger Enter Shake")]
    public float shakeAnimationLength;
    public float shakeDegree;
    public float shakeLoopDuration;

    private GameManager gameManager;
    private DialogueBox dialogueBox;
    private float lastTriggerTime;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        gameManager = GameManager.Instance;
        dialogueBox = DialogueBox.Instance;
        lastTriggerTime = -triggerCoolDown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (shakeCoroutine != null)
            {
                spriteTransform.localRotation = Quaternion.identity;
                StopCoroutine(shakeCoroutine);
            }
            shakeCoroutine = StartCoroutine(ShakeAnimation());
            if (Time.time - lastTriggerTime < triggerCoolDown)
                return;

            lastTriggerTime = Time.time;
            switch(gameManager.currentControlScheme)
            {
                case "Gamepad":
                    dialogueBox.StartDialogue(gamepadMessage);
                    break;
                case "Keyboard":
                    dialogueBox.StartDialogue(keyboardMessage);
                    break;
                default:
                    Debug.LogError("Cannot recognize control scheme.");
                    break;
            }
        }
    }

    private IEnumerator ShakeAnimation()
    {
        Vector3 rotation = spriteTransform.localEulerAngles;
        float startTime = Time.time;
        burstParticle.Play();

        while(Time.time - startTime <= shakeAnimationLength)
        {
            rotation.z = shakeDegree * Mathf.Sin(Time.time / shakeLoopDuration * 2 * Mathf.PI);
            spriteTransform.localEulerAngles = rotation;
            yield return 0;
        }
        spriteTransform.localRotation = Quaternion.identity;
    }
}
