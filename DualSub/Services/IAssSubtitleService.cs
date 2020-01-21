using System.Collections.Generic;
using DualSub.ViewModel;
using SubtitlesParser.Classes;

namespace DualSub.Services
{
    public interface IGenerateSubtitleService
    {
        void CreateDualSub(IEnumerable<SubtitleItem> topContent, IEnumerable<SubtitleItem> bottomContent, string title, SettingViewModel settingViewModel);
    }
}