using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraEffects : MonoBehaviour
{
  private Rigidbody2D rb;
  public GameObject player;

  [Header("Follow cam options")]
  public Vector2 baseOffset = new Vector2(0, 1);
  public Vector2 upwardsInputOffset = new Vector2(0, 1);
  public Vector2 downwardsInputOffset = new Vector2(0, -1);
  public Vector2 leftwardsInputOffset = new Vector2(1, 1);
  public Vector2 rightwardsInputOffset = new Vector2(-1, 1);
  public Vector2 fallingOffset = new Vector2(0, -1);
  public float verticalSmoothing = 10f;
  public float horizontalSmoothing = 10f;
  [Range(0, 1)]
  public float fallSmoothingMultiplier = 0.5f;

  [Header("Screen shake options")]
  public float screenShakeDuration = 1f;
  public AnimationCurve screenShakeStrengthOverTime;

  [Header("Zoom options")]
  [Range(0, 2)]
  public float zoomMultiplier = 1f;
  public float zoomTransitionDuration = 0.5f;
  public float zoomDuration = 1f;
  private Vector3 initialScale;
  private Coroutine zooming;

  [Header("Debug options")]
  public bool printCameraOffset = false;

  private void Start()
  {
    rb = player.GetComponent<Rigidbody2D>();
    initialScale = transform.localScale;
  }

  private void Update()
  {
    ApplyFollowCam();

    if (printCameraOffset)
    {
      print("Camera offset: " + (transform.position - player.transform.position));
    }
  }

  private void ApplyFollowCam()
  {
    Vector2 desiredPosition = (Vector2)player.transform.position + CalculateOffset();
    float x = Mathf.Lerp(transform.position.x, desiredPosition.x, horizontalSmoothing * Time.deltaTime);
    float y = Mathf.Lerp(transform.position.y, desiredPosition.y, (Falling() ? verticalSmoothing * fallSmoothingMultiplier : verticalSmoothing) * Time.deltaTime);
    transform.position = new Vector3(x, y, transform.position.z);
  }

  private void ApplyZoom()
  {
    Vector3 newScale = initialScale * zoomMultiplier;
    transform.localScale = new Vector3(newScale.x, newScale.y, transform.localScale.z);
  }

  private Vector2 CalculateOffset()
  {
    Vector2 offset = baseOffset;

    // Can only offset camera either left OR right
    if (InputController.GetAxisHorz() == -1)
    {
      offset += leftwardsInputOffset;
    }
    else if (InputController.GetAxisHorz() == 1)
    {
      offset += rightwardsInputOffset;
    }

    // Can only offset camera either up OR down
    if (InputController.GetAxisVert() == -1)
    {
      offset += downwardsInputOffset;
    }
    else if (InputController.GetAxisVert() == 1)
    {
      offset += upwardsInputOffset;
    }
    else if (Falling())
    {
      offset += fallingOffset;
    }

    return offset;
  }

  public IEnumerator ShakeRoutine()
  {
    float elapsedTime = 0f;
    Vector3 initialPosition = transform.position;

    while (elapsedTime < screenShakeDuration)
    {
      elapsedTime += Time.deltaTime;
      float strength = screenShakeStrengthOverTime.Evaluate(elapsedTime / screenShakeDuration);
      transform.position = transform.position + Random.insideUnitSphere * strength;
      yield return null;
    }
  }

  private bool Falling()
  {
    return PlayerController.subState == AirbornState.FallingSubState;
  }

  public void Zoom()
  {
    // Only allow one "instance" of the zooming routine to run at once
    if (zooming != null)
    {
      StopCoroutine(zooming);
    }

    zooming = StartCoroutine(ZoomRoutine());
  }

  private IEnumerator ZoomRoutine()
  {
    Vector3 desiredScale = initialScale * zoomMultiplier;
    yield return StartCoroutine(LerpScale(desiredScale, zoomTransitionDuration));
    yield return new WaitForSeconds(zoomDuration);
    yield return StartCoroutine(LerpScale(initialScale, zoomTransitionDuration));
  }

  private IEnumerator LerpScale(Vector3 endScale, float duration)
  {
    float time = 0;
    Vector3 startScale = transform.localScale;
    while (time < duration)
    {
      float t = time / duration;
      t = t * t * (3f - (2f * t));
      transform.localScale = Vector3.Lerp(startScale, endScale, t);
      time += Time.deltaTime;
      yield return null;
    }

    transform.localScale = endScale;
  }
}
