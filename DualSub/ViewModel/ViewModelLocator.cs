using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using DualSub.Services;

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

            Logger.AddLog("Application Started");
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public LoggerViewModel Logger
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoggerViewModel>();
            }
        }

        public static void Cleanup()
        {

        }
    }
}