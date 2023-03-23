using System;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
  Left,
  Right,
  Up,
  Down
}

[Serializable]
public struct MoveCommand
{
  public Direction dir;
  public float numUnits;
  public float speed;
}

public class MotionBetweenPoints : MonoBehaviour
{
  private bool finished = false;
  private int currMoveCommand;
  private Vector2 currDestPoint;

  public Rigidbody2D rb;
  public List<MoveCommand> moveCommands;
  public bool loop = true;
  public bool flipSprite = true;

  private void Start()
  {
    currMoveCommand = 0;
    currDestPoint = CalculateDestPoint(moveCommands[currMoveCommand]);
  }

  private Vector2 CalculateDestPoint(MoveCommand moveCommand)
  {
    switch (moveCommand.dir)
    {
      case Direction.Left:
        return rb.position + (Vector2.left * moveCommand.numUnits);
      case Direction.Right:
        return rb.position + (Vector2.right * moveCommand.numUnits);
      case Direction.Up:
        return rb.position + (Vector2.up * moveCommand.numUnits);
      case Direction.Down:
        return rb.position + (Vector2.down * moveCommand.numUnits);
      default:
        return rb.position;
    }
  }

  private bool AtDestPoint()
  {
    Direction currDir = moveCommands[currMoveCommand].dir;
    switch (currDir)
    {
      case Direction.Left:
        return rb.position.x <= currDestPoint.x;
      case Direction.Right:
        return rb.position.x >= currDestPoint.x;
      case Direction.Up:
        return rb.position.y >= currDestPoint.y;
      case Direction.Down:
        return rb.position.y <= currDestPoint.y;
      default:
        return false;
    }
  }

  private void FixedUpdate()
  {
    if (finished)
    {
      return;
    }

    if (AtDestPoint())
    {
      currMoveCommand += 1;
      if (currMoveCommand == moveCommands.Count)
      {
        if (!loop)
        {
          StopMovement();
        }
        currMoveCommand = 0;
      }

      currDestPoint = CalculateDestPoint(moveCommands[currMoveCommand]);
    }

    MoveCommand moveCommand = moveCommands[currMoveCommand];

    if (flipSprite)
    {
      if (moveCommand.dir == Direction.Left)
      {
        rb.GetComponentInParent<SpriteRenderer>().flipX = true;
      }
      else if (moveCommand.dir == Direction.Right)
      {
        rb.GetComponentInParent<SpriteRenderer>().flipX = false;
      }
    }

    float velocity = moveCommand.speed;
    Direction dir = moveCommand.dir;
    Vector2 currPos = rb.position;
    Vector2 newPos = currPos;

    switch (dir)
    {
      case Direction.Left:
        newPos = currPos + (Vector2.left * velocity * Time.fixedDeltaTime);
        break;
      case Direction.Right:
        newPos = currPos + (Vector2.right * velocity * Time.fixedDeltaTime);
        break;
      case Direction.Up:
        newPos = currPos + (Vector2.up * velocity * Time.fixedDeltaTime);
        break;
      case Direction.Down:
        newPos = currPos + (Vector2.down * velocity * Time.fixedDeltaTime);
        break;
    }

    rb.MovePosition(newPos);
  }

  public float GetCurrMovementVelocityX()
  {
    MoveCommand command = moveCommands[currMoveCommand];
    return command.dir == Direction.Left ? -(command.speed) : command.speed;
  }

  public void StopMovement()
  {
    finished = true;
  }

  public void StartMovement()
  {
    finished = false;
  }
}