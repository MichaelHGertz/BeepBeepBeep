namespace BeepBeepBeep;

public interface IBeepPlayer
{
    Task PlayLongAlertBeepAsync();
    Task PlayAlertBeepAsync();
    Task PlaySingleBeepAsync();
    Task PlayThreeBeepsAsync();
}
