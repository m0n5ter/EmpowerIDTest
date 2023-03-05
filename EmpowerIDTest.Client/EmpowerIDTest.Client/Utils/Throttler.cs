using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace EmpowerIDTest.Client.Utils;

public class Throttler : Throttler<string>
{
    public Task Next(int delay, Action<string?, CancellationToken> action, [CallerMemberName] string? param = null) => Next(delay, param, action);

    public Task Next(int delay, Func<string?, CancellationToken, Task> action, [CallerMemberName] string? param = null) => Next(delay, param, action);
}

public class Throttler<T>
{
    private readonly struct TaskDesc
    {
        public readonly Func<T?, CancellationToken, Task> Action;
        public readonly CancellationTokenSource Cancellation;
        public readonly T? Param;
        public readonly DateTime ExecutionTime;

        public TaskDesc(int delay, T? param, Func<T?, CancellationToken, Task> action) : this()
        {
            Action = action;
            Param = param;
            ExecutionTime = DateTime.Now.AddMilliseconds(delay);
            Cancellation = new CancellationTokenSource();
        }
    }

    private Task? _governorTask;
    private TaskDesc? _currentTask;
    private TaskDesc? _nextTask;

    public Task Next(int delay, T? param, Action<T?, CancellationToken> action) => Next(delay, param, (p, ct) => Task.Run(() =>
    {
        if (ct.IsCancellationRequested)
        {
            return;
        }

        try
        {
            action(p, ct);
        }
        catch (Exception exception)
        {
            Dispatcher.UIThread.Post(() => throw exception);
        }
    }, ct));

    public Task Next(int delay, T? param, Func<T?, CancellationToken, Task> action)
    {
        lock (this)
        {
            var taskDesc = new TaskDesc(delay, param, action);

            if (_currentTask == null)
            {
                _currentTask = taskDesc;
            }
            else
            {
                _currentTask.Value.Cancellation.Cancel();
                _nextTask = taskDesc;
            }

            return _governorTask ??= Task.Run(async () =>
            {
                while (true)
                {
                    TaskDesc? currentTask;

                    lock (this)
                    {
                        currentTask = _currentTask;
                    }

                    if (currentTask == null)
                    {
                        break;
                    }

                    while (currentTask.Value.ExecutionTime > DateTime.Now)
                    {
                        await Task.Delay(1).ConfigureAwait(false);

                        if (currentTask.Value.Cancellation.IsCancellationRequested)
                        {
                            break;
                        }
                    }

                    if (!currentTask.Value.Cancellation.IsCancellationRequested)
                    {
                        try
                        {
                            await currentTask.Value.Action(currentTask.Value.Param, currentTask.Value.Cancellation.Token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception exception)
                        {
                            Dispatcher.UIThread.Post(() => throw new ApplicationException("Throttled task threw exception", exception));
                        }
                    }

                    lock (this)
                    {
                        _currentTask = _nextTask;
                        _nextTask = null;
                    }
                }

                _governorTask = null;
            });
        }
    }
}
