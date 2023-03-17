using UnityEngine;

public class OffsetIntroAnimation : MonoBehaviour
{
  public Animator animator;
  public string animationName;

  private void Start()
  {
    float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
    animator.Play(animationName, 0, randomIdleStart);
  }
}
