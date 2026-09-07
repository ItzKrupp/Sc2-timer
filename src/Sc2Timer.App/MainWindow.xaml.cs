using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Sc2Timer.App.Speech;
using Sc2Timer.Core;
using Sc2Timer.Core.Models;

namespace Sc2Timer.App;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _tickTimer;
    private readonly GameClock _clock = new();
    private readonly SapiSpeechCuePlayer _speechPlayer = new();

    private BuildOrderFile? _buildOrder;
    private CueScheduler? _scheduler;

    public MainWindow()
    {
        InitializeComponent();

        _tickTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _tickTimer.Tick += (_, _) => OnTick();
        _tickTimer.Start();

        RefreshUi();
        UpdateButtonStates();
    }

    protected override void OnClosed(EventArgs e)
    {
        _speechPlayer.Dispose();
        base.OnClosed(e);
    }

    private void OnImportClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Build order JSON (*.json)|*.json|All files (*.*)|*.*",
            Title = "Import build order",
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            var buildOrder = BuildOrderLoader.LoadFromFile(dialog.FileName);
            var timeline = TimelineBuilder.Build(buildOrder);

            _buildOrder = buildOrder;
            _scheduler = new CueScheduler(timeline);
            _clock.Reset();

            BuildNameText.Text = string.IsNullOrWhiteSpace(buildOrder.Matchup)
                ? buildOrder.Name
                : $"{buildOrder.Name} ({buildOrder.Matchup})";
        }
        catch (Exception ex) when (ex is InvalidBuildOrderException or IOException)
        {
            MessageBox.Show(this, ex.Message, "Couldn't load build order",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        RefreshUi();
        UpdateButtonStates();
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        _clock.Start();
        UpdateButtonStates();
    }

    private void OnPauseClick(object sender, RoutedEventArgs e)
    {
        _clock.Pause();
        UpdateButtonStates();
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        _clock.Reset();
        _scheduler?.Reset();
        RefreshUi();
        UpdateButtonStates();
    }

    private void OnTick()
    {
        if (_scheduler is null)
        {
            return;
        }

        var elapsed = _clock.Elapsed;
        foreach (var cue in _scheduler.Poll(elapsed))
        {
            _speechPlayer.Speak(cue.Speech);
        }

        RefreshUi();
    }

    private void RefreshUi()
    {
        var elapsed = _clock.Elapsed;
        ClockText.Text = GameTime.Format(elapsed);

        if (_scheduler is null)
        {
            NextLabel.Text = "—";
            NextCountdown.Text = "";
            UpcomingList.ItemsSource = null;
            return;
        }

        var next = _scheduler.GetNext(elapsed);
        if (next is not null)
        {
            NextLabel.Text = next.Label;
            var remainingSeconds = Math.Max(0, (int)(next.Time - elapsed).TotalSeconds);
            NextCountdown.Text = $"in {remainingSeconds}s";
        }
        else
        {
            NextLabel.Text = "Build complete";
            NextCountdown.Text = "";
        }

        UpcomingList.ItemsSource = _scheduler.GetUpcoming(elapsed, 6)
            .Select(ev => $"{GameTime.Format(ev.Time)}  {ev.Label}")
            .ToList();
    }

    private void UpdateButtonStates()
    {
        var hasBuildOrder = _scheduler is not null;
        StartButton.IsEnabled = hasBuildOrder && _clock.State != ClockState.Running;
        PauseButton.IsEnabled = hasBuildOrder && _clock.State == ClockState.Running;
        ResetButton.IsEnabled = hasBuildOrder && _clock.State != ClockState.Stopped;
    }
}
