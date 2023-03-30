using UnityEngine;

public class GroundedState : BaseState
{
  public GroundedState(PlayerController ctx) : base(ctx) { }

  public override BaseState Update()
  {
    if (ctx.spaceKeyDown)
    {
      // Then jump and switch to airborn state
      ctx.Jump(ctx.jumpForce);
      return ctx.airbornState;
    }

    // Otherwise stay in grounded state
    return ctx.groundedState;
  }

  public override void FixedUpdate()
  {
    float targetSpeed;
    float groundedSpeed = ctx.horizontalAxis * ctx.maxVelocity;
    float platformSpeed = GetMovingPlatformSpeed();
    bool onPlatform = platformSpeed != 0;

    if (onPlatform)
      targetSpeed = groundedSpeed + platformSpeed;
    else
      targetSpeed = groundedSpeed;

    float accelRate = accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? ctx.runAcceleration : ctx.runDeceleration;
    float speedDiff = targetSpeed - ctx.rb.velocity.x;
    float movement = speedDiff * accelRate;
    ctx.rb.AddForce(movement * Vector2.right);

    if (!onPlatform && ctx.horizontalAxis == 0)
    {
      ApplyStoppingFriction();
    }
  }

  private float GetMovingPlatformSpeed()
  {
    Collider2D collider = ctx.GetGroundedCollider();
    if (collider && collider.gameObject.tag == "Moving Platform")
    {
      return collider.gameObject.GetComponent<MotionBetweenPoints>().GetCurrMovementVelocityX();
    }

    return 0f;
  }

  private void ApplyStoppingFriction()
  {
    float amount = Mathf.Min(Mathf.Abs(ctx.rb.velocity.x), ctx.friction);
    amount *= Mathf.Sign(ctx.rb.velocity.x);
    ctx.rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
  }
}