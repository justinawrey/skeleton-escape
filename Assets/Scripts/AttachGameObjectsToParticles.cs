using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ParticleSystem))]
public class AttachGameObjectsToParticles : MonoBehaviour
{
  public GameObject m_Prefab;
  public float maxIntensity, fadeInPeriod;

  private ParticleSystem m_ParticleSystem;
  private List<GameObject> m_Instances = new List<GameObject>();
  private ParticleSystem.Particle[] m_Particles;

  // Start is called before the first frame update
  void Start()
  {
    m_ParticleSystem = GetComponent<ParticleSystem>();
    m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
  }

  // Update is called once per frame
  void LateUpdate()
  {
    int count = m_ParticleSystem.GetParticles(m_Particles);

    while (m_Instances.Count < count)
      m_Instances.Add(Instantiate(m_Prefab, m_ParticleSystem.transform));

    bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
    for (int i = 0; i < m_Instances.Count; i++)
    {
      if (i < count)
      {
        if (worldSpace)
          m_Instances[i].transform.position = m_Particles[i].position;
        else
          m_Instances[i].transform.localPosition = m_Particles[i].position;
        SetIntensityBasedOnParticleLifetime(m_Instances[i], m_Particles[i]);
        m_Instances[i].gameObject.SetActive(true);
      }
      else
      {
        m_Instances[i].gameObject.SetActive(false);
      }
    }
  }

  private void SetIntensityBasedOnParticleLifetime(GameObject instance, ParticleSystem.Particle particle)
  {
    Light2D light = instance.GetComponent<Light2D>();
    float timeElapsed = particle.startLifetime - particle.remainingLifetime;
    bool fadeIn = timeElapsed < fadeInPeriod;

    if (fadeIn)
    {
      light.intensity = Mathf.Min(timeElapsed, maxIntensity);
    }
    else
    {
      light.intensity = Mathf.Min(particle.remainingLifetime, maxIntensity);
    }
  }
}
