using System.Linq;

public enum Scenes
{
    Menu,
    OakHighway,
    Extras,
    BugTesting,
    OakHighwayNight,
    SandBurrows
}

public static class ScenesExtensions
{
    private static Scenes[] raceScenes = new[] { Scenes.OakHighway, Scenes.BugTesting, Scenes.OakHighwayNight, Scenes.SandBurrows };

    public static bool IsRace(this Scenes scene) => raceScenes.Contains(scene);
}
