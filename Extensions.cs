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
using TqkLibrary.WpfUi.Converters;
using TqkLibrary.WpfUi;
using $safeprojectname$.UI.ViewModels;

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

        public static T RemoveFlag<T>(this T @enum, T flag) where T : struct, Enum
        {
            return (T)@enum.And(flag.Not());
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

        
        public static IEnumerable<EnumVM<T>> GetAll<T>(this IEnumerable<EnumVM<T>?> enumVMs) where T : Enum
        {
            foreach (var item in enumVMs)
            {
                if (item is not null)
                {
                    yield return item;
                    foreach (var child in item.Childs.GetAll())
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
