using System.Collections;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
  private Animator animator;
  private SpriteRenderer spriteRenderer;
  private BoxCollider2D boxCollider;
  private MotionBetweenPoints motionBetweenPoints;
  private Vector3 initialPos;

  [SerializeField]
  private GameObject itemLight;

  public float respawnTime = 5f;
  public float animateInTime, animateOutTime;

  private void Start()
  {
    animator = GetComponent<Animator>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    boxCollider = GetComponent<BoxCollider2D>();
    motionBetweenPoints = GetComponent<MotionBetweenPoints>();
    initialPos = transform.position;
  }

  public float GetRespawnTime()
  {
    return respawnTime;
  }

  private IEnumerator Die()
  {
    animator.speed = 0;
    boxCollider.enabled = false;
    bool enableSprite = false;
    if (motionBetweenPoints) motionBetweenPoints.StopMovement();

    float increment = animateOutTime / 10f;
    for (float i = 0f; i < animateOutTime; i += increment)
    {
      enableSprite = !enableSprite;
      transform.position -= new Vector3(0f, increment / 8, 0f);
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1 - i);
      spriteRenderer.enabled = enableSprite;
      yield return new WaitForSeconds(increment);
    }

    itemLight.SetActive(false);
    spriteRenderer.enabled = false;
    yield return null;
  }

  private IEnumerator Respawn()
  {
    itemLight.SetActive(true);
    animator.speed = 1f;
    spriteRenderer.enabled = true;
    transform.position = initialPos;
    if (motionBetweenPoints) motionBetweenPoints.StartMovement();

    float increment = animateInTime / 10f;
    for (float i = 0f; i < animateInTime; i += increment)
    {
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, i * 2);
      yield return new WaitForSeconds(increment);
    }

    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
    boxCollider.enabled = true;
    yield return null;
  }

  public IEnumerator PeriodicallyKillRespawnableRoutine()
  {
    yield return StartCoroutine(Die());
    yield return new WaitForSeconds(GetRespawnTime());
    yield return StartCoroutine(Respawn());
    yield return null;
  }
}
