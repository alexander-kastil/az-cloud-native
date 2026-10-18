namespace TelemetryIngest;

public class ConnectedClients
{
    private int count;

    public int Count => Volatile.Read(ref count);

    public void Connected() => Interlocked.Increment(ref count);

    public void Disconnected() => Interlocked.Decrement(ref count);

    public async Task WaitForAnyAsync(CancellationToken cancellationToken)
    {
        while (Count == 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }
    }
}
