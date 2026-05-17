namespace BeepBeepBeep;

public partial class MainPage : ContentPage
{
    private readonly IBeepPlayer _beepPlayer;
    private CancellationTokenSource? _cts;

    private int _primarySeconds = 10;
    private int _secondarySeconds = 5;
    private int _repetitions = 5;
    private bool _reminderEnabled = false;
    private int _reminderIntervalSeconds = 10;
    private TaskCompletionSource? _pauseTcs;

    public MainPage(IBeepPlayer beepPlayer)
    {
        InitializeComponent();
        _beepPlayer = beepPlayer;
        SettingsDrawer.TranslationY = 800;
        LoadPreferences();
        PrimaryTimeEntry.Text = _primarySeconds.ToString();
        SecondaryTimeEntry.Text = _secondarySeconds.ToString();
        RepsLabel.Text = _repetitions.ToString();
        ReminderSwitch.IsToggled = _reminderEnabled;
        ReminderIntervalEntry.Text = _reminderIntervalSeconds.ToString();
        ReminderIntervalSection.IsVisible = _reminderEnabled;
        UpdateSettingsSummary();
    }

    private void LoadPreferences()
    {
        _primarySeconds = Preferences.Get("primarySeconds", _primarySeconds);
        _secondarySeconds = Preferences.Get("secondarySeconds", _secondarySeconds);
        _repetitions = Preferences.Get("repetitions", _repetitions);
        _reminderEnabled = Preferences.Get("reminderEnabled", _reminderEnabled);
        _reminderIntervalSeconds = Preferences.Get("reminderIntervalSeconds", _reminderIntervalSeconds);
    }

    private void SavePreferences()
    {
        Preferences.Set("primarySeconds", _primarySeconds);
        Preferences.Set("secondarySeconds", _secondarySeconds);
        Preferences.Set("repetitions", _repetitions);
        Preferences.Set("reminderEnabled", _reminderEnabled);
        Preferences.Set("reminderIntervalSeconds", _reminderIntervalSeconds);
    }

    private void UpdateSettingsSummary()
    {
        var reminder = _reminderEnabled ? $"  ·  reminder {_reminderIntervalSeconds}s" : "";
        SettingsSummaryLabel.Text = $"{_primarySeconds}s  ·  {_secondarySeconds}s  ·  {_repetitions} reps{reminder}";
    }

    // ── Settings drawer ───────────────────────────────────────────────────────

    private void OnSettingsClicked(object? sender, EventArgs e) =>
        _ = OpenSettingsDrawerAsync();

    private void OnSettingsDoneClicked(object? sender, EventArgs e) =>
        _ = CloseSettingsDrawerAsync();

    private void OnOverlayTapped(object? sender, TappedEventArgs e) =>
        _ = CloseSettingsDrawerAsync();

    private async Task OpenSettingsDrawerAsync()
    {
        DimOverlay.Opacity = 0;
        DimOverlay.IsVisible = true;
        SettingsDrawer.IsVisible = true;
        await Task.WhenAll(
            DimOverlay.FadeTo(1, 200),
            SettingsDrawer.TranslateTo(0, 0, 300, Easing.CubicOut));
    }

    private async Task CloseSettingsDrawerAsync()
    {
        await Task.WhenAll(
            DimOverlay.FadeTo(0, 200),
            SettingsDrawer.TranslateTo(0, 800, 300, Easing.CubicIn));
        DimOverlay.IsVisible = false;
        SettingsDrawer.IsVisible = false;
        SavePreferences();
        UpdateSettingsSummary();
    }

    // ── Primary time ──────────────────────────────────────────────────────────

    private void OnPrimaryDecrement(object? sender, EventArgs e)
    {
        if (_primarySeconds > 1) _primarySeconds--;
        PrimaryTimeEntry.Text = _primarySeconds.ToString();
    }

    private void OnPrimaryIncrement(object? sender, EventArgs e)
    {
        _primarySeconds++;
        PrimaryTimeEntry.Text = _primarySeconds.ToString();
    }

    private void OnPrimaryTimeChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int v) && v >= 1)
            _primarySeconds = v;
    }

    // ── Secondary time ────────────────────────────────────────────────────────

    private void OnSecondaryDecrement(object? sender, EventArgs e)
    {
        if (_secondarySeconds > 1) _secondarySeconds--;
        SecondaryTimeEntry.Text = _secondarySeconds.ToString();
    }

    private void OnSecondaryIncrement(object? sender, EventArgs e)
    {
        _secondarySeconds++;
        SecondaryTimeEntry.Text = _secondarySeconds.ToString();
    }

    private void OnSecondaryTimeChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int v) && v >= 1)
            _secondarySeconds = v;
    }

    // ── Repetitions ───────────────────────────────────────────────────────────

    private void OnRepsDecrement(object? sender, EventArgs e)
    {
        if (_repetitions > 1) _repetitions--;
        RepsLabel.Text = _repetitions.ToString();
    }

    private void OnRepsIncrement(object? sender, EventArgs e)
    {
        if (_repetitions < 20) _repetitions++;
        RepsLabel.Text = _repetitions.ToString();
    }

    // ── Reminder beep ─────────────────────────────────────────────────────────

    private void OnReminderToggled(object? sender, ToggledEventArgs e)
    {
        _reminderEnabled = e.Value;
        ReminderIntervalSection.IsVisible = _reminderEnabled;
    }

    private void OnReminderIntervalDecrement(object? sender, EventArgs e)
    {
        if (_reminderIntervalSeconds > 1) _reminderIntervalSeconds--;
        ReminderIntervalEntry.Text = _reminderIntervalSeconds.ToString();
    }

    private void OnReminderIntervalIncrement(object? sender, EventArgs e)
    {
        _reminderIntervalSeconds++;
        ReminderIntervalEntry.Text = _reminderIntervalSeconds.ToString();
    }

    private void OnReminderIntervalChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int v) && v >= 1)
            _reminderIntervalSeconds = v;
    }

    // ── Session control ───────────────────────────────────────────────────────

    private void OnStartClicked(object? sender, EventArgs e)
    {
        if (!int.TryParse(PrimaryTimeEntry.Text, out int p) || p < 1) p = 1;
        if (!int.TryParse(SecondaryTimeEntry.Text, out int s) || s < 1) s = 1;
        _primarySeconds = p;
        _secondarySeconds = s;

        SavePreferences();
        _cts = new CancellationTokenSource();
        PreSessionPanel.IsVisible = false;
        ActivePanel.IsVisible = true;
        _ = RunSessionAsync(_cts.Token);
    }

    private void OnPauseClicked(object? sender, EventArgs e)
    {
        if (_pauseTcs == null)
        {
            _pauseTcs = new TaskCompletionSource();
            PauseButton.Text = "RESUME";
        }
        else
        {
            _pauseTcs.SetResult();
            _pauseTcs = null;
            PauseButton.Text = "PAUSE";
        }
    }

    private void OnStopClicked(object? sender, EventArgs e)
    {
        _pauseTcs?.SetResult();
        _pauseTcs = null;
        _cts?.Cancel();
    }

    // ── Session loop ──────────────────────────────────────────────────────────

    private async Task RunSessionAsync(CancellationToken token)
    {
        try
        {
            for (int rep = 1; rep <= _repetitions; rep++)
            {
                SetRepLabel(rep, _repetitions);

                _ = _beepPlayer.PlayLongAlertBeepAsync();
                await RunCountdownAsync("PRIMARY", "#FF4757", _primarySeconds, isPrimary: true, token).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();

                await RunCountdownAsync("SECONDARY", "#2ED573", _secondarySeconds, isPrimary: false, token).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            _pauseTcs = null;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PauseButton.Text = "PAUSE";
                ActivePanel.IsVisible = false;
                PreSessionPanel.IsVisible = true;
            });
        }
    }

    private async Task RunCountdownAsync(string phase, string colorHex, int seconds, bool isPrimary, CancellationToken token)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PhaseLabel.Text = phase;
            PhaseLabel.TextColor = Color.FromArgb(colorHex);
        });

        for (int remaining = seconds; remaining >= 0; remaining--)
        {
            token.ThrowIfCancellationRequested();
            if (_pauseTcs != null)
                await _pauseTcs.Task.WaitAsync(token);

            int display = remaining;
            MainThread.BeginInvokeOnMainThread(() =>
                CountdownLabel.Text = $"{display / 60:D2}:{display % 60:D2}");

            if (isPrimary && _reminderEnabled && _reminderIntervalSeconds > 0 && remaining > 0)
            {
                int elapsed = seconds - remaining;
                if (elapsed > 0 && elapsed % _reminderIntervalSeconds == 0)
                    _ = _beepPlayer.PlaySingleBeepAsync();
            }

            if (remaining >= 1 && remaining <= 3)
                _ = _beepPlayer.PlayAlertBeepAsync();

            if (remaining > 0)
                await Task.Delay(1000, token).ConfigureAwait(false);
        }
    }

    private void SetRepLabel(int current, int total)
    {
        MainThread.BeginInvokeOnMainThread(() =>
            RepCounterLabel.Text = $"Rep {current} of {total}");
    }
}
