using Android.Media;

namespace BeepBeepBeep;

public class BeepPlayer : IBeepPlayer
{
    private const int SampleRate = 44100;
    private readonly SoundPool _pool;
    private readonly int _longAlertId;
    private readonly int _alertId;
    private readonly int _reminderId;

    public BeepPlayer()
    {
        _pool = new SoundPool.Builder()
            .SetMaxStreams(4)
            .SetAudioAttributes(new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build())
            .Build()!;

        int loaded = 0;
        var ready = new TaskCompletionSource();
        _pool.LoadComplete += (_, _) =>
        {
            if (Interlocked.Increment(ref loaded) >= 3)
                ready.TrySetResult();
        };

        // Durations here are the total ring-out length, not a sustained buzz.
        // The envelope decays to ~1 % by the end of each buffer.
        _longAlertId = LoadSound(880,  1200);
        _alertId     = LoadSound(880,  500);
        _reminderId  = LoadSound(1400, 250);

        ready.Task.Wait(3000);
    }

    public Task PlayLongAlertBeepAsync() { _pool.Play(_longAlertId, 1f,   1f,   1, 0, 1f); return Task.CompletedTask; }
    public Task PlayAlertBeepAsync()     { _pool.Play(_alertId,     1f,   1f,   1, 0, 1f); return Task.CompletedTask; }
    public Task PlaySingleBeepAsync()    { _pool.Play(_reminderId,  0.7f, 0.7f, 1, 0, 1f); return Task.CompletedTask; }

    public Task PlayThreeBeepsAsync() => Task.Run(async () =>
    {
        for (int i = 0; i < 3; i++)
        {
            _pool.Play(_alertId, 1f, 1f, 1, 0, 1f);
            if (i < 2) await Task.Delay(500);
        }
    });

    private int LoadSound(double hz, int ms)
    {
        string path = Path.Combine(FileSystem.CacheDirectory, $"tone_{(int)hz}hz_{ms}ms.wav");
        if (!File.Exists(path))
            File.WriteAllBytes(path, GenerateWav(hz, ms));
        return _pool.Load(path, 1);
    }

    // Generates a bell-like tone: fast linear attack + exponential decay + harmonics.
    // durationMs is the total ring-out time; the envelope reaches ~1 % at the end.
    private static byte[] GenerateWav(double frequency, int durationMs)
    {
        int n = SampleRate * durationMs / 1000;
        int dataSize = n * 2;
        using var ms = new MemoryStream(44 + dataSize);
        using var w = new BinaryWriter(ms);
        Ascii(w, "RIFF"); w.Write(36 + dataSize);
        Ascii(w, "WAVE");
        Ascii(w, "fmt "); w.Write(16); w.Write((short)1); w.Write((short)1);
        w.Write(SampleRate); w.Write(SampleRate * 2); w.Write((short)2); w.Write((short)16);
        Ascii(w, "data"); w.Write(dataSize);

        int attackSamples = Math.Min(SampleRate / 100, n / 4); // 10 ms attack
        double tau = durationMs / 1000.0 / 4.6;                // decay to ~1 % at t = durationMs

        for (int i = 0; i < n; i++)
        {
            double t      = (double)i / SampleRate;
            double attack = i < attackSamples ? (double)i / attackSamples : 1.0;
            double env    = attack * Math.Exp(-t / tau);

            // Fundamental + 2nd harmonic (warmth) + 3rd harmonic (brightness)
            double sample = (Math.Sin(2 * Math.PI * frequency * t)
                           + 0.4 * Math.Sin(4 * Math.PI * frequency * t)
                           + 0.1 * Math.Sin(6 * Math.PI * frequency * t)) / 1.5;

            w.Write((short)(short.MaxValue * env * sample));
        }
        return ms.ToArray();
    }

    private static void Ascii(BinaryWriter w, string s) { foreach (char c in s) w.Write((byte)c); }
}
