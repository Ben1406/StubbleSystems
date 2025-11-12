using System.ComponentModel;
using System.Windows.Media;

namespace FortisFramework;

public class SystemTheme : INotifyPropertyChanged
{
    public string Title { get; set; } = "No Name";

    private Enums.ColorTheme _colorTheme = Enums.ColorTheme.Light;
    public Enums.ColorTheme ColorTheme
    {
        get => _colorTheme;
        set
        {
            _colorTheme = value;

            switch (_colorTheme)
            {
                case Enums.ColorTheme.Dark:
                    BackgroundColor = Brushes.Black;
                    ForegroundColor = Brushes.White;
                    BorderColor = Brushes.White;
                    break;
                case Enums.ColorTheme.Light:
                    BackgroundColor = Brushes.White;
                    ForegroundColor = Brushes.Black;
                    BorderColor = Brushes.Black;
                    break;
                default:
                    BackgroundColor = Brushes.White;
                    ForegroundColor = Brushes.Black;
                    BorderColor = Brushes.Black;
                    break;
            }
            OnPropertyChanged(nameof(ColorTheme));
        }
    }

    private SolidColorBrush _backgroundColor = Brushes.White;
    public SolidColorBrush BackgroundColor
    {
        get => _backgroundColor;
        private set
        {
            _backgroundColor = value;
            OnPropertyChanged(nameof(BackgroundColor));
        }
    }

    private SolidColorBrush _foregroundColor = Brushes.Black;
    public SolidColorBrush ForegroundColor
    {
        get => _foregroundColor;
        private set
        {
            _foregroundColor = value;
            OnPropertyChanged(nameof(ForegroundColor));
        }
    }

    private SolidColorBrush _borderColor = Brushes.Black;
    public SolidColorBrush BorderColor
    {
        get => _borderColor;
        private set
        {
            _borderColor = value;
            OnPropertyChanged(nameof(BorderColor));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler? handler = PropertyChanged;

        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
