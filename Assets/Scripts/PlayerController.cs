using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
  // States
  public GroundedState groundedState { private set; get; }
  public AirbornState airbornState { private set; get; }
  public IFramesState iFramesState { private set; get; }
  public static BaseState currentState { private set; get; }
  public static string subState { private set; get; }

  // Input
  public float horizontalAxis { private set; get; }
  public bool spaceKeyDown { private set; get; }
  public bool spaceKeyUp { private set; get; }

  // Components
  public Rigidbody2D rb { private set; get; }
  public SpriteRenderer spriteRenderer { private set; get; }
  public BoxCollider2D boxCollider { private set; get; }

  // Leeway Timers
  public Leeway coyoteTime { private set; get; }
  public Leeway jumpBuffer { private set; get; }
  public Leeway bounceBuffer { private set; get; }

  // Other
  private CameraEffects cameraEffects;

  [Header("Movement Parameters")]
  public float maxVelocity = 2f;
  public float runAcceleration = 1.5f;
  public float runDeceleration = 1.5f;
  public float airAcceleration = 1.2f;
  public float airDeceleration = 1.2f;
  public float maxFallVelocity = -15f;
  public float friction = 0.2f;
  public float gravityScale = 1f;
  public float fallGravityMultiplier = 1.5f;
  public float jumpForce = 1f;
  public float jumpOffEnemyForce = 1f;
  public float jumpOffEnemyWithTimingForce = 1f;
  public float jumpCutMultiplier = 0.1f;
  public float coyoteTimeAmount = 0.15f;
  public float jumpBufferAmount = 0.2f;
  public float bounceBufferAmount = 0.35f;
  public float iFramesGravityMultiplier = 0.5f;
  public float iFramesLength = 1.5f;
  public float iFrameRecoilMultiplier = 1.2f;

  [Header("Debug Options")]
  public bool logStateContinuously = false;
  public bool includeSubState = false;
  public float logInterval = 1f;
  private Coroutine logStateRoutine;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    boxCollider = GetComponent<BoxCollider2D>();

    groundedState = new GroundedState(this);
    airbornState = new AirbornState(this);
    iFramesState = new IFramesState(this);
    SetState(groundedState);
    SetSubstate(groundedState.ToString());

    coyoteTime = new Leeway(coyoteTimeAmount);
    jumpBuffer = new Leeway(jumpBufferAmount);
    bounceBuffer = new Leeway(bounceBufferAmount);

    cameraEffects = Camera.main.GetComponent<CameraEffects>();

    if (logStateContinuously)
    {
      logStateRoutine = StartCoroutine(LogStateRoutine());
    }
  }

  private void OnDestroy()
  {
    if (logStateRoutine != null)
    {
      StopCoroutine(LogStateRoutine());
    }
  }

  private IEnumerator LogStateRoutine()
  {
    while (true)
    {
      yield return new WaitForSeconds(logInterval);

      string info = "State: " + currentState;
      if (includeSubState)
      {
        info += " Substate: " + subState;
      }

      print(info);
    }
  }

  private void SetInputs()
  {
    horizontalAxis = InputController.GetAxisHorz();
    spaceKeyDown = InputController.GetSpaceKeyDown();
    spaceKeyUp = InputController.GetSpaceKeyUp();
  }

  private void FlipSprite()
  {
    if (horizontalAxis > 0)
    {
      spriteRenderer.flipX = false;
    }
    else if (horizontalAxis < 0)
    {
      spriteRenderer.flipX = true;
    }
  }

  private void TickTimers()
  {
    if (currentState == groundedState)
    {
      coyoteTime.Reset();
    }
    else
    {
      coyoteTime.Tick(Time.deltaTime);
    }

    if (spaceKeyDown)
    {
      jumpBuffer.Reset();
      bounceBuffer.Reset();
    }
    else
    {
      jumpBuffer.Tick(Time.deltaTime);
      bounceBuffer.Tick(Time.deltaTime);
    }
  }

  private void Update()
  {
    SetInputs();
    FlipSprite();
    TickTimers();

    SetState(currentState.Update());
  }

  private void FixedUpdate()
  {
    currentState.FixedUpdate();
  }

  public Collider2D GetGroundedCollider()
  {
    return Physics2D.OverlapCapsule(transform.position, new Vector2(0.2f, 0.03f), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Ground"));
  }

  private bool CheckGround()
  {
    return GetGroundedCollider();
  }

  public void Jump(float force)
  {
    rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    coyoteTime.Invalidate();
    jumpBuffer.Invalidate();
    bounceBuffer.Invalidate();
    SetState(airbornState);
  }

  public void SetState(BaseState state)
  {
    // If returned state is null, preserve current state and substate
    if (state == null)
    {
      return;
    }

    currentState = state;

    // Just reset the substate to stringified state in order
    // to invalidate substate changes from individual states
    SetSubstate(currentState.ToString());
  }

  public void SetSubstate(string _subState)
  {
    subState = _subState;
  }

  private void OnCollisionStay2D(Collision2D collision)
  {
    bool collidedWithGroundLayer = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
    bool collidedAtFeet = CheckGround();
    if (collidedWithGroundLayer && collidedAtFeet)
    {
      if (jumpBuffer.Valid())
      {
        Jump(jumpForce);
      }
      else
      {
        SetState(groundedState);
      }
    }
  }

  private void OnCollisionExit2D(Collision2D collision)
  {
    bool leftGroundLayer = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
    if (leftGroundLayer)
    {
      SetState(airbornState);
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.tag == "Deals Damage")
    {
      OnTriggerEnter2DEnemy(other);
    }
  }

  private Collider2D GetBounceCollider()
  {
    return Physics2D.OverlapCapsule(transform.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Bounce"));
  }

  private void BounceOffEnemy(Collider2D other)
  {
    rb.velocity = new Vector2(rb.velocity.x, 0);

    if (bounceBuffer.Valid())
    {
      Jump(jumpOffEnemyWithTimingForce);
    }
    else
    {
      Jump(jumpOffEnemyForce);
    }

    Respawnable respawnable = other.gameObject.GetComponent<Respawnable>();
    StartCoroutine(respawnable.PeriodicallyKillRespawnableRoutine());
    SetState(airbornState);
  }

  private void OnTriggerEnter2DEnemy(Collider2D other)
  {
    Collider2D bounceCollider = GetBounceCollider();

    if (bounceCollider)
    {
      BounceOffEnemy(other);
    }
    else
    {
      StartCoroutine(iFramesState.IFramesRoutine());
      StartCoroutine(cameraEffects.ShakeRoutine());
    }
  }
}