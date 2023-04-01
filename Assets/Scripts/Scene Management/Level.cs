using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Level
{
  private string prefix = "Level ";
  private Regex re;
  public int Number { private set; get; }

  public Level(string name)
  {
    re = new Regex(Regex.Escape(prefix) + @"(?<number>\d+)");
    Number = Parse(name);
  }

  public Level GetNext()
  {
    // TODO: should there be a cap?
    // Realistically, probably doesn't matter
    return new Level($"{prefix}{Number + 1}");
  }

  public Level GetPrev()
  {
    // Don't allow Level "0" - Anything below level 1 just goes to level 1
    int prev = Number > 1 ? Number - 1 : 1;
    return new Level($"{prefix}{prev}");
  }

  public override string ToString()
  {
    return $"{prefix}{Number}";
  }

  private int Parse(string name)
  {
    Match match = re.Match(name);
    if (!match.Success)
    {
      throw new Exception($"Failed to parse level information from string: {name}");
    }

    return Int32.Parse(match.Groups["number"].Value);
  }
}

public static class LevelLoader
{
  public static Level GetCurrentLevel()
  {
    return new Level(SceneManager.GetActiveScene().name);
  }

  public static IEnumerator ReloadCurrentLevelRoutine()
  {
    return AsyncLoader.LoadSceneRoutine(GetCurrentLevel().ToString());
  }

  public static IEnumerator LoadNextLevelRoutine()
  {
    return AsyncLoader.LoadSceneRoutine(GetCurrentLevel().GetNext().ToString());
  }

  public static IEnumerator LoadPreviousLevelRoutine()
  {
    return AsyncLoader.LoadSceneRoutine(GetCurrentLevel().GetPrev().ToString());
  }
}