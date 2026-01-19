using System.Collections.Generic;

public static class ArcadeRunState
{
    public const int MaxStages = 3;
    public static bool IsRunning { get; private set; }
    public static int CurrentStage { get; private set; }
    public static List<SongMeta> PlayedSongs { get; } = new();
    public static void StartNewRun()
    {
        IsRunning = true;
        CurrentStage = 1;
        PlayedSongs.Clear();
    }

    public static void OnSongCleared(SongMeta song)
    {
        if (song != null)
            PlayedSongs.Add(song);

        CurrentStage++;
    }

    public static bool IsFinished =>
        CurrentStage > MaxStages;

    public static void EndRun()
    {
        IsRunning = false;
    }
}
