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
        private Action<string> changeValue;

        public string TopFontSize { get => topFontSize; set => Set(ref topFontSize, value); }
        public string TopFontColor { get => topFontColor; set => Set(ref topFontColor, value); }
        public string TopFontColorOutline { get => topFontColorOutline; set => Set(ref topFontColorOutline, value); }

        public string BottomFontSize { get => bottomFontSize; set => Set(ref bottomFontSize, value); }
        public string BottomFontColor { get => bottomFontColor; set => Set(ref bottomFontColor, value); }
        public string BottomFontColorOutline { get => bottomFontColorOutline; set => Set(ref bottomFontColorOutline, value); }

        public string TagsFilter { get => filter; set { Set(ref filter, value); changeValue?.Invoke(value); } }

        public void SubcribeChanged(Action<string> changeValue)
        {
            this.changeValue = changeValue;
        }
    }
}
