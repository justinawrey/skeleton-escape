public class BaseState
{
  public PlayerController ctx;
  public BaseState(PlayerController ctx)
  {
    this.ctx = ctx;
  }

  public virtual BaseState Update() { return ctx.currentState; }
  public virtual void FixedUpdate() { }
}