using DualSub.Models;
using DualSub.Services;
using DualSub.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DualSub.Services
{
    public class PlexDataMapperService : ViewModelBase
    {
        public PlexDataMapperService(LoggerViewModel logger)
        {
        }
        
        public void AddChecking(PlexData data)
        {
            data.File = data.File.Replace(@"\mnt\library", @"\\raspberrypi\HDD");
            data.File = data.File.Replace(@"/mnt/library", @"\\raspberrypi\HDD");
            var folder = Path.GetDirectoryName(data.File);
            var file = Path.GetFileNameWithoutExtension(data.File);
            if (File.Exists(Path.Combine(folder, file + ".ass")))
            {
                data.HasDualSub = true;
            }
        }
    }
}