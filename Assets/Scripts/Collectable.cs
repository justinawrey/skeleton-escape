using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
  private SlowlyDie slowlyDie;
  private SpriteRenderer spriteRenderer;
  private BoxCollider2D boxCollider;
  private Animator animator;
  private Vector3 initialPos;

  public float fullnessAmt;
  public float respawnTime = 2.5f;
  public float animateInTime, animateOutTime;

  private void Start()
  {
    slowlyDie = GameObject.Find("Fullness").GetComponent<SlowlyDie>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    boxCollider = GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();
    initialPos = transform.position;
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
    yield return StartCoroutine(AnimateOut());
    yield return new WaitForSeconds(respawnTime);
    yield return StartCoroutine(AnimateIn());
  }

  private IEnumerator AnimateOut()
  {
    animator.enabled = false;
    boxCollider.enabled = false;
    float increment = animateOutTime / 10f;

    for (float i = 0f; i < animateOutTime; i += increment)
    {
      transform.position += new Vector3(0f, increment / 8, 0f);
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1 - i);
      yield return new WaitForSeconds(increment);
    }

    spriteRenderer.enabled = false;
    transform.position = initialPos;
    yield return null;
  }

  private IEnumerator AnimateIn()
  {
    spriteRenderer.enabled = true;
    float increment = animateInTime / 10f;
    transform.position = initialPos + (new Vector3(0, animateInTime / 10, 0));
    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

    for (float i = 0f; i < animateInTime; i += increment)
    {
      transform.position -= new Vector3(0f, increment / 8, 0f);
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, i * 2);
      yield return new WaitForSeconds(increment);
    }

    animator.enabled = true;
    boxCollider.enabled = true;
    transform.position = initialPos;
    yield return null;
  }
}
