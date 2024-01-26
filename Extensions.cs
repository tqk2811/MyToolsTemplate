using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$
{
    static class Extensions
    {
        /// <summary>
        /// A workaround for getting all of AggregateException.InnerExceptions with try/await/catch
        /// </summary>
        public static Task WithAggregatedExceptions(this Task @this)
        {
            // using AggregateException.Flatten as a bonus
            return @this.ContinueWith(
                continuationFunction: anteTask =>
                    anteTask.IsFaulted &&
                    anteTask.Exception is AggregateException ex &&
                    (ex.InnerExceptions.Count > 1 || ex.InnerException is AggregateException) ?
                    Task.FromException(ex.Flatten()) : anteTask,
                cancellationToken: CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler: TaskScheduler.Default).Unwrap();
        }


        public static void ShowMessageBox(this Exception ex, bool isShowStackTrace = false)
        {
            string message = ex.Message;
            if (isShowStackTrace) message += "\r\n" + ex.StackTrace;
            MessageBox.Show(message, ex.GetType().FullName);
        }
        public static void LogErrorFunction(this ILogger? logger, Exception ex, [CallerMemberName] string? functionName = "")
        {
            logger?.LogError(ex, functionName);
        }
    }
}
