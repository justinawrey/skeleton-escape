using System.Collections;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
  public Transform playerCheck;
  public LayerMask playerLayer;
  public float timer = 1f;

  private void FixedUpdate()
  {
    Collider2D collider = Physics2D.OverlapBox(playerCheck.position, new Vector2(1, 0.15f), 0, playerLayer);
    if (collider)
    {
      StartCoroutine(DisableAfterTimeRoutine());
    }
  }

  private IEnumerator DisableAfterTimeRoutine()
  {
    yield return new WaitForSeconds(timer);
    Respawnable respawnable = GetComponent<Respawnable>();
    StartCoroutine(respawnable.PeriodicallyKillRespawnableRoutine());
  }
}
