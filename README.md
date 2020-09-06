# AutoSplit Integration

Directly connects [AutoSplit](https://github.com/KaDiWa4/Auto-Split) with [LiveSplit](https://github.com/LiveSplit/LiveSplit).

## Installation

- Close AutoSplit if it’s currently open
- Download [the `.dll` file](/update/LiveSplit.AutoSplitIntegration.dll?raw=true) and move it into LiveSplit’s `Components` folder
- Edit your layout, click the plus and choose Control → AutoSplit Integration
- Click Layout Settings → AutoSplit Integration → Browse... and select your `AutoSplit.exe`. You can find compatible AutoSplit versions [here](https://github.com/KaDiWa4/Auto-Split)

## Opening/Closing AutoSplit

- To reopen AutoSplit if you accidentally closed it, right click LiveSplit and choose Control → Start AutoSplit.
- If you want to close AutoSplit, just close its window. Alternatively, if AutoSplit is for some reason not responding, you can kill it in the same menu where you can reopen it. This way your settings won’t be saved though.
- You can also find the options to reopen/kill AutoSplit in the component’s settings. When you close the settings again, make sure to use “OK” to close the windows, not “Cancel” as this might close AutoSplit.

## Compiling

- Clone/download LiveSplit and this repository
- Open it in [Visual Studio 2019](https://visualstudio.microsoft.com/vs)
- Add `LiveSplit.exe`, `LiveSplit.Core.dll` and `UpdateManager` as assembly references

## Resources
- Still need help? [Open an issue](../../issues)
- Join the [AutoSplit Discord](https://discord.gg/Qcbxv9y)
