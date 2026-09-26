using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Prima.DiscordNet.Services;

namespace Prima.Tests
{
    [TestFixture]
    public class InteractionHandlingServiceTests
    {
        private const int CallerCount = 10;
        private static readonly TimeSpan RaceWindow = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(3);

        [Test]
        public async Task InitializeAsync_ConcurrentCallers_RunSubscriptionsModuleLoadingAndRegistrationOnce()
        {
            var releaseSetup = NewCompletionSource();
            var registrationStarted = NewCompletionSource();
            var releaseRegistration = NewCompletionSource();
            var subscriptionCalls = 0;
            var moduleLoadingCalls = 0;
            var registrationCalls = 0;
            var initializer = new InteractionInitializationCoordinator(
                async _ =>
                {
                    Interlocked.Increment(ref subscriptionCalls);
                    Interlocked.Increment(ref moduleLoadingCalls);
                    Thread.Sleep(RaceWindow);
                    await releaseSetup.Task.WaitAsync(TestTimeout);
                },
                async () =>
                {
                    Interlocked.Increment(ref registrationCalls);
                    registrationStarted.TrySetResult(true);
                    await releaseRegistration.Task.WaitAsync(TestTimeout);
                });

            using var callersReady = new CountdownEvent(CallerCount);
            using var startCallers = new ManualResetEventSlim();
            using var callersInvoked = new CountdownEvent(CallerCount);
            var callerTasks = StartConcurrentCallers(
                callersReady,
                startCallers,
                callersInvoked,
                () => initializer.InitializeAsync(GetType().Assembly));

            Assert.That(callersReady.Wait(TestTimeout), Is.True, "Callers did not reach the start gate.");
            startCallers.Set();
            Assert.That(callersInvoked.Wait(TestTimeout), Is.True, "Callers did not invoke initialization.");

            Assert.Multiple(() =>
            {
                Assert.That(subscriptionCalls, Is.EqualTo(1));
                Assert.That(moduleLoadingCalls, Is.EqualTo(1));
                Assert.That(registrationCalls, Is.Zero);
            });

            releaseSetup.SetResult(true);
            await registrationStarted.Task.WaitAsync(TestTimeout);

            Assert.That(registrationCalls, Is.EqualTo(1));

            releaseRegistration.SetResult(true);
            await Task.WhenAll(callerTasks).WaitAsync(TestTimeout);
            await initializer.InitializeAsync(GetType().Assembly).WaitAsync(TestTimeout);

            Assert.Multiple(() =>
            {
                Assert.That(subscriptionCalls, Is.EqualTo(1));
                Assert.That(moduleLoadingCalls, Is.EqualTo(1));
                Assert.That(registrationCalls, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task InitializeAsync_AfterRegistrationFailure_RetriesRegistrationWithoutRepeatingSetup()
        {
            var subscriptionCalls = 0;
            var moduleLoadingCalls = 0;
            var registrationCalls = 0;
            var retryStarted = NewCompletionSource();
            var releaseRetry = NewCompletionSource();
            var initializer = new InteractionInitializationCoordinator(
                _ =>
                {
                    Interlocked.Increment(ref subscriptionCalls);
                    Interlocked.Increment(ref moduleLoadingCalls);
                    return Task.CompletedTask;
                },
                async () =>
                {
                    var attempt = Interlocked.Increment(ref registrationCalls);
                    if (attempt == 1)
                    {
                        throw new InvalidOperationException("Registration failed.");
                    }

                    Thread.Sleep(RaceWindow);
                    retryStarted.TrySetResult(true);
                    await releaseRetry.Task.WaitAsync(TestTimeout);
                });

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await initializer.InitializeAsync(GetType().Assembly).WaitAsync(TestTimeout));

            using var callersReady = new CountdownEvent(CallerCount);
            using var startCallers = new ManualResetEventSlim();
            using var callersInvoked = new CountdownEvent(CallerCount);
            var retryTasks = StartConcurrentCallers(
                callersReady,
                startCallers,
                callersInvoked,
                () => initializer.InitializeAsync(GetType().Assembly));

            Assert.That(callersReady.Wait(TestTimeout), Is.True, "Retry callers did not reach the start gate.");
            startCallers.Set();
            Assert.That(callersInvoked.Wait(TestTimeout), Is.True, "Retry callers did not invoke initialization.");
            await retryStarted.Task.WaitAsync(TestTimeout);

            Assert.Multiple(() =>
            {
                Assert.That(subscriptionCalls, Is.EqualTo(1));
                Assert.That(moduleLoadingCalls, Is.EqualTo(1));
                Assert.That(registrationCalls, Is.EqualTo(2));
            });

            releaseRetry.SetResult(true);
            await Task.WhenAll(retryTasks).WaitAsync(TestTimeout);
        }

        private static Task[] StartConcurrentCallers(
            CountdownEvent callersReady,
            ManualResetEventSlim startCallers,
            CountdownEvent callersInvoked,
            Func<Task> initialize)
        {
            return Enumerable.Range(0, CallerCount)
                .Select(_ => Task.Factory.StartNew(
                        async () =>
                        {
                            callersReady.Signal();
                            if (!startCallers.Wait(TestTimeout))
                            {
                                throw new TimeoutException("The caller start gate was not released.");
                            }

                            var initialization = initialize();
                            callersInvoked.Signal();
                            await initialization.WaitAsync(TestTimeout);
                        },
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default)
                    .Unwrap())
                .ToArray();
        }

        private static TaskCompletionSource<bool> NewCompletionSource()
        {
            return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
