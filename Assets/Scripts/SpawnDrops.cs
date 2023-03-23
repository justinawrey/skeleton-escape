using UnityEngine;
using System.Collections;

public class SpawnDrops : MonoBehaviour
{
  private Coroutine spawnRoutine;

  public float minInterval, maxInterval;
  public GameObject prefab;
  public Transform spawnPos;

  private void Start()
  {
    spawnRoutine = StartCoroutine(SpawnDropsRoutine());
  }

  private void OnDisable()
  {
    StopCoroutine(spawnRoutine);
  }

  private IEnumerator SpawnDropsRoutine()
  {
    while (true)
    {
      Instantiate(prefab, spawnPos.position, Quaternion.identity);
      float interval = Random.Range(minInterval, maxInterval);
      yield return new WaitForSeconds(interval);
    }
  }
}
