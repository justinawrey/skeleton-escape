using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
  private SlowlyDie slowlyDie;
  private SpriteRenderer spriteRenderer;
  private BoxCollider2D boxCollider;
  private Animator animator;
  private Vector3 initialPos;
  private bool bobbing = true;
  private bool bobbingUp = true;
  private float bobCounter = 0;

  [SerializeField]
  private GameObject itemLight;

  public float fullnessAmt;
  public float respawnTime = 2.5f;
  public float animateInTime, animateOutTime;
  public float bobAmount = 0.1f;
  public float bobTime = 0.5f;

  private CameraEffects cameraEffects;

  private void Start()
  {
    slowlyDie = GameObject.Find("Fullness").GetComponent<SlowlyDie>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    boxCollider = GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();
    cameraEffects = Camera.main.GetComponent<CameraEffects>();
    initialPos = transform.position;

    float offset = Random.Range(-0.09f, 0.09f);
    transform.position = transform.position + new Vector3(0f, offset, 0f);
  }

  private void Update()
  {
    bobCounter += Time.deltaTime;

    if (!bobbing)
    {
      return;
    }

    if (bobCounter < bobTime)
    {
      return;
    }

    if (transform.position.y >= initialPos.y + bobAmount)
    {
      bobbingUp = false;
    }

    if (transform.position.y <= initialPos.y - bobAmount)
    {
      bobbingUp = true;
    }

    if (bobbingUp)
    {
      transform.position += new Vector3(0, 0.03f, 0);
    }
    else
    {
      transform.position -= new Vector3(0, 0.03f, 0);
    }

    bobCounter = 0;
  }

  private void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.name == "Player")
    {
      slowlyDie.AddToFullness(fullnessAmt);
      StartCoroutine(RespawnRoutine());
      cameraEffects.Zoom();
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
    bobbing = false;
    animator.Play("Chomping");
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
    transform.position = initialPos;
    bobbing = true;
    itemLight.SetActive(true);
    animator.Play("Idle");
    spriteRenderer.enabled = true;
    float increment = animateInTime / 10f;
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
