using DualSub.ViewModel;
using HtmlAgilityPack;
using SubtitlesParser.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DualSub.Services
{
    public class DualSrtSubtitleService: IGenerateSubtitleService
    {
        public LoggerViewModel Logger { get; }

        public DualSrtSubtitleService(LoggerViewModel logger)
        {
            Logger = logger;
        }

        private class DualSubtitleItem : SubtitleItem
        {
            public enum Position
            {
                Top = 1,
                Bot = 2
            }

            public Position PositionAt { get; set; }
        }

        public void CreateDualSub(IEnumerable<SubtitleItem> topContent, IEnumerable<SubtitleItem> bottomContent, string title, SettingViewModel settingViewModel)
        {
            try
            {

                var allSubtitle = topContent.Select(x => new DualSubtitleItem
                {
                    EndTime = x.EndTime,
                    StartTime = x.StartTime,
                    Lines = x.Lines,
                    PositionAt = DualSubtitleItem.Position.Top
                }).Concat(bottomContent.Select(x => new DualSubtitleItem
                {
                    EndTime = x.EndTime,
                    StartTime = x.StartTime,
                    Lines = x.Lines,
                    PositionAt = DualSubtitleItem.Position.Bot
                }))
                .OrderBy(x => x.StartTime).ToList();

                var allSubtitle2 = allSubtitle.Select(x => (allSubtitle.IndexOf(x) + 1) + "\r\n" + FormarTime(x.StartTime) + "  --> " + FormarTime(x.EndTime) + "\r\n" + $"<font {(x.PositionAt==DualSubtitleItem.Position.Top ? " size=\"9px\" " : "")} color=\"#{(x.PositionAt == DualSubtitleItem.Position.Bot ? "F46B41" : "ffffff")}\">" + string.Join(" ", x.Lines.Select(RemoveHtml)) + "</font>\r\n");

                StringBuilder builder = new StringBuilder();

                builder.AppendLine(string.Join("\r\n", allSubtitle2));

                //File.WriteAllText(@"temp\" + title + ".ass", builder.ToString());
                File.WriteAllText(@"temp\converted.srt", builder.ToString());
            }
            catch (Exception ex)
            {
                Logger.AddError(ex.Message);
            }
        }

        private static string FormarTime(int time) =>  TimeSpan.FromMilliseconds(time).ToString(@"hh\:mm\:ss\,ff");

        private string RemoveHtml(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                return string.Empty;
            }

            var document = new HtmlDocument();
            document.LoadHtml(arg);
            return string.Join("\\N" , document.DocumentNode.InnerText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
