using UnityEngine;

public class StickToCamera : MonoBehaviour
{
  public SpriteRenderer spriteRenderer;
  public float xOffset, yOffset;

  private void Update()
  {
    Camera mainCamera = Camera.main;
    float halfViewportX = mainCamera.orthographicSize * mainCamera.aspect;
    float halfViewportY = mainCamera.orthographicSize;

    Bounds spriteBounds = spriteRenderer.sprite.bounds;
    float x = -(halfViewportX - spriteBounds.extents.x - xOffset);
    float y = (halfViewportY - spriteBounds.extents.y - yOffset);

    transform.position = new Vector3(x + mainCamera.transform.position.x, y + mainCamera.transform.position.y, transform.position.z);
  }
}
