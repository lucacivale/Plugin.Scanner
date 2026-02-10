namespace Plugin.Scanner.Android;

public interface ICurrentActivity
{
    Func<Activity?> GetActivity { get; }
}