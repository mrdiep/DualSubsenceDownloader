using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using DualSub.ViewModel;
using DualSub.Models;
using System.Windows;
using System.Windows.Threading;

namespace DualSub.Services
{
    public class PlexXmlService
    {

        public PlexXmlService(LoggerViewModel logger)
        {
            Logger = logger;
        }

        public LoggerViewModel Logger { get; }

        public async Task<List<PlexData>> GetAll(string serverId, string section, string token)
        {
            var data = new List<PlexData>();
            var notify = string.Empty;
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(
                        $@"{serverId}/library/sections/{section}/all?X-Plex-Token={token}");

                    var videoNodes = xmlDoc.SelectNodes("//MediaContainer/Video");
                    foreach (var videoNode in videoNodes)
                    {
                        var xmlNode = (XmlNode)videoNode;
                        var filePath = xmlNode.SelectSingleNode("Media/Part")?.Attributes["file"]?.Value;

                        if (string.IsNullOrWhiteSpace(filePath)) continue;

                        data.Add(new PlexData
                        {
                            Title = xmlNode.Attributes["title"]?.Value,
                            Year = xmlNode.Attributes["year"]?.Value,
                            File = filePath
                        });
                    }

                    notify = "Found : " + data.Count + " movies";
                }
                catch (Exception ex)
                {
                    notify = ex.Message;
                    
                    
                }
            });

            Dispatcher.CurrentDispatcher.Invoke(() => {
                Logger.AddLog(notify);
            });
            return data;
        }
    }
}
