namespace Plugin.Scanner.Android.Exceptions;

internal class ActivityMustBeILifecycleOwnerException : Exception
{
    public ActivityMustBeILifecycleOwnerException(string message)
        : base(message)
    {
    }

    public ActivityMustBeILifecycleOwnerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ActivityMustBeILifecycleOwnerException()
    {
    }
}
