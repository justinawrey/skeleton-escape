using System.Collections;
using UnityEngine;

public class AnimationRespawnable : MonoBehaviour
{
  private Animator animator;
  private BoxCollider2D boxCollider;
  private CapsuleCollider2D capsuleCollider;
  private SpriteRenderer spriteRenderer;

  [SerializeField]
  private GameObject itemLight;

  public float respawnTime = 5f;
  public float animateInTime = 1f;

  private void Start()
  {
    animator = GetComponent<Animator>();
    boxCollider = GetComponent<BoxCollider2D>();
    capsuleCollider = GetComponent<CapsuleCollider2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public float GetRespawnTime()
  {
    return respawnTime;
  }

  public void Die()
  {
    animator.Play("Breaking");
    capsuleCollider.enabled = false;
  }

  public void DieAnimationEnd()
  {
    itemLight.SetActive(false);
    spriteRenderer.enabled = false;
  }

  public void PlatformBroken()
  {
    boxCollider.enabled = false;
  }

  private IEnumerator Respawn()
  {
    animator.Play("Idle");
    itemLight.SetActive(true);
    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
    spriteRenderer.enabled = true;

    float increment = animateInTime / 10f;
    for (float i = 0f; i < animateInTime; i += increment)
    {
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, i * 2);
      yield return new WaitForSeconds(increment);
    }

    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
    boxCollider.enabled = true;
    capsuleCollider.enabled = true;
    yield return null;
  }

  public IEnumerator PeriodicallyKillRespawnableRoutine()
  {
    Die();
    yield return new WaitForSeconds(GetRespawnTime());
    yield return StartCoroutine(Respawn());
    yield return null;
  }
}
