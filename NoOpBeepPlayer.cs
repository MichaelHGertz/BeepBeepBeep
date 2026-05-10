namespace BeepBeepBeep;

public class NoOpBeepPlayer : IBeepPlayer
{
    public Task PlayThreeBeepsAsync() => Task.CompletedTask;
}
