using UnityEngine;

public static class InputController
{
  public static bool inputEnabled = true;

  public static float GetAxisHorz()
  {
    if (!inputEnabled)
    {
      return 0;
    }

    return Input.GetAxisRaw("Horizontal");
  }

  public static float GetAxisVert()
  {
    if (!inputEnabled)
    {
      return 0;
    }

    return Input.GetAxisRaw("Vertical");
  }

  public static bool GetSpaceKeyDown()
  {
    if (!inputEnabled)
    {
      return false;
    }

    return Input.GetKeyDown(KeyCode.Space);
  }


  public static bool GetSpaceKeyUp()
  {
    if (!inputEnabled)
    {
      return false;
    }

    return Input.GetKeyUp(KeyCode.Space);
  }
}
