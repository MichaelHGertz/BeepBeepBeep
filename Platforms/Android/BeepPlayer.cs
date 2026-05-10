using Android.Media;

namespace BeepBeepBeep;

public class BeepPlayer : IBeepPlayer
{
    public Task PlayThreeBeepsAsync() => Task.Run(() =>
    {
        using var toneGen = new ToneGenerator(Android.Media.Stream.Music, 100);
        for (int i = 0; i < 3; i++)
        {
            toneGen.StartTone(Tone.PropBeep, 200);
            Thread.Sleep(200);
            toneGen.StopTone();
            if (i < 2)
                Thread.Sleep(300);
        }
    });
}
