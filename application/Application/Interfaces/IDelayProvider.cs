namespace KubeKataApp.Application.Interfaces;

public interface IDelayProvider
{
    int CurrentDelayMs { get; }
    void SetDelay(int milliseconds);
}

public class DelayProvider : IDelayProvider
{
    public int CurrentDelayMs { get; private set; } = 0;

    public void SetDelay(int milliseconds)
    {
        CurrentDelayMs = milliseconds;
    }
}
