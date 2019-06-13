using CommonServiceLocator;
using DualSub.ViewModel;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace DualSub
{
    public partial class MainWindow : MetroWindow
    {
        MainViewModel mainViewModel;
        SettingViewModel settingViewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            settingViewModel = ServiceLocator.Current.GetInstance<SettingViewModel>();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            File.WriteAllText("setting.json", JsonConvert.SerializeObject(settingViewModel));
        }

        private void FilmFile_Drop(object sender, System.Windows.DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try {
                    var file = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
                    var filePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".ass";
                    File.Copy(@"temp\converted.ass", filePath);
                    ServiceLocator.Current.GetInstance<LoggerViewModel>().AddLog("Copied: "+ filePath);
                }
                catch (Exception ex)
                {
                    ServiceLocator.Current.GetInstance<LoggerViewModel>().AddError(ex.Message);
                }
            }
        }

        private void FilmFileInfo_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
                mainViewModel.Title = Path.GetFileNameWithoutExtension(file);
                mainViewModel.FileFilm = file;
            }
        }
    }
}
