using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
  private Vector2 input;
  private float lastOnGroundTime = 0f;
  private bool groundCheckEnabled = true;

  public Rigidbody2D rb;
  public Transform groundCheck;
  public LayerMask groundLayer;

  public float maxVelocity = 2f;
  public float runAcceleration = 1.5f;
  public float runDeceleration = 1.5f;
  public float airAcceleration = 1.2f;
  public float airDeceleration = 1.2f;
  public float friction = 0.2f;
  public float gravityScale = 1f;
  public float fallGravityMultiplier = 1.5f;
  public float jumpForce = 1f;
  public float jumpCutMultiplier = 0.1f;
  public float coyoteTime = 1f;

  private void Update()
  {
    lastOnGroundTime -= Time.deltaTime;

    if (groundCheckEnabled && Grounded())
    {
      lastOnGroundTime = coyoteTime;
    }

    input.x = Input.GetAxisRaw("Horizontal");
    input.y = Input.GetAxisRaw("Vertical");

    if (lastOnGroundTime > 0 && Input.GetKeyDown(KeyCode.Space))
    {
      lastOnGroundTime = 0;
      groundCheckEnabled = false;

      rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
      StartCoroutine(GroundCheckDelayRoutine());
    }

    if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
    {
      rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
    }

    // Extra fall gravity
    if (rb.velocity.y < 0)
    {
      rb.gravityScale = gravityScale * fallGravityMultiplier;
    }
    else
    {
      rb.gravityScale = gravityScale;
    }
  }

  private void FixedUpdate()
  {
    // Calculate the direction we want to move in and our desired velocity
    float targetSpeed = input.x * maxVelocity;

    // Gets an acceleration value based on if we are accelerating (includes turning) 
    // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
    float accelRate;
    if (Grounded())
      accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDeceleration;
    else
      accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * airAcceleration : runDeceleration * airDeceleration;

    // Calculate difference between current velocity and desired velocity
    float speedDiff = targetSpeed - rb.velocity.x;
    float movement = speedDiff * accelRate;

    // Convert this to a vector and apply to rigidbody
    rb.AddForce(movement * Vector2.right);

    // Add stopping friction
    if (Grounded() && input.x == 0)
    {
      float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), friction);
      amount *= Mathf.Sign(rb.velocity.x);
      rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    }
  }

  private bool Grounded()
  {
    return Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);
  }

  private IEnumerator GroundCheckDelayRoutine()
  {
    yield return new WaitForSeconds(0.2f);
    groundCheckEnabled = true;
    yield return null;
  }
}

