using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shimmer : MonoBehaviour
{
  private Coroutine shimmerRoutine;
  public Light2D light;

  public float minIntensity, maxIntensity, minInterval, maxInterval;

  private void Start()
  {
    shimmerRoutine = StartCoroutine(ShimmerRoutine());
  }

  private void OnDisable()
  {
    StopCoroutine(shimmerRoutine);
  }

  private IEnumerator ShimmerRoutine()
  {
    float interval = Random.Range(minInterval, maxInterval);
    float intensity = Random.Range(minIntensity, maxIntensity);
  }
}
