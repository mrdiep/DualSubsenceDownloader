using CommonServiceLocator;
using DualSub.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace DualSub
{
    public partial class MainWindow : MetroWindow
    {
        MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();

            mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
        }

        private void FilmFile_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var file = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
                File.Copy(@"temp\converted.ass", Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".ass");

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
