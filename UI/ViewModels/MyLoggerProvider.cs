﻿using Microsoft.Extensions.Logging;
using System;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollections;

namespace $safeprojectname$.UI.ViewModels
{
    class MyLoggerProviderVM : LimitObservableCollection<string>, ILoggerProvider
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public MyLoggerProviderVM(Func<string> delegatePath) : base(delegatePath)
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, this);
        }

        public void Dispose()
        {
        }


        class Logger : ILogger
        {
            readonly string _categoryName;
            readonly MyLoggerProviderVM _myLoggerProvider;
            public Logger(string categoryName, MyLoggerProviderVM myLoggerProvider)
            {
                _categoryName = categoryName;
                _myLoggerProvider = myLoggerProvider;
            }


            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= _myLoggerProvider.LogLevel;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                string time = DateTime.Now.ToString("HH:mm:ss");
                if (exception is not null)
                {
                    _myLoggerProvider.AddAsync($"{time} [{logLevel,-12}] {_categoryName} {state} - {exception.GetType().FullName}: {exception.Message}\r\n{exception.StackTrace}");
                }
                else
                {
                    _myLoggerProvider.AddAsync($"{time} [{logLevel,-12}] {_categoryName} - {formatter(state, exception)}");
                }
            }
        }
    }
}
