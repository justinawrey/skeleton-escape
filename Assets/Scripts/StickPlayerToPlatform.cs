using UnityEngine;

public class StickPlayerToPlatform : MonoBehaviour
{
  private Transform player = null;

  public Rigidbody2D rb;
  public Transform playerCheck;
  public LayerMask playerMask;

  void FixedUpdate()
  {
    Collider2D collider = Physics2D.OverlapBox(playerCheck.position, new Vector2(1, 0.15f), 0, playerMask);

    // if (collider)
    // {
    //   collider.gameObject.transform.SetParent(transform);
    //   player = collider.gameObject.transform;
    // }
    // else
    // {
    //   if (player)
    //   {
    //     player.parent = null;
    //   }
    // }
  }
}
