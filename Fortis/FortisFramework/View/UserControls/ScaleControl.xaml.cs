using Autofac;
using FortisDeviceCenter;
using FortisDeviceCenter.Scale;
using System.Windows.Controls;

namespace FortisFramework.View.UserControls;

public partial class ScaleControl : UserControl
{
    private IDeviceCenter _deviceCenter { get; set; } = null!;

    public ScaleControlViewModel ScaleControlViewModel { get; set; }

    public int ActiveScaleId { get; set; } = 1;

    public ScaleControl()
    {
        InitializeComponent();

        ScaleControlViewModel = new ScaleControlViewModel();
        DataContext = ScaleControlViewModel;
    }

    public void Start()
    {
        if (_deviceCenter is null)
        {
            _deviceCenter = CompositionRoot.BaseContainer.Resolve<IDeviceCenter>();
        }

        _deviceCenter.ScaleWeightResult += OnScaleWeightResult;
    }

    public void Stop()
    {
        _deviceCenter.ScaleWeightResult -= OnScaleWeightResult;
    }

    public void SetLimits(decimal lowerLimitNetWeight, decimal targetNetWeight, decimal upperLimitNetWeight)
    {
        ScaleControlViewModel.LowerLimitNetWeight = lowerLimitNetWeight;
        ScaleControlViewModel.TargetNetWeight = targetNetWeight;
        ScaleControlViewModel.UpperLimitNetWeight = upperLimitNetWeight;
    }

    public void ResetLimits()
    {
        ScaleControlViewModel.LowerLimitNetWeight = 0;
        ScaleControlViewModel.TargetNetWeight = 0;
        ScaleControlViewModel.UpperLimitNetWeight = 0;
    }

    private void OnScaleWeightResult(object? sender, ScaleWeightResultEventArgs e)
    {
        if (e.ClientDeviceId == ActiveScaleId)
        {
            ScaleControlViewModel.NetWeight = e.ScaleWeightResult.NetWeight;
            ScaleControlViewModel.TareWeight = e.ScaleWeightResult.TareWeight;
            ScaleControlViewModel.GrossWeight = e.ScaleWeightResult.GrossWeight;
            ScaleControlViewModel.Decimals = e.ScaleWeightResult.Decimals;
            ScaleControlViewModel.Motion = e.ScaleWeightResult.Motion;
            ScaleControlViewModel.WeightType = e.ScaleWeightResult.WeightType;
            ScaleControlViewModel.WeightUnit = e.ScaleWeightResult.WeightUnit;
        }
    }
}
