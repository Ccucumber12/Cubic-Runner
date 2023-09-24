using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    public GameObject explosion;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Bullet")
        {
            var obj = Instantiate(explosion, gameObject.transform);
            obj.transform.parent = null;
            obj.GetComponent<ParticleSystem>().Play();
            Destroy(col.collider.gameObject, 0.05f);
            Destroy(gameObject, 0.05f);
        }
    }
}
