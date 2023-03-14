using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private Vector2 input;
  private bool jumpInputDown;
  private bool jumpInputUp;
  private bool jumping = false;

  public Rigidbody2D rb;
  public GameObject groundCheck;
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
  public float jumpHangTimeThreshold = 0.1f;
  public float jumpApexAccelerationMultiplier = 1.1f;
  public float jumpApexMaxVelocityMultiplier = 1.1f;

  private void Update()
  {
    input.x = Input.GetAxisRaw("Horizontal");
    input.y = Input.GetAxisRaw("Vertical");
    jumpInputDown = Input.GetKeyDown(KeyCode.Space);
    jumpInputUp = Input.GetKeyUp(KeyCode.Space);
  }

  private void FixedUpdate()
  {
    if (Grounded())
    {
      jumping = false;
    }

    // Calculate the direction we want to move in and our desired velocity
    float targetSpeed = input.x * maxVelocity;

    // Gets an acceleration value based on if we are accelerating (includes turning) 
    // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
    float accelRate;
    if (Grounded())
      accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDeceleration;
    else
      accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * airAcceleration : runDeceleration * airDeceleration;

    // Increase are acceleration and maxSpeed when at the apex of their jump,
    // makes the jump feel a bit more bouncy, responsive and natural
    if (jumping && (Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold))
    {
      accelRate *= jumpApexAccelerationMultiplier;
      targetSpeed *= jumpApexMaxVelocityMultiplier;
    }

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

    // Do jump
    if (jumpInputDown)
    {
      Jump();
    }

    // Jump cut
    if (jumpInputUp)
    {
      if (rb.velocity.y > 0 && jumping)
      {
        rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
      }
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

  private bool Grounded()
  {
    return Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);
  }

  private bool Jumping()
  {
    return false;
  }

  private void Jump()
  {
    jumping = true;
    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
  }
}

