﻿namespace Infrastructure.Api;

/// <summary>
/// Hosted service basic configuration
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class HostedService : IHostedService
{
    private Task _executingTask;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Start hosted service
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Create a linked token so we can trigger cancellation outside of this token's cancellation
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Store the task we're executing
        _executingTask = ExecuteAsync(_cts.Token);

        // If the task is completed then return it
        if (_executingTask.IsCompleted)
        {
            return _executingTask;
        }

        // Otherwise it's running
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop hosted service
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start
        if (_executingTask == null)
        {
            return;
        }

        // Signal cancellation to the executing method
        _cts.Cancel();

        // Wait until the task completes or the stop token triggers
        await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

        // Throw if cancellation triggered
        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Derived classes should override this and execute a long running method until 
    /// cancellation is requested
    /// </summary>
    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
}