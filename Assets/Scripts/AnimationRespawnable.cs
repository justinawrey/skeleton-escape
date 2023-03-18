using System.Collections;
using UnityEngine;

public class AnimationRespawnable : MonoBehaviour, IRespawnable
{
  public float respawnTime = 5f;
  public Animator animator;

  public float GetRespawnTime()
  {
    return respawnTime;
  }

  public void Die()
  {
    animator.SetBool("Broken", true);
  }

  public void Respawn()
  {
    animator.SetBool("Broken", false);
  }

  public IEnumerator PeriodicallyKillRespawnableRoutine()
  {
    Die();
    yield return new WaitForSeconds(GetRespawnTime());
    Respawn();
    yield return null;
  }
}
