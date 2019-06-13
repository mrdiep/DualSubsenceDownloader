using System;
using System.Collections.Generic;
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
                var content = await "https://subscene.com/subtitles/searchbytitle".PostMultipartAsync(x =>
                    x.AddString("query", filmTitle)
                    .AddString("l", "")
                    .AddString("g-recaptcha-response", "03AOLTBLQ-ehT2q33gENO3ckDtDQtdPuevceGUNyQ-82yYlQ1QnutsZ529GHjRllrUYLP180ek5e-1iXw3Xs4VU8cEx2HAf4JmuxL7Kyuz6ZBl4Hqknr-nNsqXeZjN-B0ztqgtBwGfb1nq_0q0pyBc9Mxkc3cfR6hhZePacLjSfOvfoQqtHG9BBbszlFHGJh_6hB6xt7malvG-QCD5X7SwfQ-DSstNQADGMC-EYan9uX2VuG-NQHbLe_hW9TlwkMdeaqGKjjFgYQyVNY92CkM8hz_Py05zel6aoihAYJyAQCXc7ZT1T0y9B21GPbtZ-UYpOKjuR4msD7Vd")
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
                logger.AddLog(ex.Message);
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
                logger.AddLog("Error: " + ex.Message);
            }

            return null;
        }

        

        public async Task<IEnumerable<SubtitleItem>> DownloadContent(string url)
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
                           entry.ExtractToFile(Path.Combine("temp", "subtitle.text"), true);

                            var parser = new SrtParser();
                            using (var fileStream = File.OpenRead(@"temp\subtitle.text"))
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
                logger.AddLog(ex.Message);
            }

            return null;
        }
    }
}
