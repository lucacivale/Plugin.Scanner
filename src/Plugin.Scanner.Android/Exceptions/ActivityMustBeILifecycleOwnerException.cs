namespace Plugin.Scanner.Android.Exceptions;

/// <summary>
/// Exception thrown when an Activity does not implement ILifecycleOwner as required by the camera controller.
/// </summary>
public sealed class ActivityMustBeILifecycleOwnerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityMustBeILifecycleOwnerException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityMustBeILifecycleOwnerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityMustBeILifecycleOwnerException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityMustBeILifecycleOwnerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityMustBeILifecycleOwnerException"/> class.
    /// </summary>
    public ActivityMustBeILifecycleOwnerException()
    {
    }
}
