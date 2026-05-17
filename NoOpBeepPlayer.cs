namespace BeepBeepBeep;

public class NoOpBeepPlayer : IBeepPlayer
{
    public Task PlayLongAlertBeepAsync() => Task.CompletedTask;
    public Task PlayAlertBeepAsync() => Task.CompletedTask;
    public Task PlaySingleBeepAsync() => Task.CompletedTask;
    public Task PlayThreeBeepsAsync() => Task.CompletedTask;
}
