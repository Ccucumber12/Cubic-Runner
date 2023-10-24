using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class BloodStainTrigger : MonoBehaviour
{
    public GameObject bloodHolder;
    public GameObject bloodStain;
    public float stainProbability;

    private ParticleSystem ps;
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

    private void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Awake()
    {
        ClearBloodStain();
    }

    void OnParticleTrigger()
    {
        int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        for (int i = 0; i < numInside; i++)
        {
            ParticleSystem.Particle p = inside[i];
            if (Random.Range(0f, 1f) < stainProbability)
            {
                Instantiate(bloodStain, p.position, Quaternion.identity, bloodHolder.transform);
            }
        }


        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
    }

    [ContextMenu("Clear Blood Stain")]
    void ClearBloodStain()
    {
        for (int i = bloodHolder.transform.childCount; i > 0; --i)
            DestroyImmediate(bloodHolder.transform.GetChild(0).gameObject);
    }
}
