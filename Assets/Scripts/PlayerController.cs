using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
  private Vector2 input;
  private float coyoteTimeCounter = 0f;
  private float jumpBufferCounter = 0f;
  private float jumpTimingBufferCounter = 0f;
  private bool groundCheckEnabled = true;
  private bool inIFrames = false;

  public Rigidbody2D rb;
  public SpriteRenderer spriteRenderer;
  public BoxCollider2D boxCollider;
  public Transform groundCheck;
  public LayerMask groundLayer;
  public LayerMask bounceLayer;

  public float maxVelocity = 2f;
  public float runAcceleration = 1.5f;
  public float runDeceleration = 1.5f;
  public float airAcceleration = 1.2f;
  public float airDeceleration = 1.2f;
  public float friction = 0.2f;
  public float gravityScale = 1f;
  public float fallGravityMultiplier = 1.5f;
  public float jumpForce = 1f;
  public float jumpOffEnemyForce = 1f;
  public float jumpOffEnemyWithTimingForce = 1f;
  public float jumpCutMultiplier = 0.1f;
  public float coyoteTime = 0.15f;
  public float jumpBuffer = 0.2f;
  public float jumpTimingBuffer = 0.3f;
  public float postBounceTimingBuffer = 0.1f;
  public float iFramesGravityMultiplier = 0.5f;
  public float iFramesLength = 1.5f;
  public float iFrameRecoilMultiplier = 1.2f;

  private void Update()
  {
    coyoteTimeCounter -= Time.deltaTime;

    if (groundCheckEnabled && Grounded())
    {
      coyoteTimeCounter = coyoteTime;
    }

    input.x = InputController.GetAxisHorz();
    input.y = InputController.GetAxisVert();

    if (input.x > 0)
    {
      spriteRenderer.flipX = false;
    }
    else if (input.x < 0)
    {
      spriteRenderer.flipX = true;
    }

    if (InputController.GetSpaceDown())
    {
      jumpBufferCounter = jumpBuffer;
      jumpTimingBufferCounter = jumpTimingBuffer;
    }
    else
    {
      jumpBufferCounter -= Time.deltaTime;
      jumpTimingBufferCounter -= Time.deltaTime;
    }

    if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
    {
      coyoteTimeCounter = 0;
      jumpBufferCounter = 0;
      groundCheckEnabled = false;

      Jump(jumpForce);
      StartCoroutine(GroundCheckDelayRoutine());
    }

    if (InputController.GetSpaceUp() && rb.velocity.y > 0)
    {
      rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
    }

    if (!inIFrames)
    {
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
  }

  private void FixedUpdate()
  {
    // Calculate the direction we want to move in and our desired velocity
    float targetSpeed;
    float platformSpeed = GetMovingPlatformSpeed();
    bool onPlatform = platformSpeed != 0;
    float groundedSpeed = input.x * maxVelocity;

    if (onPlatform)
      targetSpeed = groundedSpeed + platformSpeed;
    else
      targetSpeed = groundedSpeed;

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

    // Add stopping friction ONLY if not on moving platform
    if (Grounded() && !onPlatform && input.x == 0)
    {
      float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), friction);
      amount *= Mathf.Sign(rb.velocity.x);
      rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    }
  }

  private bool Grounded()
  {
    return GetGroundedCollider();
  }

  private Collider2D GetGroundedCollider()
  {
    return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.2f, 0.03f), CapsuleDirection2D.Horizontal, 0, groundLayer);
  }

  private float GetMovingPlatformSpeed()
  {
    Collider2D collider = GetGroundedCollider();
    if (collider && collider.gameObject.tag == "Moving Platform")
    {
      return collider.gameObject.GetComponent<MotionBetweenPoints>().GetCurrMovementVelocityX();
    }

    return 0f;
  }

  private IEnumerator GroundCheckDelayRoutine()
  {
    yield return new WaitForSeconds(0.2f);
    groundCheckEnabled = true;
    yield return null;
  }

  private void Jump(float force)
  {
    rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.tag == "Deals Damage")
    {
      Collider2D footCollider = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, bounceLayer);

      // Then we can jump off of it
      if (footCollider)
      {
        rb.velocity = new Vector2(rb.velocity.x, 0);

        if (jumpTimingBufferCounter > 0)
        {
          Jump(jumpOffEnemyWithTimingForce);
          jumpTimingBufferCounter = 0;
        }
        else
        {
          Jump(jumpOffEnemyForce);
        }

        Respawnable respawnable = other.gameObject.GetComponent<Respawnable>();
        StartCoroutine(respawnable.PeriodicallyKillRespawnableRoutine());
      }
      // Otherwise take damage :(
      else
      {
        StartCoroutine(IFramesRoutine());
      }
    }
  }

  private IEnumerator IFramesRoutine()
  {
    InputController.inputEnabled = false;
    boxCollider.enabled = false;
    inIFrames = true;
    rb.gravityScale = gravityScale * iFramesGravityMultiplier;
    Vector2 currVelocity = rb.velocity;
    rb.velocity = new Vector2(0, 0);
    rb.AddForce(new Vector2(-currVelocity.x, -currVelocity.y).normalized * iFrameRecoilMultiplier, ForceMode2D.Impulse);

    float interval = iFramesLength / 5;
    for (float i = 0; i <= iFramesLength; i += interval)
    {
      spriteRenderer.enabled = !spriteRenderer.enabled;
      yield return new WaitForSeconds(interval);
    }

    inIFrames = false;
    spriteRenderer.enabled = true;
    InputController.inputEnabled = true;
    boxCollider.enabled = true;
    yield return null;
  }
}

