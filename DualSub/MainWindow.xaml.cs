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
        public MainWindow()
        {
            InitializeComponent();

            ServiceLocator.Current.GetInstance<MainViewModel>();
        }

        private void FilmFile_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var file = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
                File.Copy(@"temp\converted.ass", Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".ass");

                var tfile = TagLib.File.Create(file);
                string title = tfile.Tag.Title;
                TimeSpan duration = tfile.Properties.Duration;

                tfile.Tag.Title = "my new title";
                tfile.Save();
            }
        }
    }
}
