using System;
using System.Diagnostics;
using System.IO;
using LiveSplit.Model;

namespace LiveSplit.UI.Components
{
    class AutoSplitProcess
    {
        private AutoSplitIntegrationComponent _component;
        private AutoSplitIntegrationComponentSettings _settings;
        private LiveSplitState _state;
        private TimerModel _timer;
        private int _initLine = 0;

        internal Process StartupProcess { get; }
        internal Process MainProcess { get; set; }

        private bool _isRunning = false;
        internal bool IsRunning
        {
            get => _isRunning;
            set
            {
                _component.ContextMenuControls.Clear();

                if (value || string.IsNullOrEmpty(_component.AutoSplitPath) || !File.Exists(_component.AutoSplitPath))
                    _settings.ButtonStartAutoSplit_Enabled = false;

                else
                {
                    _settings.ButtonStartAutoSplit_Enabled = true;
                    _component.ContextMenuControls.Add("Start AutoSplit", _component.StartAutoSplit);
                }

                _settings.ButtonKillAutoSplit_Enabled = _isRunning = value;

                if (value)
                    _component.ContextMenuControls.Add("Kill AutoSplit", _component.KillAutoSplit);
            }
        }

        private string _version = "N/A";
        internal string Version
        {
            get => _version;
            set => _version = _settings.LabelAutoSplitVersion_Text = value;
        }

        internal bool GameTimePausing { get; set; } = false;

        public AutoSplitProcess(AutoSplitIntegrationComponent component)
        {
            _component = component;
            _settings = _component.Settings;
            _state = _component.State;
            _timer = _component.Timer;

            try
            {
                StartupProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = _component.AutoSplitPath,
                        Arguments = "--auto-controlled",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                StartupProcess.OutputDataReceived += StartupProcess_OutputDataReceived;
                StartupProcess.ErrorDataReceived += StartupProcess_ErrorDataReceived;

                StartupProcess.Start();
                StartupProcess.BeginOutputReadLine();
                StartupProcess.BeginErrorReadLine();
                IsRunning = true;
            }

            catch (Exception e)
            {
                Console.WriteLine("Error while trying to start AutoSplit: " + e.Message);
            }
        }

        private void StartupProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            switch (_initLine)
            {
                case 0:
                    Version = e.Data;
                    _initLine++;
                    return;
                case 1:
                    MainProcess = Process.GetProcessById(int.Parse(e.Data));
                    MainProcess.EnableRaisingEvents = true;
                    MainProcess.Exited += MainProcess_Exited;
                    _initLine++;
                    return;
            }

            switch (e.Data)
            {
                case "killme":
                    Send("kill");
                    return;
                case "start":
                    _component.IgnoreNext["start"] = true;
                    _timer.Start();
                    if (_component.GameTimePausing)
                        _timer.InitializeGameTime();

                    _settings.OnStart();
                    return;
                case "split":
                    _component.IgnoreNext["split"] = true;
                    _timer.Split();
                    return;
                case "reset":
                    _component.IgnoreNext["reset"] = true;
                    _timer.Reset();
                    _settings.OnReset();
                    return;
                case "pause":
                    if (_component.GameTimePausing)
                        _state.IsGameTimePaused ^= true;
                    else
                        _timer.Pause();
                    return;
            }
        }

        private void StartupProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine("[ERROR] AutoSplit: " + e.Data);
        }

        private void MainProcess_Exited(object sender, EventArgs e)
        {
            IsRunning = false;
            Version = "N/A";
        }

        internal void Send(string command)
        {
            if (!IsRunning)
                return;

            StartupProcess.StandardInput.WriteLine(command);
            StartupProcess.StandardInput.Flush();
        }

        internal void Close()
        {
            if (!_isRunning)
                return;

            Send("kill");

            try
            {
                MainProcess.CloseMainWindow();
            }

            catch { }
        }
    }
}
