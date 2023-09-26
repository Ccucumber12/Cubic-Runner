using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasControl : MonoBehaviour
{
    public TextMeshProUGUI appleCountText;
    public TextMeshProUGUI timeText;

    private string timeTextPrefix = "TIME: ";
    private string appleCountTextPrefix = ": ";

    private GameManager gameManager;

    
    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.onTimerTick.AddListener(UpdateTime);
        gameManager.onAppleCountChanged.AddListener(UpdateAppleCount);

        appleCountText.text = appleCountTextPrefix + "000";
        timeText.text = timeTextPrefix + gameManager.gameLength.ToString("D3");
    }

    private void OnDestroy()
    {
        gameManager.onTimerTick.RemoveListener(UpdateTime);
        gameManager.onAppleCountChanged.RemoveListener(UpdateAppleCount);
    }

    public void UpdateAppleCount(int appleCount)
    {
        appleCountText.text = appleCountTextPrefix + appleCount.ToString("D3");
    }

    public void UpdateTime()
    {
        timeText.text = timeTextPrefix + gameManager.timer.ToString("D3");
    }

    void Update()
    {
        
    }
}
