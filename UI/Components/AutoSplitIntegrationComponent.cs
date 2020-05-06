using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;

namespace LiveSplit.UI.Components
{
    public class AutoSplitIntegrationComponent : IComponent
    {
        internal AutoSplitProcess AutoSplit { get; set; }

        internal bool GameTimePausing { get; set; } = false;

        internal AutoSplitIntegrationComponentSettings Settings { get; set; }

        internal LiveSplitState State { get; set; }

        internal TimerModel Timer { get; set; }

        internal Dictionary<string, bool> IgnoreNext { get; set; } = new Dictionary<string, bool>()
        {
            { "start", false },
            { "split", false },
            { "reset", false }
        };

        public string ComponentName =>
            "AutoSplit Integration";

        public IDictionary<string, Action> ContextMenuControls { get; protected set; } = new Dictionary<string, Action>();

        private string _AutoSplitPath;
        public string AutoSplitPath
        {
            get => _AutoSplitPath;

            set
            {
                _AutoSplitPath = value;

                if (AutoSplit != null)
                    AutoSplit.Close();

                StartAutoSplit();
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
        public void DrawVertical(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
        public float HorizontalWidth => 0;
        public float MinimumHeight => 0;
        public float VerticalHeight => 0;
        public float MinimumWidth => 0;
        public float PaddingTop => 0;
        public float PaddingBottom => 0;
        public float PaddingLeft => 0;
        public float PaddingRight => 0;

        public AutoSplitIntegrationComponent(LiveSplitState state)
        {
            State = state;
            Timer = new TimerModel() { CurrentState = State };

            Settings = new AutoSplitIntegrationComponentSettings(this);

            State.OnStart += State_OnStart;
            State.OnSplit += State_OnSplit;
            State.OnReset += State_OnReset;
            State.OnSkipSplit += State_OnSkipSplit;
            State.OnUndoSplit += State_OnUndoSplit;
        }

        public Control GetSettingsControl(LayoutMode mode) =>
            Settings;

        public XmlNode GetSettings(XmlDocument document) =>
            Settings.GetSettings(document);

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);

            StartAutoSplit();
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

        public delegate void ContextMenuDelgate();

        public void StartAutoSplit()
        {
            if (string.IsNullOrEmpty(_AutoSplitPath) || !File.Exists(_AutoSplitPath) || (AutoSplit != null && AutoSplit.IsRunning))
                return;

            AutoSplit = new AutoSplitProcess(this);
        }

        public void KillAutoSplit()
        {
            if (!AutoSplit.IsRunning)
                return;

            try
            {
                AutoSplit.MainProcess.Kill();
            }

            catch { }
        }

        public void Dispose()
        {
            if (AutoSplit != null)
                AutoSplit.Close();
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            if (IgnoreNext["start"] == true)
            {
                IgnoreNext["start"] = false;
                return;
            }

            AutoSplit.Send("start");
            if (GameTimePausing)
                Timer.InitializeGameTime();

            Settings.OnStart();
        }

        private void State_OnSplit(object sender, EventArgs e)
        {
            if (IgnoreNext["split"] == true)
            {
                IgnoreNext["split"] = false;
                return;
            }

            AutoSplit.Send("split");
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
            if (IgnoreNext["reset"] == true)
            {
                IgnoreNext["reset"] = false;
                return;
            }

            AutoSplit.Send("reset");
            Settings.OnReset();
        }

        private void State_OnSkipSplit(object sender, EventArgs e) =>
            AutoSplit.Send("skip");

        private void State_OnUndoSplit(object sender, EventArgs e) =>
            AutoSplit.Send("undo");
    }
}