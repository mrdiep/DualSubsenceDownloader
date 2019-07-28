using DualSub.Models;
using DualSub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Year {
            get => _year;
            set => Set(ref _year, value);
        }

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


        public ICommand SearchCommand { get => searchCommand ?? (searchCommand = new RelayCommand<string>(async (x) => await SearchCommandImplment((string)x))); }
        public ICommand GetSubtitleListCommand { get => getSubtitleListCommand ?? (getSubtitleListCommand = new RelayCommand<FilmMetadata>(async x => await GetSubtitleListCommandImplement(x))); }
        public ICommand ConvertToDualSubtitleCommand { get => convertToDualSubtitleCommand ?? (convertToDualSubtitleCommand = new RelayCommand<object>(async x => await ConvertToDualSubtitleCommandImplement(x))); }
        public string FileFilm { get => fileFilm; set => Set(ref fileFilm, value); }



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


        private async Task SearchCommandImplment(string title)
        {
            Logger.AddLog("Start search film" + title);

            Subtitles = Enumerable.Empty<SubtitleMetadata>();
            Films = Enumerable.Empty<FilmMetadata>();
            TopSubtitles = Enumerable.Empty<SubtitleMetadata>();
            BottomSubtitles = Enumerable.Empty<SubtitleMetadata>();

            Films = (await SubsenceService.SearchFilms(title)).Where(x => string.IsNullOrWhiteSpace(Year) || x.Title.Contains(Year));
            Logger.AddLog("Complete search film" + title);
        }
    }
}