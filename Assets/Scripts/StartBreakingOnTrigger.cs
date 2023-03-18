using UnityEngine;

public class StartBreakingOnTrigger : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.name == "Player")
    {
      AnimationRespawnable respawnable = GetComponent<AnimationRespawnable>();
      StartCoroutine(respawnable.PeriodicallyKillRespawnableRoutine());
    }
  }
}
