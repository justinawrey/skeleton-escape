using UnityEngine;
using System.Collections;

public class IFramesState : BaseState
{
  public IFramesState(PlayerController context) : base(context) { }

  public IEnumerator IFramesRoutine()
  {
    BaseState prevState = ctx.currentState;
    ctx.SetState(ctx.iFramesState);
    ctx.boxCollider.enabled = false;
    ctx.rb.gravityScale = ctx.gravityScale * ctx.iFramesGravityMultiplier;
    Vector2 currVelocity = ctx.rb.velocity;
    ctx.rb.velocity = new Vector2(0, 0);
    ctx.rb.AddForce(new Vector2(-currVelocity.x, -currVelocity.y).normalized * ctx.iFrameRecoilMultiplier, ForceMode2D.Impulse);

    float interval = ctx.iFramesLength / 5;
    for (float i = 0; i <= ctx.iFramesLength; i += interval)
    {
      ctx.spriteRenderer.enabled = !ctx.spriteRenderer.enabled;
      yield return new WaitForSeconds(interval);
    }

    ctx.spriteRenderer.enabled = true;
    ctx.boxCollider.enabled = true;
    ctx.SetState(prevState);
    yield return null;
  }
}