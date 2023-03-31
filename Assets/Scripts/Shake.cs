
using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
  public float duration = 1f;
  public AnimationCurve animationCurve;

  public IEnumerator ShakeRoutine()
  {
    // Vector3 startPos = transform.position;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      float strength = animationCurve.Evaluate(elapsedTime / duration);
      transform.position = transform.position + Random.insideUnitSphere * strength;
      yield return null;
    }

    // transform.position = startPos;
  }
}