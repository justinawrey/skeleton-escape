using System.Collections;
using UnityEngine;

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
  public float verticalSmoothing = 10f;
  public float horizontalSmoothing = 10f;

  [Header("Screen shake options")]
  public float screenShakeDuration = 1f;
  public AnimationCurve screenShakeStrengthOverTime;

  [Header("Debug options")]
  public bool printCameraOffset = false;

  private void Start()
  {
    rb = player.GetComponent<Rigidbody2D>();
  }

  private void Update()
  {
    Vector2 desiredPosition = (Vector2)player.transform.position + CalculateOffset();
    float x = Mathf.Lerp(transform.position.x, desiredPosition.x, horizontalSmoothing * Time.deltaTime);
    float y = Mathf.Lerp(transform.position.y, desiredPosition.y, verticalSmoothing * Time.deltaTime);
    transform.position = new Vector3(x, y, transform.position.z);

    if (printCameraOffset)
    {
      print("Camera offset: " + (transform.position - player.transform.position));
    }
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
}
