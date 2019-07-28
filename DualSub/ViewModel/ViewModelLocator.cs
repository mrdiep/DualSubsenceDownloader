using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using DualSub.Services;
using Newtonsoft.Json;
using System.IO;
using System;

namespace DualSub.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoggerViewModel>();
            SimpleIoc.Default.Register<SubsenceService>();
            SimpleIoc.Default.Register<AssSubtitleService>();
            SimpleIoc.Default.Register<PlexDataMapperService>();
            SimpleIoc.Default.Register<PlexXmlService>();
            SimpleIoc.Default.Register<PlexExplorerViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>(() => JsonConvert.DeserializeObject<SettingViewModel>(File.ReadAllText("setting.json")));

            Logger.AddLog("Application Started");
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public SettingViewModel Setting
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingViewModel>();
            }
        }

        public LoggerViewModel Logger
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoggerViewModel>();
            }
        }
        public PlexExplorerViewModel PlexExplorerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlexExplorerViewModel>();
            }
        }
        public static void Cleanup()
        {

        }
    }
}