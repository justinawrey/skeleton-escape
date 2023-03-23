using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
  private SlowlyDie slowlyDie;
  private SpriteRenderer spriteRenderer;
  private BoxCollider2D boxCollider;
  private Animator animator;
  private Vector3 initialPos;

  [SerializeField]
  private GameObject itemLight;

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
    itemLight.SetActive(false);
    yield return null;
  }

  private IEnumerator AnimateIn()
  {
    itemLight.SetActive(true);
    animator.enabled = true;
    spriteRenderer.enabled = true;
    float increment = animateInTime / 10f;
    transform.position = initialPos;
    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

    for (float i = 0f; i < animateInTime; i += increment)
    {
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, i * 2);
      yield return new WaitForSeconds(increment);
    }

    boxCollider.enabled = true;
    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
    yield return null;
  }
}
