using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlowlyDie : MonoBehaviour
{
  private float TOTAL_STEPS = 100;
  private float currFullness = 100;
  private float drainRate;
  private float totalWorldUnits, distancePerStep;
  private Coroutine drainRoutine;

  public float fullYCoord, emptyYCoord;
  public float timeToEmpty = 4f;
  public bool allowDraining = true;

  private void Start()
  {
    drainRate = timeToEmpty / TOTAL_STEPS;
    totalWorldUnits = fullYCoord - emptyYCoord;
    distancePerStep = totalWorldUnits / TOTAL_STEPS;

    // Lets go!
    SetFullness(TOTAL_STEPS);
    drainRoutine = StartCoroutine(ConstantlyDrainRoutine());
  }

  private void OnDisable()
  {
    StopCoroutine(drainRoutine);
  }

  // Where amt is between 0-100, 0 being empty, 100 being full
  private void SetFullness(float amt)
  {
    float distanceTravelled = (TOTAL_STEPS - amt) * distancePerStep;
    float newY = fullYCoord - distanceTravelled;
    transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
  }

  private IEnumerator ConstantlyDrainRoutine()
  {
    while (currFullness > 0)
    {
      if (allowDraining)
      {
        currFullness--;
        SetFullness(currFullness);
      }
      yield return new WaitForSeconds(drainRate);
    }

    GameOver();
    yield return null;
  }

  private void GameOver()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void AddToFullness(float amt)
  {
    currFullness += amt;
    if (currFullness > TOTAL_STEPS)
    {
      currFullness = TOTAL_STEPS;
    }
  }
}
