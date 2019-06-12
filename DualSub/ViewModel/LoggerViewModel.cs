using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;

namespace DualSub.ViewModel
{
    public class LoggerViewModel :ViewModelBase
    {
        public ObservableCollection<LogMessage> Logs { get; } = new ObservableCollection<LogMessage>();
        public void AddLog(string log)
        {
            Logs.Insert(0, new LogMessage { LoggedAt = DateTime.Now, Message = log });
        }
    }

    public class LogMessage
    {
        public string Message { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}
