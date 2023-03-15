using UnityEngine;

public class FollowCam : MonoBehaviour
{
  private Rigidbody2D rb;

  public GameObject player;
  public Vector2 offset = new Vector2(0, 2);
  public float smoothSpeed = 10f;

  private void Start()
  {
    rb = player.GetComponent<Rigidbody2D>();
  }

  private void Update()
  {
    Vector2 desiredPosition = (Vector2)player.transform.position + offset;
    float calculatedSmoothSpeed = rb.velocity.y >= -10 ? smoothSpeed : smoothSpeed * 2;
    Vector2 smoothedPosition = Vector2.Lerp(transform.position, desiredPosition, calculatedSmoothSpeed * Time.deltaTime);
    transform.position = new Vector3(transform.position.x, smoothedPosition.y, transform.position.z);
  }
}
