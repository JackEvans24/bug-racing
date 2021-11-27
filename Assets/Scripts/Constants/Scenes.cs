using System.Linq;

public enum Scenes
{
    Menu,
    OakHighway,
    Extras,
    BugTesting
}

public static class ScenesExtensions
{
    private static Scenes[] raceScenes = new[] { Scenes.OakHighway, Scenes.BugTesting };

    public static bool IsRace(this Scenes scene) => raceScenes.Contains(scene);
}
