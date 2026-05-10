using System.Runtime.InteropServices;
using System.Text;

namespace BeepBeepBeep;

public class BeepPlayer : IBeepPlayer
{
    [DllImport("winmm.dll")]
    private static extern bool PlaySound(byte[] pszSound, IntPtr hmod, uint fdwSound);

    private const uint SND_SYNC = 0x0000;
    private const uint SND_MEMORY = 0x0004;

    public async Task PlayThreeBeepsAsync()
    {
        for (int i = 0; i < 3; i++)
        {
            await Task.Run(PlaySingleBeep);
            if (i < 2)
                await Task.Delay(300);
        }
    }

    private static void PlaySingleBeep()
    {
        PlaySound(GenerateBeepWav(880, 200), IntPtr.Zero, SND_SYNC | SND_MEMORY);
    }

    private static byte[] GenerateBeepWav(double frequency, int durationMs)
    {
        const int sampleRate = 22050;
        int numSamples = sampleRate * durationMs / 1000;
        int dataSize = numSamples * 2;

        using var stream = new MemoryStream();
        using var w = new BinaryWriter(stream);

        w.Write(Encoding.ASCII.GetBytes("RIFF"));
        w.Write(36 + dataSize);
        w.Write(Encoding.ASCII.GetBytes("WAVE"));
        w.Write(Encoding.ASCII.GetBytes("fmt "));
        w.Write(16);
        w.Write((short)1);       // PCM
        w.Write((short)1);       // mono
        w.Write(sampleRate);
        w.Write(sampleRate * 2); // byte rate
        w.Write((short)2);       // block align
        w.Write((short)16);      // bits per sample
        w.Write(Encoding.ASCII.GetBytes("data"));
        w.Write(dataSize);

        for (int i = 0; i < numSamples; i++)
        {
            double t = (double)i / sampleRate;
            double env = Math.Min((double)i / 100, Math.Min(1.0, (double)(numSamples - i) / 100));
            w.Write((short)(short.MaxValue * 0.8 * env * Math.Sin(2 * Math.PI * frequency * t)));
        }

        return stream.ToArray();
    }
}
