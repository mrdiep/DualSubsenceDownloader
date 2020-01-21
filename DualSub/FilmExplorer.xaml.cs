using DualSub.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DualSub
{
    public partial class FilmExplorer
    {
        public FilmExplorer()
        {
            InitializeComponent();
           
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            var data = (PlexData)((Button)sender).DataContext;

            var folder = Path.GetDirectoryName(data.File);
            Process.Start("explorer.exe", folder);
        }
    }
}
