using UnityEngine;

public class AirbornState : BaseState
{
  public AirbornState(PlayerController context) : base(context) { }

  public static string FallingSubState = "Airborn.Falling";

  public override BaseState Update()
  {
    // We ran off a ledge but still tried to jump in coyote time
    if (ctx.spaceKeyDown && ctx.coyoteTime.Valid())
    {
      ctx.Jump(ctx.jumpForce);
    }

    // Used for follow cam
    if (ctx.rb.velocity.y < 0)
    {
      ctx.SetSubstate(FallingSubState);
    }

    ApplyJumpCut();
    ApplyFallGravity();
    return null;
  }

  public override void FixedUpdate()
  {
    float targetSpeed = ctx.horizontalAxis * ctx.maxVelocity;
    float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? ctx.runAcceleration * ctx.airAcceleration : ctx.runDeceleration * ctx.airDeceleration;
    float speedDiff = targetSpeed - ctx.rb.velocity.x;
    float movement = speedDiff * accelRate;
    ctx.rb.AddForce(movement * Vector2.right);

    LimitFallVelocity();
  }

  private void LimitFallVelocity()
  {
    if (ctx.rb.velocity.y < ctx.maxFallVelocity)
    {
      ctx.rb.velocity = new Vector2(ctx.rb.velocity.x, ctx.maxFallVelocity);
    }
  }

  private void ApplyJumpCut()
  {
    if (ctx.spaceKeyUp && ctx.rb.velocity.y > 0)
    {
      ctx.rb.AddForce(Vector2.down * ctx.rb.velocity.y * (1 - ctx.jumpCutMultiplier), ForceMode2D.Impulse);
    }
  }

  private void ApplyFallGravity()
  {
    if (ctx.rb.velocity.y < 0)
    {
      ctx.rb.gravityScale = ctx.gravityScale * ctx.fallGravityMultiplier;
    }
    else
    {
      ctx.rb.gravityScale = ctx.gravityScale;
    }
  }
}