using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Operations.UiCatalog;

public partial class MainWindow
{
    private const uint InputMouse = 0;
    private const uint InputKeyboard = 1;
    private const uint KeyUpFlag = 0x0002;
    private const uint MouseLeftDown = 0x0002;
    private const uint MouseLeftUp = 0x0004;

    [DllImport("user32.dll", SetLastError = true)] private static extern uint SendInput(uint count, Input[] inputs, int size);
    [DllImport("user32.dll")] private static extern nint GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint window, out uint processId);
    [DllImport("user32.dll")] private static extern bool AttachThreadInput(uint attach, uint attachTo, bool attachState);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(nint window);
    [DllImport("user32.dll")] private static extern bool BringWindowToTop(nint window);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll", SetLastError = true)] private static extern bool SetCursorPos(int x, int y);

    private async Task VerifyInteractionsAsync(string outputPath)
    {
        var results = new List<string>();
        try
        {
            var window = new WindowInteropHelper(this).Handle;
            var currentThread = GetCurrentThreadId();
            var foregroundThread = GetWindowThreadProcessId(GetForegroundWindow(), out _);
            var attached = foregroundThread != 0 && foregroundThread != currentThread && AttachThreadInput(currentThread, foregroundThread, true);
            try
            {
                Show();
                Activate();
                SetForegroundWindow(window);
                BringWindowToTop(window);
                await Task.Delay(350);
                results.Add($"FOREGROUND={GetForegroundWindow()} WINDOW={window} ATTACHED={attached}");
                if (GetForegroundWindow() != window)
                    throw new InvalidOperationException("Catalog window could not become the foreground window.");

                CatalogTabs.SelectedIndex = 3;
                await Dispatcher.InvokeAsync(UpdateLayout);
                InputScrollViewer.ScrollToEnd();
                await Dispatcher.InvokeAsync(UpdateLayout);
                await Task.Delay(120);
                FocusPreviewSwitch.IsChecked = true;
                FocusPreviewSwitch.Focus();
                Keyboard.Focus(FocusPreviewSwitch);
                await Dispatcher.InvokeAsync(UpdateLayout);
                if (Keyboard.FocusedElement != FocusPreviewSwitch)
                    throw new InvalidOperationException("Switch did not receive keyboard focus.");
                await SendKeyAsync(Key.Space);
                if (FocusPreviewSwitch.IsChecked != false)
                    throw new InvalidOperationException($"Space did not toggle switch; state={FocusPreviewSwitch.IsChecked}, focus={Keyboard.FocusedElement == FocusPreviewSwitch}.");
                results.Add("PASS: Space toggled OperationSwitch.");

                await SendKeyAsync(Key.Tab);
                if (Keyboard.FocusedElement is null || Keyboard.FocusedElement == FocusPreviewSwitch)
                    throw new InvalidOperationException("Tab did not move focus to the next control.");
                var tabTarget = Keyboard.FocusedElement;
                var tabTargetName = tabTarget is Operations.UI.Controls.OperationSwitch nextSwitch
                    ? $"OperationSwitch({nextSwitch.Content})"
                    : tabTarget is FrameworkElement element && element.Name.Length > 0 ? element.Name : tabTarget.GetType().Name;
                results.Add($"PASS: Tab moved focus from OperationSwitch to {tabTargetName}.");

                CatalogTabs.SelectedIndex = 2;
                await Dispatcher.InvokeAsync(UpdateLayout);
                KeyboardDemoButton.Focus();
                Keyboard.Focus(KeyboardDemoButton);
                await Dispatcher.InvokeAsync(UpdateLayout);
                var beforeEnter = _buttonActivationCount;
                await SendKeyAsync(Key.Enter);
                if (_buttonActivationCount != beforeEnter + 1)
                    throw new InvalidOperationException("Enter did not activate OperationButton.");
                results.Add("PASS: Enter activated OperationButton.");

                var point = KeyboardDemoButton.PointToScreen(new Point(KeyboardDemoButton.ActualWidth / 2, KeyboardDemoButton.ActualHeight / 2));
                if (!SetCursorPos((int)point.X, (int)point.Y)) throw new InvalidOperationException("SetCursorPos failed.");
                await Task.Delay(150);
                var beforeClick = _buttonActivationCount;
                SendInputs(new[]
                {
                    new Input { Type = InputMouse, Union = new InputUnion { Mouse = new MouseInput { Flags = MouseLeftDown } } },
                    new Input { Type = InputMouse, Union = new InputUnion { Mouse = new MouseInput { Flags = MouseLeftUp } } }
                });
                await Task.Delay(150);
                if (_buttonActivationCount != beforeClick + 1)
                    throw new InvalidOperationException("Mouse click did not activate OperationButton.");
                results.Add("PASS: Mouse click activated OperationButton.");
            }
            finally
            {
                if (attached) AttachThreadInput(currentThread, foregroundThread, false);
            }
        }
        catch (Exception exception)
        {
            Environment.ExitCode = 1;
            results.Add($"FAIL: {exception}");
        }
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        await File.WriteAllLinesAsync(outputPath, results);
    }

    private static async Task SendKeyAsync(Key key)
    {
        var virtualKey = (ushort)KeyInterop.VirtualKeyFromKey(key);
        SendInputs(new[]
        {
            new Input { Type = InputKeyboard, Union = new InputUnion { Keyboard = new KeyboardInput { VirtualKey = virtualKey } } },
            new Input { Type = InputKeyboard, Union = new InputUnion { Keyboard = new KeyboardInput { VirtualKey = virtualKey, Flags = KeyUpFlag } } }
        });
        await Task.Delay(120);
    }

    private static void SendInputs(Input[] inputs)
    {
        if (SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Input>()) != (uint)inputs.Length)
            throw new InvalidOperationException($"SendInput failed with Win32 error {Marshal.GetLastWin32Error()}.");
    }

    [StructLayout(LayoutKind.Sequential)] private struct Input { public uint Type; public InputUnion Union; }
    [StructLayout(LayoutKind.Explicit)] private struct InputUnion
    {
        [FieldOffset(0)] public MouseInput Mouse;
        [FieldOffset(0)] public KeyboardInput Keyboard;
    }
    [StructLayout(LayoutKind.Sequential)] private struct MouseInput { public int X; public int Y; public uint Data; public uint Flags; public uint Time; public nint ExtraInfo; }
    [StructLayout(LayoutKind.Sequential)] private struct KeyboardInput { public ushort VirtualKey; public ushort ScanCode; public uint Flags; public uint Time; public nint ExtraInfo; }
}
