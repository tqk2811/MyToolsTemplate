﻿using Microsoft.Extensions.Logging;
using $safeprojectname$.DataClass;
using $safeprojectname$.UI.ViewModels.Commands;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.UI.ViewModels
{
    internal class BaseVM : BaseViewModel
    {
        protected readonly ILogger _logger;
        public BaseVM()
        {
            _logger = Singleton.ILoggerFactory.CreateLogger(this.GetType());
            this.CopyTextCommand = new BaseCommand<string>(_CopyTextCommand);
            this.LinkNavigateCommand = new BaseCommand<string>(_LinkNavigateCommand);
        }
        
        protected SettingData Setting { get { return Singleton.Setting.Data; } }
        public virtual void SaveSetting() => Singleton.Setting.TriggerSave();

        Cursor? _Cursor = null;
        public Cursor? Cursor
        {
            get { return _Cursor; }
            set { _Cursor = value; NotifyPropertyChange(); }
        }

        Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { _Visibility = value; NotifyPropertyChange(); }
        }

        public virtual void RefreshAll()
        {
            foreach (var item in GetType().GetProperties())
            {
                NotifyPropertyChange(item.Name);
            }
        }

        public BaseCommand<string> CopyTextCommand { get; }
        void _CopyTextCommand(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    Clipboard.SetText(text);
                }
                catch (Exception ex)
                {
                    _logger.LogErrorFunction(ex);
                }
            }
        }

        public BaseCommand<string> LinkNavigateCommand { get; }
        void _LinkNavigateCommand(string link)
        {
            if (Uri.TryCreate(link, UriKind.Absolute, out Uri? uri))
            {
                try
                {
                    using Process? process = Process.Start(new ProcessStartInfo()
                    {
                        FileName = uri.ToString(),
                        UseShellExecute = true,
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogErrorFunction(ex);
                }
            }
        }
    }
}
