using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;

namespace LiveSplit.UI.Components
{
    public partial class AutoSplitIntegrationComponentSettings : UserControl
    {
        private AutoSplitIntegrationComponent _component;
        private LiveSplitState _state;

        internal string LabelAutoSplitVersion_Text
        {
            set
            {
                try
                {
                    if (labelAutoSplitVersion.Text.Substring(19) == value)
                        return;

                    if (labelAutoSplitVersion.InvokeRequired)
                        Invoke(new _setPropertyDelegate(() =>
                        {
                            labelAutoSplitVersion.Text = "AutoSplit version: " + value;
                        }));
                    else
                        labelAutoSplitVersion.Text = "AutoSplit version: " + value;
                }

                catch { }
            }
        }

        internal bool ButtonStartAutoSplit_Enabled
        {
            set
            {
                try
                {
                    if (buttonStartAutoSplit.Enabled == value)
                        return;

                    if (buttonStartAutoSplit.InvokeRequired)
                    {
                        Invoke(new _setPropertyDelegate(() =>
                        {
                            buttonStartAutoSplit.Enabled = value;
                        }));
                    }

                    else
                        buttonStartAutoSplit.Enabled = value;
                }

                catch { }
            }
        }

        internal bool ButtonKillAutoSplit_Enabled
        {
            set
            {
                try
                {
                    if (buttonKillAutoSplit.Enabled == value)
                        return;

                    if (buttonStartAutoSplit.InvokeRequired)
                    {
                        Invoke(new _setPropertyDelegate(() =>
                        {
                            buttonKillAutoSplit.Enabled = value;
                        }));
                    }

                    else
                        buttonKillAutoSplit.Enabled = value;
                }

                catch { }
            }
        }

        internal AutoSplitIntegrationComponentSettings(AutoSplitIntegrationComponent component)
        {
            _component = component;
            _state = _component.State;

            InitializeComponent();

            checkBoxGameTimePausing.Enabled = _state.CurrentPhase == TimerPhase.NotRunning;
        }

        internal void OnStart() =>
            checkBoxGameTimePausing.Enabled = false;

        internal void OnReset() =>
            checkBoxGameTimePausing.Enabled = true;

        internal XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settings_element = document.CreateElement("Settings");

            SettingsHelper.CreateSetting(document, settings_element, "Version", "1.8");
            SettingsHelper.CreateSetting(document, settings_element, "AutoSplitPath", _component.AutoSplitPath);
            SettingsHelper.CreateSetting(document, settings_element, "GameTimePausing", _component.GameTimePausing);

            return settings_element;
        }

        internal void SetSettings(XmlNode settings)
        {
            if (((XmlElement)settings).IsEmpty)
                return;

            _component.AutoSplitPath = textBoxAutoSplitPath.Text = SettingsHelper.ParseString(settings["AutoSplitPath"]);
            _component.GameTimePausing = checkBoxGameTimePausing.Checked = SettingsHelper.ParseBool(settings["GameTimePausing"]);

            if (_component.AutoSplit != null)
            {
                LabelAutoSplitVersion_Text = _component.AutoSplit.Version;
                _component.AutoSplit.IsRunning = _component.AutoSplit.IsRunning;
            }
        }

        private void buttonAutoSplitPathBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "AutoSplit.exe|*.exe"
            };

            if (File.Exists(_component.AutoSplitPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(_component.AutoSplitPath);
                dialog.FileName = Path.GetFileName(_component.AutoSplitPath);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
                _component.AutoSplitPath = textBoxAutoSplitPath.Text = dialog.FileName;
        }

        private void buttonStartAutoSplit_Click(object sender, EventArgs e) =>
            _component.StartAutoSplit();

        private void buttonKillAutoSplit_Click(object sender, EventArgs e) =>
            _component.KillAutoSplit();

        private void checkBoxGameTimePausing_CheckedChanged(object sender, EventArgs e) =>
            _component.GameTimePausing = checkBoxGameTimePausing.Checked;

        private void textBoxAutoSplitPath_TextChanged(object sender, EventArgs e) =>
            _component.AutoSplitPath = textBoxAutoSplitPath.Text;

        private delegate void _setPropertyDelegate();
    }
}