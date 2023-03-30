using UnityEngine;

public class Droplet : MonoBehaviour
{
  public Animator animator;

  private void Start()
  {
    GameObject player = GameObject.Find("Player");
    Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    animator.Play("Contact");
  }

  private void Die()
  {
    Destroy(gameObject);
  }
}
