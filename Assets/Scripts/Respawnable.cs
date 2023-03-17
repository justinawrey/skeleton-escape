using System.Collections;
using UnityEngine;

public class Respawnable : MonoBehaviour, IRespawnable
{
  public float respawnTime = 5f;

  public float GetRespawnTime()
  {
    return respawnTime;
  }

  public void Die()
  {
    gameObject.SetActive(false);
  }

  public void Respawn()
  {
    gameObject.SetActive(true);
  }

  public IEnumerator PeriodicallyKillRespawnableRoutine()
  {
    Die();
    yield return new WaitForSeconds(GetRespawnTime());
    Respawn();
    yield return null;
  }
}
