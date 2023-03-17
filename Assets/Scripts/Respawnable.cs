using UnityEngine;

public interface IRespawnable
{
  public float GetRespawnTime();
  public void Die();
  public void Respawn();
}

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
}
