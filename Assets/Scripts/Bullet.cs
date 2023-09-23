using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float lifespan;
    public float torque;
    public float collisionDelayTime;

    [HideInInspector] public int direction;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        torque *= direction;
        speed *= direction;

        rb.velocity = new Vector2(speed, 0);
        rb.AddTorque(torque);

        StartCoroutine(DelayedDisableIsTrigger());
        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
    }

    private IEnumerator DelayedDisableIsTrigger()
    {
        yield return new WaitForSeconds(collisionDelayTime);
        col.isTrigger = false;
    }
}
