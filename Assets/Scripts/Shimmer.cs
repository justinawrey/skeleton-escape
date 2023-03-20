using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shimmer : MonoBehaviour
{
  private float targetIntensity;
  private Coroutine shimmerRoutine;
  public Light2D light;

  public float minIntensity, maxIntensity, speed, touchedIntensityMultiplier, maxTouchedIntensity;

  private void Start()
  {
    targetIntensity = light.intensity;
    shimmerRoutine = StartCoroutine(ShimmerRoutine());
  }

  private void OnDisable()
  {
    StopCoroutine(shimmerRoutine);
  }

  private IEnumerator ShimmerRoutine()
  {
    while (true)
    {
      // If close enough to target, pick a new one
      if (Mathf.Abs((Mathf.Abs(light.intensity) - Mathf.Abs(targetIntensity))) < 0.01)
      {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
      }

      float increment;
      if (targetIntensity - light.intensity > 0)
      {
        increment = 0.01f;
      }
      else
      {
        increment = -0.01f;
      }

      light.intensity += increment;
      yield return new WaitForSeconds(speed);
    }
  }

  private void OnTriggerEnter2D(Collider2D collider)
  {
    light.intensity = Mathf.Min(light.intensity * touchedIntensityMultiplier, maxTouchedIntensity);
  }
}
