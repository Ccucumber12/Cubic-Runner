using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Collectable")
        {
            Destroy(col.gameObject, 0.05f);
            gameManager.appleCollected();
        }
    }
}
