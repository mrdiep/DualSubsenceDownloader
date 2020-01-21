using System;
using GalaSoft.MvvmLight;

namespace DualSub.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        private string topFontSize = "13";
        private string topFontColor = "000000";
        private string topFontColorOutline = "ffffff";
        private string bottomFontSize = "16";
        private string bottomFontColor = "000000";
        private string bottomFontColorOutline = "ffffff";
        private string filter = "264 1080";
        public string _plexServer = "http://192.168.1.2:32400";
        public string _librarySection = "1";
        public string _plexToken = "pPbFPN1_EQ1RKLV8xb3j";

        private Action<string> changeValue;

        public string TopFontSize { get => topFontSize; set => Set(ref topFontSize, value); }
        public string TopFontColor { get => topFontColor; set => Set(ref topFontColor, value); }
        public string TopFontColorOutline { get => topFontColorOutline; set => Set(ref topFontColorOutline, value); }

        public string BottomFontSize { get => bottomFontSize; set => Set(ref bottomFontSize, value); }
        public string BottomFontColor { get => bottomFontColor; set => Set(ref bottomFontColor, value); }
        public string BottomFontColorOutline { get => bottomFontColorOutline; set => Set(ref bottomFontColorOutline, value); }

        public string TagsFilter { get => filter; set { Set(ref filter, value); changeValue?.Invoke(value); } }


        public string PlexServer { get => _plexServer; set => Set(ref _plexServer, value); }
        public string LibrarySection { get => _librarySection; set => Set(ref _librarySection, value); }
        public string PlexToken { get => _plexToken; set => Set(ref _plexToken, value); }


        public void SubcribeChanged(Action<string> changeValue)
        {
            this.changeValue = changeValue;
        }
    }
}
