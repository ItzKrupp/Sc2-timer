using System.Speech.Synthesis;

namespace Sc2Timer.App.Speech;

/// <summary>
/// Speaks cues using the offline SAPI voices built into Windows (System.Speech).
/// No network access or API key required. SpeakAsync queues calls internally,
/// so cues that fire close together are read out in order rather than cutting each other off.
/// </summary>
public sealed class SapiSpeechCuePlayer : IDisposable
{
    private readonly SpeechSynthesizer _synthesizer = new();

    public SapiSpeechCuePlayer()
    {
        _synthesizer.SetOutputToDefaultAudioDevice();
    }

    public void Speak(string text) => _synthesizer.SpeakAsync(text);

    public void Dispose() => _synthesizer.Dispose();
}
