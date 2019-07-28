using DualSub.Models;
using DualSub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DualSub.ViewModel
{
    public class PlexExplorerViewModel : ViewModelBase
    {
        private readonly PlexXmlService service;
        private readonly SettingViewModel setting;
        private readonly LoggerViewModel logger;
        public ObservableCollection<PlexData> PlexItems { get; private set; } = new ObservableCollection<PlexData>();
        public PlexExplorerViewModel(PlexXmlService service, PlexDataMapperService mapperService, SettingViewModel setting, LoggerViewModel logger)
        {
            this.service = service;
            MapperService = mapperService;
            this.setting = setting;
            this.logger = logger;
        }

        public bool IsScaning { get => _isScaning; set => Set(ref _isScaning, value); }

        public ICommand BeginScanCommand { get => beginScanCommand ?? new RelayCommand(async () => await BeginScanCommandImplement()); }
        public PlexDataMapperService MapperService { get; }

        private ICommand beginScanCommand;
        private bool _isScaning;

        private async Task BeginScanCommandImplement()
        {
            IsScaning = true;
            try
            {
                PlexItems.Clear();
                var movies = await service.GetAll(setting.PlexServer, setting.LibrarySection, setting.PlexToken);
                PlexItems = new ObservableCollection<PlexData>(movies);
                foreach(var plexItem in PlexItems)
                {
                    MapperService.AddChecking(plexItem);
                }
                RaisePropertyChanged("PlexItems");
            }
            catch(Exception ex)
            {
                logger.AddLog(ex.Message);
            }
            finally
            {
                IsScaning = false;
            }
        }
    }
}