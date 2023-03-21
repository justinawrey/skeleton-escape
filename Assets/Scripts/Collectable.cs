using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
  private SlowlyDie slowlyDie;
  private SpriteRenderer spriteRenderer;
  private BoxCollider2D boxCollider;

  public float fullnessAmt;
  public float respawnTime = 2.5f;

  private void Start()
  {
    slowlyDie = GameObject.Find("Fullness").GetComponent<SlowlyDie>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    boxCollider = GetComponent<BoxCollider2D>();
  }

  private void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.name == "Player")
    {
      slowlyDie.AddToFullness(fullnessAmt);
      StartCoroutine(RespawnRoutine());
    }
  }

  private IEnumerator RespawnRoutine()
  {
    spriteRenderer.enabled = false;
    boxCollider.enabled = false;
    yield return new WaitForSeconds(respawnTime);
    spriteRenderer.enabled = true;
    boxCollider.enabled = true;
  }
}
