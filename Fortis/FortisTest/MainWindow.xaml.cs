using System.Windows;
using Autofac;
using FortisDeviceCenter;
using FortisDeviceCenter.Scale;
using FortisDeviceCenter.BarcodeScanner;
using FortisFramework;
using FortisFramework.Utilities;
using System.Windows.Media;

namespace FortisTest;

public partial class MainWindow : MainWindowBase
{
    private readonly IDeviceCenter _deviceCenter;

    public MainWindow()
    {
        InitializeComponent();
        InitializeWindow();

        Theme = new SystemTheme
        {
            Title = "Benny",
            ColorTheme = Enums.ColorTheme.Light
        };

        var systemSettings = new SystemSetting
        {
            TerminalName = "BennyVision",
            MsSqlServerName = "BennyVision",
            MsSqlDatabaseName = "FortisDB",
            MsSqlTrustServerCertificateEnabled = true,
            MsSqlAuthenticationType = MsSqlAuthenticationType.WindowsAuthentication,
            MsSqlConnectionSecurityEncryption = MsSqlConnectionSecurityEncryption.Optional
        };

        var @base = new FortisFrameworkBase(systemSettings);
        @base.Initialize();

        _deviceCenter = @base.BaseContainer.Resolve<IDeviceCenter>();

        _deviceCenter.ScaleWeightResult += OnScaleWeightResult;
        _deviceCenter.BarcodeScannerResult += OnBarcodeScannerResult;
        _deviceCenter.ReceivedData += OnDeviceCenter_ReceivedData;
        _deviceCenter.TransmittedData += OnDeviceCenter_TransmittedData;
        _deviceCenter.Messages += OnDeviceCenter_Messages;
    }

    public string Color { get; set; } = "White";

    private void OnScaleWeightResult(object? sender, ScaleWeightResultEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            var deviceName = e.DeviceName;
            var deviceId = e.ClientDeviceId;
            var scaleWeightResult = e.ScaleWeightResult;
            LblScaleWeight.Content = $"'{deviceName}'-'{deviceId}' ... Net > {scaleWeightResult.NetWeight:F3}     Tare > {scaleWeightResult.TareWeight:F3}     Gross > {scaleWeightResult.GrossWeight:F3}     {scaleWeightResult.WeightUnit}";
        });
    }

    private void OnBarcodeScannerResult(object? sender, BarcodeScannerResultEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            var deviceName = e.DeviceName;
            var deviceId = e.ClientDeviceId;
            var barcodeScannerResult = e.BarcodeScannerResult;
            LblBarcodeScanner.Content = $"'{deviceName}'-'{deviceId}' ... Barcode > {Helper.ConvertBytesToStringTags(barcodeScannerResult.Barcode ?? "")}     Length > {barcodeScannerResult.Length}";
        });
    }

    private void OnDeviceCenter_ReceivedData(object? sender, DeviceCenterDataEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            LblLog1.Content = "Rx > " + Helper.ConvertBytesToStringTags(e.Text ?? "");
        });
    }

    private void OnDeviceCenter_TransmittedData(object? sender, DeviceCenterDataEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            LblLog1.Content = "Tx > " + Helper.ConvertBytesToStringTags(e.Text ?? "");
        });
    }

    private void OnDeviceCenter_Messages(object? sender, DeviceCenterMessageEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            LblLog2.Content = Helper.ConvertBytesToStringTags(e.Text ?? "");
        });
    }

    private void ButtonTest_Click(object sender, RoutedEventArgs e)
    {
        _deviceCenter.TransmitData("Typeless", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
    }

    private void ButtonWeight_Click(object sender, RoutedEventArgs e)
    {
        var scaleWeightResult = new ScaleWeightResult
        {
            Weight = 12.345m,
            TareWeight = 1.111m,
            Decimals = 3,
            Motion = false,
            Registration = true,
            WeightType = WeightType.Net,
            WeightUnit = WeightUnit.Kilogram
        };
        _deviceCenter.SimulateScaleWeightResult("ScaleSimulator1", 1, scaleWeightResult);
    }
}
