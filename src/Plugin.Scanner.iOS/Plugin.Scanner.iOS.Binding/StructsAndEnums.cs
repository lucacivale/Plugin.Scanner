using ObjCRuntime;

namespace Plugin.Scanner.iOS.Binding
{
    [Native]
    public enum QualityLevel : long
    {
        Balanced = 0,
        Fast = 1,
        Accurate = 2
    }

    [Native]
    public enum ScanningUnavailable : long
    {
        Unsupported = 0,
        CameraRestricted = 1
    }

    [Native]
    public enum TextContentType : long
    {
        Default = 0,
        DateTimeDuration = 1,
        EmailAddress = 2,
        FlightNumber = 3,
        FullStreetAddress = 4,
        ShipmentTrackingNumber = 5,
        TelephoneNumber = 6,
        Url = 7,
        Currency = 8
    }
}