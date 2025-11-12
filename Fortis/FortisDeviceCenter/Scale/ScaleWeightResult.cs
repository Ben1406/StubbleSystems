namespace FortisDeviceCenter.Scale;

public class ScaleWeightResult
{
    public decimal Weight { get; set; }
    public decimal TareWeight { get; set; }

    public short Decimals { get; set; }
    public bool Motion { get; set; }
    public bool Registration { get; set; }
    public bool SwingLoad { get; set; }

    public WeightType WeightType { get; set; }
    public WeightUnit WeightUnit { get; set; }
    public long? Alibi { get; set; }

    public decimal NetWeight => WeightType == WeightType.Net ? Weight : Weight - TareWeight;
    public decimal GrossWeight => WeightType == WeightType.Gross ? Weight : Weight + TareWeight;
}
