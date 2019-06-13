using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace DualSub.ViewModel
{
    public class LoggerViewModel :ViewModelBase
    {
        public ObservableCollection<LogMessage> Logs { get; } = new ObservableCollection<LogMessage>();
        public void AddLog(string log)
        {
            Logs.Insert(0, new LogMessage { LoggedAt = DateTime.Now, Message = log });
        }

        public void AddError(string log)
        {
            Logs.Insert(0, new LogMessage { LoggedAt = DateTime.Now, Message = log, Background = new SolidColorBrush(Color.FromArgb(125, 240, 128, 128))  });
        }
    }

    public class LogMessage
    {
        public string Message { get; set; }
        public DateTime LoggedAt { get; set; }
        public Brush Background { get; set; }
    }
}
