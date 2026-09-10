using System.Windows;
using System.Windows.Controls;
using Winvim.Automation;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using Key = System.Windows.Input.Key;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Keyboard = System.Windows.Input.Keyboard;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace Winvim.Overlay;

/// <summary>
/// Full-screen transparent window that draws Vimium-style hint labels over every
/// clickable element found in the foreground window, and lets the user type a
/// hint's code to invoke it. Escape cancels; Backspace edits the typed code.
/// </summary>
public partial class HintOverlayWindow : Window
{
    private sealed class HintVisual
    {
        public required string Code { get; init; }
        public required ClickableElement Target { get; init; }
        public required Border Label { get; init; }
    }

    private readonly IReadOnlyList<ClickableElement> _elements;
    private readonly List<HintVisual> _hints = new();
    private string _typed = "";

    public HintOverlayWindow(IReadOnlyList<ClickableElement> elements)
    {
        InitializeComponent();
        _elements = elements;

        // The process is DPI-unaware (see app.manifest), so these virtual-screen
        // values and the physical-pixel bounds UI Automation reports share one
        // coordinate space — no per-monitor scaling math needed.
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.VirtualScreenWidth;
        Height = SystemParameters.VirtualScreenHeight;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var codes = HintCodeGenerator.Generate(_elements.Count);

        for (int i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            var code = codes[i];

            var text = new TextBlock
            {
                Text = code.ToUpperInvariant(),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Padding = new Thickness(3, 1, 3, 1)
            };

            var label = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xF2, 0x85)),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = text
            };

            Canvas.SetLeft(label, element.ScreenBounds.X - Left);
            Canvas.SetTop(label, element.ScreenBounds.Y - Top);
            HintCanvas.Children.Add(label);

            _hints.Add(new HintVisual { Code = code, Target = element, Label = label });
        }

        Keyboard.Focus(this);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            Close();
            return;
        }

        if (e.Key == Key.Back)
        {
            if (_typed.Length > 0)
            {
                _typed = _typed[..^1];
                ApplyFilter();
            }
            return;
        }

        char? typedChar = KeyToChar(e.Key);
        if (typedChar is null) return;

        _typed += typedChar;
        var matches = ApplyFilter();

        if (matches.Count == 1)
        {
            var target = matches[0].Target;
            Close();
            ElementInvoker.Invoke(target);
        }
        else if (matches.Count == 0)
        {
            Close();
        }
    }

    private List<HintVisual> ApplyFilter()
    {
        var matches = new List<HintVisual>();

        foreach (var hint in _hints)
        {
            bool isMatch = hint.Code.StartsWith(_typed, StringComparison.OrdinalIgnoreCase);
            hint.Label.Visibility = isMatch ? Visibility.Visible : Visibility.Collapsed;
            if (isMatch) matches.Add(hint);
        }

        return matches;
    }

    private static char? KeyToChar(Key key) =>
        key is >= Key.A and <= Key.Z ? (char)('a' + (key - Key.A)) : null;
}
