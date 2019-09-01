using CommonServiceLocator;
using DualSub.Models;
using DualSub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DualSub.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public SubsenceService SubsenceService { get; }
        public LoggerViewModel Logger { get; }
        public AssSubtitleService AssSubtitleService { get; }
        public SettingViewModel Setting { get; }

        private string title = "The incredibles 1";

        private ICommand searchCommand;
        private IEnumerable<FilmMetadata> films;
        private IEnumerable<SubtitleMetadata> topSubtitles;
        private IEnumerable<SubtitleMetadata> bottomSubtitles;
        private ICommand getSubtitleListCommand;
        private ICommand convertToDualSubtitleCommand;
        private IEnumerable<SubtitleMetadata> subtitles;
        private string filterTopLanguage = "ENGLISH";
        private string filterBottomLanguage = "VIETNAMESE";
        private string isCreateSubtitle;
        private bool isInCreateSubtitles;
        private string fileFilm;
        private string _year = "2004";

        public MainViewModel(SubsenceService subsenceService, LoggerViewModel loggerViewModel, AssSubtitleService assSubtitleService, SettingViewModel settingViewModel)
        {
            SubsenceService = subsenceService;
            Logger = loggerViewModel;
            AssSubtitleService = assSubtitleService;
            Setting = settingViewModel;
            Setting.SubcribeChanged((x) => FilterNow());

        }

        public string Title
        {
            get => title; set
            {
                Set(ref title, value); FileFilm = string.Empty;
            }
        }

        public string Year
        {
            get => _year;
            set => Set(ref _year, value);
        }
        public PlexData CurrentPlexData { get => currentPlexData; set => Set(ref currentPlexData, value); }

        private void FilterNow()
        {
            var filters = Setting.TagsFilter.ToLower().Split(new[] { ";", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            TopSubtitles = subtitles.Where(x => x.Language.ToUpper() == FilterTopLanguage).Where(x => !filters.Any() || ContainsList(x.Title.ToLower(), filters));
            BottomSubtitles = subtitles.Where(x => x.Language.ToUpper() == FilterBottomLanguage).Where(x => !filters.Any() || ContainsList(x.Title.ToLower(), filters));
        }

        private bool ContainsList(string value, string[] filters)
        {
            foreach (var filter in filters)
            {
                if (!value.Contains(filter)) return false;
            }

            return true;
        }

        public IEnumerable<FilmMetadata> Films { get => films; set => Set(ref films, value); }

        public IEnumerable<SubtitleMetadata> Subtitles { get => subtitles; set => Set(ref subtitles, value); }
        public IEnumerable<SubtitleMetadata> TopSubtitles { get => topSubtitles; set => Set(ref topSubtitles, value); }
        public IEnumerable<SubtitleMetadata> BottomSubtitles { get => bottomSubtitles; set => Set(ref bottomSubtitles, value); }

        public string CreateSubtitleStatus { get => isCreateSubtitle; set => Set(ref isCreateSubtitle, value); }
        public bool IsInCreateSubtitles { get => isInCreateSubtitles; set => Set(ref isInCreateSubtitles, value); }

        public string FilterTopLanguage
        {
            get => filterTopLanguage; set
            {
                Set(ref filterTopLanguage, value);
                if (Subtitles == null || !Subtitles.Any())
                    return;

                FilterNow();
            }
        }
        public string FilterBottomLanguage
        {
            get => filterBottomLanguage;
            set
            {
                Set(ref filterBottomLanguage, value);
                if (Subtitles == null || !Subtitles.Any())
                    return;

                FilterNow();
            }
        }


        public ICommand SearchCommand { get => searchCommand ?? (searchCommand = new RelayCommand(async () => await SearchCommandImplment())); }
        public ICommand GetSubtitleListCommand { get => getSubtitleListCommand ?? (getSubtitleListCommand = new RelayCommand<FilmMetadata>(async x => await GetSubtitleListCommandImplement(x))); }
        public ICommand ConvertToDualSubtitleCommand { get => convertToDualSubtitleCommand ?? (convertToDualSubtitleCommand = new RelayCommand<object>(async x => await ConvertToDualSubtitleCommandImplement(x))); }
        public string FileFilm { get => fileFilm; set => Set(ref fileFilm, value); }
        public ICommand DownloadDualSubCommand { get => downloadDualSubCommand ?? (downloadDualSubCommand = new RelayCommand<PlexData>(async x => await DownloadDualSubCommandImplementation(x))); }
        public ICommand TestVLCDisplayDuaSubCommand { get => testVLCDisplayDuaSubCommand ?? (testVLCDisplayDuaSubCommand = new RelayCommand<PlexData>(async x => await TestVLCDisplayDuaSubCommandImplementation(x))); }

        private ICommand testVLCDisplayDuaSubCommand;
        private async Task DownloadDualSubCommandImplementation(PlexData x)
        {
            Title = x.Title;
            Year = x.Year;
            CurrentPlexData = x;
            await SearchCommandImplment();

            if (Films.Any())
            {
                await GetSubtitleListCommandImplement(Films.First());
            }
        }
        private async Task TestVLCDisplayDuaSubCommandImplementation(PlexData x)
        {
            try
            {
                x = CurrentPlexData;
                var q = "\"";
                var sub = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"temp\converted.ass");
                var pName = $@"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe {q}{x.File}{q} --sub-file={q}{sub}{q}";
                Logger.AddLog(pName);
                Process.Start(new ProcessStartInfo {
                    FileName = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe",
                    Arguments = $@"{q}{x.File}{q} --sub-file={q}{sub}{q}"
                });
            }
            catch (Exception ex)
            {
                Logger.AddError(ex.Message);
            }
        }
        private ICommand downloadDualSubCommand;
        private PlexData currentPlexData;

        public ICommand SaveToLocalCommand { get => saveToLocalCommand ?? (saveToLocalCommand = new RelayCommand(SaveToLocal)); }
        private ICommand saveToLocalCommand;
        public void SaveToLocal()
        {
            var file = CurrentPlexData.File;
            var filePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
            File.Copy(@"temp\converted.ass", filePath + ".ass", true);
            File.Copy(@"temp\top.srt", filePath + ".en.srt", true);
            File.Copy(@"temp\bottom.srt", filePath + ".vi.srt", true);
            Logger.AddLog("Copied: " + filePath);
        }

        private async Task ConvertToDualSubtitleCommandImplement(object x)
        {
            CreateSubtitleStatus = "Start Convert...";
            IsInCreateSubtitles = true;
            try
            {
                Logger.AddLog("Start Convert");
                var arr = (object[])x;
                var topSubtitle = (SubtitleMetadata)arr[0];
                var bottomSubtitle = (SubtitleMetadata)arr[1];

                Logger.AddLog("Start download top subtitle");
                CreateSubtitleStatus = "downloading top subtitle...";
                var topContent = await SubsenceService.DownloadContent(topSubtitle.Href, "top.srt");

                Logger.AddLog("Start download bottom subtitle");
                CreateSubtitleStatus = "downloading bottom subtitle...";
                var bottomContent = await SubsenceService.DownloadContent(bottomSubtitle.Href, "bottom.srt");

                Logger.AddLog("Start create subtitle from 2 list");
                CreateSubtitleStatus = "merging 2 subtitles...";
                AssSubtitleService.CreateDualSub(topContent, bottomContent, Title, Setting);
                Logger.AddLog("Complete Convert");
                CreateSubtitleStatus = "Merging completed. \r\nDrop film here to get merged subtitle.";
            }
            catch (Exception ex)
            {
                Logger.AddError(ex.Message);
                CreateSubtitleStatus = "Merge failed";
            }
            finally
            {
                IsInCreateSubtitles = false;
            }
        }

        private async Task GetSubtitleListCommandImplement(FilmMetadata film)
        {
            Logger.AddLog("Start search subtitle for " + film.Title);
            var keepFilterTop = FilterTopLanguage;
            var keepFilterBottom = FilterBottomLanguage;

            TopSubtitles = Enumerable.Empty<SubtitleMetadata>();
            BottomSubtitles = Enumerable.Empty<SubtitleMetadata>();

            Subtitles = await SubsenceService.GetSubtitles(film.Href);

            filterTopLanguage = keepFilterTop;
            filterBottomLanguage = keepFilterBottom;

            FilterNow();

            Logger.AddLog("Complete search subtitle for " + film.Title);
            RaisePropertyChanged("FilterTopLanguage");
            RaisePropertyChanged("FilterBottomLanguage");
        }


        private async Task SearchCommandImplment()
        {
            Logger.AddLog("Start search film" + title);

            Subtitles = Enumerable.Empty<SubtitleMetadata>();
            Films = Enumerable.Empty<FilmMetadata>();
            TopSubtitles = Enumerable.Empty<SubtitleMetadata>();
            BottomSubtitles = Enumerable.Empty<SubtitleMetadata>();

            Films = (await SubsenceService.SearchFilms(Title)).Where(x => string.IsNullOrWhiteSpace(Year) || x.Title.Contains(Year));
            Logger.AddLog("Complete search film" + title);
        }
    }
}