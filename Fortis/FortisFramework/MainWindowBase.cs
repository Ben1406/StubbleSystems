using System.Windows;

namespace FortisFramework;

public class MainWindowBase : Window
{
    public void InitializeWindow()
    {
    }

    public SystemTheme Theme { get; set; } = null!;
}
