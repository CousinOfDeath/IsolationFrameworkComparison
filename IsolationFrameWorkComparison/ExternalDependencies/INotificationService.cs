using System;

namespace IsolationFrameWorkComparison.ExternalDependencies
{
    public interface INotificationService
    {
        bool Notify(Uri uri, string message);
    }
}