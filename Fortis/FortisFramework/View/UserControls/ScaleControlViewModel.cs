using FortisDeviceCenter.Scale;
using System.ComponentModel;

namespace FortisFramework.View.UserControls;

public class ScaleControlViewModel : INotifyPropertyChanged
{
    private decimal _netWeight = 0;
    public decimal NetWeight
    {
        get { return _netWeight; }
        set
        {
            _netWeight = value;
            NotifyPropertyChanged("NetWeight");
            SetLimitLights();
        }
    }

    private decimal _tareWeight = 0;
    public decimal TareWeight
    {
        get { return _tareWeight; }
        set
        {
            _tareWeight = value;
            NotifyPropertyChanged("TareWeight");
            SetLimitLights();
        }
    }

    private decimal _grossWeight = 0;
    public decimal GrossWeight
    {
        get { return _grossWeight; }
        set
        {
            _grossWeight = value;
            NotifyPropertyChanged("GrossWeight");
            SetLimitLights();
        }
    }

    private short _decimals = 3;
    public short Decimals
    {
        get { return _decimals; }
        set
        {
            _decimals = value;

            switch (_decimals)
            {
                case 0:
                    DecimalAsFormatString = "N0";
                    break;
                case 1:
                    DecimalAsFormatString = "N1";
                    break;
                case 2:
                    DecimalAsFormatString = "N2";
                    break;
                case 3:
                    DecimalAsFormatString = "N3";
                    break;
            }

            NotifyPropertyChanged("Decimals");
        }
    }

    private bool _motion = false;
    public bool Motion
    {
        get { return _motion; }
        set
        {
            _motion = value;

            switch (_motion)
            {
                case true:
                    MotionAsString = "MOTION";
                    break;
                case false:
                    MotionAsString = "";
                    break;
            }

            NotifyPropertyChanged("Motion");
        }
    }

    private WeightType _weightType = WeightType.Net;
    public WeightType WeightType
    {
        get { return _weightType; }
        set
        {
            _weightType = value;

            switch (_weightType)
            {
                case WeightType.Net:
                    WeightTypeAsString = "NET";
                    break;
                case WeightType.Gross:
                    WeightTypeAsString = "B/G";
                    break;
            }

            NotifyPropertyChanged("WeightType");
            SetLimitLights();
        }
    }

    private WeightUnit _weightUnit = WeightUnit.Kilogram;
    public WeightUnit WeightUnit
    {
        get { return _weightUnit; }
        set
        {
            _weightUnit = value;

            switch (_weightUnit)
            {
                case WeightUnit.Ton:
                    WeightUnitAsString = "TN";
                    break;
                case WeightUnit.Kilogram:
                    WeightUnitAsString = "KG";
                    break;
                case WeightUnit.Gram:
                    WeightUnitAsString = "GR";
                    break;
                case WeightUnit.Pound:
                    WeightUnitAsString = "LB";
                    break;
                case WeightUnit.Ounce:
                    WeightUnitAsString = "OZ";
                    break;
            }

            NotifyPropertyChanged("WeightUnit");
            SetLimitLights();
        }
    }

    private decimal _lowerLimitNetWeight = 0;
    public decimal LowerLimitNetWeight
    {
        get { return _lowerLimitNetWeight; }
        set
        {
            _lowerLimitNetWeight = value;
            NotifyPropertyChanged("LowerLimitNetWeight");
            SetLimitLights();
        }
    }

    private decimal _upperLimitNetWeight = 0;
    public decimal UpperLimitNetWeight
    {
        get { return _upperLimitNetWeight; }
        set
        {
            _upperLimitNetWeight = value;
            NotifyPropertyChanged("UpperLimitNetWeight");
            SetLimitLights();
        }
    }

    private decimal _targetNetWeight = 0;
    public decimal TargetNetWeight
    {
        get { return _targetNetWeight; }
        set
        {
            _targetNetWeight = value;
            NotifyPropertyChanged("TargetNetWeight");
            SetLimitLights();
        }
    }

    private string _decimalAsFormatString = "N3";
    public string DecimalAsFormatString
    {
        get { return _decimalAsFormatString; }
        set
        {
            _decimalAsFormatString = value;
            NotifyPropertyChanged("DecimalAsFormatString");
            SetLimitLights();
        }
    }

    private string _motionAsString = "";
    public string MotionAsString
    {
        get { return _motionAsString; }
        set
        {
            _motionAsString = value;
            NotifyPropertyChanged("MotionAsString");
        }
    }

    private string _weightTypeAsString = "NET";
    public string WeightTypeAsString
    {
        get { return _weightTypeAsString; }
        set
        {
            _weightTypeAsString = value;
            NotifyPropertyChanged("WeightTypeAsString");
            SetLimitLights();
        }
    }

    private string _weightUnitAsString = "KG";
    public string WeightUnitAsString
    {
        get { return _weightUnitAsString; }
        set
        {
            _weightUnitAsString = value;
            NotifyPropertyChanged("WeightUnitAsString");
            SetLimitLights();
        }
    }

    public void SetLimitLights()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
