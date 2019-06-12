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
    public class AssSubtitleService
    {
        public LoggerViewModel Logger { get; }

        public AssSubtitleService(LoggerViewModel logger)
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

        public void CreateDualSub(IEnumerable<SubtitleItem> topContent, IEnumerable<SubtitleItem> bottomContent, string title)
        {
            try
            {
                var template = File.ReadAllText(@"temp\ASS-TEMPLATE.txt");
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
                })).OrderBy(x => x.StartTime)
                .Select(x => $@"Dialogue: 0," + FormarTime(x.StartTime) + "," + FormarTime(x.EndTime) + "," + x.PositionAt.ToString() + ",,0000,0000,0000,," + string.Join("\\N", x.Lines.Select(RemoveHtml)));

                StringBuilder builder = new StringBuilder(template);

                builder.AppendLine(string.Join("\r\n", allSubtitle));

                File.WriteAllText(@"temp\" + title + ".ass", builder.ToString());
                File.WriteAllText(@"temp\converted.ass", builder.ToString());
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex.Message);
            }
        }

        private static string FormarTime(int time) =>  TimeSpan.FromMilliseconds(time).ToString(@"hh\:mm\:ss\.ff");

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
