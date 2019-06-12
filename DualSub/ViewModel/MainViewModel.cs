using DualSub.Models;
using DualSub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private string title = "The incredibles 1";
        private ICommand searchCommand;
        private IEnumerable<FilmMetadata> films;
        private IEnumerable<SubtitleMetadata> topSubtitles;
        private IEnumerable<SubtitleMetadata> bottomSubtitles;
        private ICommand getSubtitleListCommand;
        private ICommand convertToDualSubtitleCommand;

        public MainViewModel(SubsenceService subsenceService, LoggerViewModel loggerViewModel, AssSubtitleService assSubtitleService)
        {
            SubsenceService = subsenceService;
            Logger = loggerViewModel;
            AssSubtitleService = assSubtitleService;
        }

        public string Title { get => title; set => Set(ref title, value); }

        public IEnumerable<FilmMetadata> Films { get => films; set => Set(ref films, value); }
        public IEnumerable<SubtitleMetadata> TopSubtitles { get => topSubtitles; set => Set(ref topSubtitles, value); }
        public IEnumerable<SubtitleMetadata> BottomSubtitles { get => bottomSubtitles; set => Set(ref bottomSubtitles, value); }

        public ICommand SearchCommand { get => searchCommand ?? (searchCommand = new RelayCommand<string>(async (x) => await SearchCommandImplment((string)x))); }
        public ICommand GetSubtitleListCommand { get => getSubtitleListCommand ?? (getSubtitleListCommand = new RelayCommand<FilmMetadata>(async x => await GetSubtitleListCommandImplement(x))); }
        public ICommand ConvertToDualSubtitleCommand { get => convertToDualSubtitleCommand ?? (convertToDualSubtitleCommand = new RelayCommand<object>(async x => await ConvertToDualSubtitleCommandImplement(x)));  }

        private async Task ConvertToDualSubtitleCommandImplement(object x)
        {
            Logger.AddLog("Start Convert");
            var arr = (object[])x;
            var topSubtitle = (SubtitleMetadata)arr[0];
            var bottomSubtitle = (SubtitleMetadata)arr[1];

            Logger.AddLog("Start download top subtitle");
            var topContent = await SubsenceService.DownloadContent(topSubtitle.Href);

            Logger.AddLog("Start download bottom subtitle");
            var bottomContent = await SubsenceService.DownloadContent(bottomSubtitle.Href);

            Logger.AddLog("Start create subtitle from 2 list");
            AssSubtitleService.CreateDualSub(topContent, bottomContent, Title);
            Logger.AddLog("Complete Convert");
        }

        private async Task GetSubtitleListCommandImplement(FilmMetadata film)
        {
            Logger.AddLog("Start search subtitle for " + film.Title);
            var subtitles = await SubsenceService.GetSubtitles(film.Href);
            TopSubtitles = subtitles.Where(x => x.Language.ToUpper() == "ENGLISH");
            BottomSubtitles = subtitles.Where(x => x.Language.ToUpper() == "VIETNAMESE");

            Logger.AddLog("Complete search subtitle for " + film.Title);
        }


        private async Task SearchCommandImplment(string title)
        {
            Logger.AddLog("Start search film" + title);
            Films = await SubsenceService.SearchFilms(title);
            Logger.AddLog("Complete search film" + title);
        }
    }
}