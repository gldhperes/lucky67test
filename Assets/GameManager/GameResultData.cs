public enum GameResultType
{
    None,
    Win,
    Lose
}

public static class GameResultData
{
    public static GameResultType Result { get; private set; }
        = GameResultType.None;

    public static void SetResult(GameResultType result)
    {
        Result = result;
    }

    public static void Reset()
    {
        Result = GameResultType.None;
    }
}