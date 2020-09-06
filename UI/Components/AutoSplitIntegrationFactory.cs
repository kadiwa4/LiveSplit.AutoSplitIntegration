using System;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(AutoSplitIntegrationFactory))]

namespace LiveSplit.UI.Components
{
    public class AutoSplitIntegrationFactory : IComponentFactory
    {
        public string ComponentName => "AutoSplit Integration";

        public string Description => "Directly connects AutoSplit with LiveSplit.";

        public ComponentCategory Category => ComponentCategory.Control;

        public Version Version => Version.Parse("1.8.1");

        public string UpdateName => ComponentName;

        public string UpdateURL => "https://raw.githubusercontent.com/KaDiWa4/LiveSplit.AutoSplitIntegration/master/update/";

        public string XMLURL => "https://raw.githubusercontent.com/KaDiWa4/LiveSplit.AutoSplitIntegration/master/update/update.LiveSplit.AutoSplitIntegration.xml";

        public IComponent Create(LiveSplitState state) => new AutoSplitIntegrationComponent(state);
    }
}