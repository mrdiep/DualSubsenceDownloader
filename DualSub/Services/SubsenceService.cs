using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DualSub.Models;
using DualSub.ViewModel;
using Flurl.Http;
using HtmlAgilityPack;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

namespace DualSub.Services
{
    public partial class SubsenceService
    {
        private readonly LoggerViewModel logger;

        public SubsenceService(LoggerViewModel logger)
        {
            this.logger = logger;
        }

        public async Task<IEnumerable<FilmMetadata>> SearchFilms(string filmTitle)
        {
            try
            {
                var content = await "https://subscene.com/subtitles/searchbytitle"
                    .PostMultipartAsync(x => x
                    .AddString("query", filmTitle)
                    .AddString("l", "")
                    .AddString("g-recaptcha-response", ConfigurationManager.AppSettings["subsence-recaptcha-response"])
                ).ReceiveString();

                var document = new HtmlDocument();
                document.LoadHtml(content);

                var list = document.QuerySelectorAll(".title a[href]")
                    .Select(x => new FilmMetadata { Href = x.Attributes["href"].Value, Title = x.InnerText.Trim() })
                    .Distinct()
                    .ToList();

                return list;

            } catch(Exception ex)
            {
                logger.AddError(ex.Message);
            }

            return null;
        }

        public async Task<IEnumerable<SubtitleMetadata>> GetSubtitles(string url)
        {
            try
            {
                var content = await ("https://subscene.com" + url).GetStringAsync();
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                var list = htmlDocument
                    .QuerySelectorAll(".content table td a[href^=/subtitles]")
                    .Select(x => new SubtitleMetadata { Href  =x.Attributes["href"].Value, Language = x.QuerySelector("span:nth-child(1)").InnerText.Trim(), Title =  x.QuerySelector("span:nth-child(2)").InnerText.Trim()})
                    .ToList();

                return list;
            }
            catch (Exception ex)
            {
                logger.AddError(ex.Message);
            }

            return null;
        }

        

        public async Task<IEnumerable<SubtitleItem>> DownloadContent(string url, string saveAsName)
        {
            try
            {
                var document = new HtmlDocument();

                var content =  await ("https://subscene.com" + url).GetStringAsync();
                document.LoadHtml(content);

                var urlFile = document.QuerySelector("#downloadButton").Attributes["href"].Value;

                await ("https://subscene.com" + urlFile).DownloadFileAsync(@"temp", "subtitle.zip");

                using (ZipArchive archive = ZipFile.OpenRead(Path.Combine("temp", "subtitle.zip")))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase))
                        {
                           entry.ExtractToFile(Path.Combine("temp", saveAsName), true);

                            var parser = new SrtParser();
                            using (var fileStream = File.OpenRead(@"temp\" + saveAsName))
                            {
                                var items = parser.ParseStream(fileStream, Encoding.UTF8);

                                return items;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.AddError(ex.Message);
            }

            return null;
        }
    }
}
