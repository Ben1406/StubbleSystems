using System.IO.Ports;

namespace FortisCommunication.Serial;

public enum SerialPortName
{
    Com1 = 1,
    Com2 = 2,
    Com3 = 3,
    Com4 = 4,
    Com5 = 5,
    Com6 = 6,
    Com7 = 7,
    Com8 = 8,
    Com9 = 9,
    Com10 = 10,
    Com11 = 11,
    Com12 = 12,
    Com13 = 13,
    Com14 = 14,
    Com15 = 15,
    Com16 = 16,
    Com17 = 17,
    Com18 = 18,
    Com19 = 19,
    Com20 = 20,
    Com21 = 21,
    Com22 = 22,
    Com23 = 23,
    Com24 = 24,
    Com25 = 25,
    Com26 = 26,
    Com27 = 27,
    Com28 = 28,
    Com29 = 29,
    Com30 = 30,
    Com31 = 31,
    Com32 = 32
}

public enum SerialBaudRate
{
    Rate1200 = 1200,
    Rate2400 = 2400,
    Rate4800 = 4800,
    Rate9600 = 9600,
    Rate19200 = 19200,
    Rate38400 = 38400,
    Rate57600 = 57600,
    Rate115200 = 115200
}

public enum SerialParity
{
    None = Parity.None,
    Odd = Parity.Odd,
    Even = Parity.Even,
    Mark = Parity.Mark,
    Space = Parity.Space
}

public enum SerialDataBit
{
    Seven = 7,
    Eight = 8
}

public enum SerialStopBit
{
    None = StopBits.None,
    One = StopBits.One,
    Two = StopBits.Two,
    OnePointFive = StopBits.OnePointFive
}
