using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

using netly.Helpers;
using netly.Models;

namespace netly.Controllers
{
    public class ChartController : BaseController
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public ChartController() : this(new Repository()) { }
        public ChartController(IRepository repository) { _repository = repository; }

        public ActionResult TotalByUrl(string shortUrl, string customUrl, string urlHash)
        {
            var chart = new Chart()
            {
                Width = 940,
                Height = 225,
                ImageType = ChartImageType.Png,
                Palette = ChartColorPalette.BrightPastel,
                BackColor = Color.WhiteSmoke,
                RenderType = RenderType.BinaryStreaming,
                BackGradientStyle = GradientStyle.TopBottom,
                BackSecondaryColor = Color.White,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            var area = chart.ChartAreas.Add("ChartArea1");
            area.BorderColor = Color.FromArgb(64, 64, 64, 64);
            area.BackSecondaryColor = Color.White;
            area.BackColor = Color.WhiteSmoke;
            area.ShadowColor = Color.Transparent;
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisY.LabelStyle.Format = "#,##0";
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisX.LabelStyle.Format = "M/d";
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            // Load from the database
            //var history = UrlHistory.GetUrlTotal(shortUrl);
            var history = (from h in _repository.FindUrlHistoryDetails() where h.ShortUrl == shortUrl || h.ShortUrl == customUrl || h.UrlHash == urlHash select h).ToList();
            if (history.Count > 0)
            {
                var chartData = from d in history
                                orderby d.ts
                                group d by d.ts.Date into g
                                select new { Count = g.Count(), Date = g.Key };
                chart.DataBindTable(chartData.ToList(), "Date");
                chart.Series[0].ChartType = SeriesChartType.Column;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            }

            var ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);

            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            return File(ms.GetBuffer(), @"image/png");
        }

        public ActionResult TotalByHash(string hash)
        {
            var chart = new Chart()
            {
                Width = 940,
                Height = 225,
                ImageType = ChartImageType.Png,
                Palette = ChartColorPalette.BrightPastel,
                BackColor = Color.WhiteSmoke,
                RenderType = RenderType.BinaryStreaming,
                BackGradientStyle = GradientStyle.TopBottom,
                BackSecondaryColor = Color.White,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            //chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            //var title = chart.Titles.Add("Main");
            //title.Text = "Alerts";
            //title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //title.ShadowOffset = 2;
            //title.ForeColor = Color.FromArgb(26, 59, 105);
            //title.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            var area = chart.ChartAreas.Add("ChartArea1");
            area.BorderColor = Color.FromArgb(64, 64, 64, 64);
            area.BackSecondaryColor = Color.White;
            area.BackColor = Color.WhiteSmoke;
            area.ShadowColor = Color.Transparent;
            //area.Area3DStyle.PointDepth = 100;
            //area.Area3DStyle.PointGapDepth = 100;
            //area.Area3DStyle.Rotation = 10;
            //area.Area3DStyle.Enable3D = true;
            //area.Area3DStyle.Inclination = 40;
            //area.Area3DStyle.IsRightAngleAxes = false;
            //area.Area3DStyle.WallWidth = 0;
            //area.Area3DStyle.IsClustered = false;
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisY.LabelStyle.Format = "#,##0";
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisX.LabelStyle.Format = "M/d";
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            //var legend = chart.Legends.Add("Default");
            //legend.Enabled = true;
            //legend.IsTextAutoFit = true;
            //legend.BackColor = Color.Transparent;
            //legend.Font = new Font("Segoe Condensed", 8f, FontStyle.Regular);
            //legend.Docking = Docking.Right;

            // TODO: Load from the database
            //var history = UrlHistory.GetUrlHashTotal(hash);
            //chart.DataBindTable(history.DefaultView, "ts");
            chart.Series[0].ChartType = SeriesChartType.Column;
            chart.Series[0].IsValueShownAsLabel = true;
            chart.Series[0].Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);

            var ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);

            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            return File(ms.GetBuffer(), @"image/png");
        }

        public ActionResult CountriesByUrl(string shortUrl, string customUrl, string urlHash)
        {
            var chart = new Chart()
            {
                Width = 350,
                Height = 350,
                ImageType = ChartImageType.Png,
                Palette = ChartColorPalette.BrightPastel,
                BackColor = Color.WhiteSmoke,
                RenderType = RenderType.BinaryStreaming,
                BackGradientStyle = GradientStyle.TopBottom,
                BackSecondaryColor = Color.White,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            //chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            //var title = chart.Titles.Add("Main");
            //title.Text = "Alerts";
            //title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //title.ShadowOffset = 2;
            //title.ForeColor = Color.FromArgb(26, 59, 105);
            //title.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            var area = chart.ChartAreas.Add("ChartArea1");
            area.BorderColor = Color.FromArgb(64, 64, 64, 64);
            area.BackSecondaryColor = Color.White;
            area.BackColor = Color.WhiteSmoke;
            area.ShadowColor = Color.Transparent;
            area.Area3DStyle.PointDepth = 100;
            area.Area3DStyle.PointGapDepth = 100;
            area.Area3DStyle.Rotation = 10;
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 40;
            area.Area3DStyle.IsRightAngleAxes = false;
            area.Area3DStyle.WallWidth = 0;
            area.Area3DStyle.IsClustered = false;
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            //area.AxisY.LabelStyle.Format = "$#,##0";
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            //area.AxisX.LabelStyle.Format = "MM/dd";
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            var legend = chart.Legends.Add("Default");
            legend.Enabled = true;
            legend.IsTextAutoFit = true;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font("Segoe UI", 7f, FontStyle.Regular);
            legend.Docking = Docking.Bottom;

            // Load from the database
            //var history = UrlHistory.GetUrlTotal(shortUrl);
            var history = (from h in _repository.FindUrlHistoryDetails() where h.ShortUrl == shortUrl || h.ShortUrl == customUrl || h.UrlHash == urlHash select h).ToList();
            if (history.Count > 0)
            {
                var chartData = from d in history
                                orderby d.CountryCode
                                group d by d.CountryName + " (" + d.CountryCode + ")" into g
                                select new { Count = g.Count(), Country = g.Key };
                chart.DataBindTable(chartData.ToList(), "Country");
                chart.Series[0].ChartType = SeriesChartType.Pie;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].Font = new Font("Segoe UI", 7f, FontStyle.Regular);
            }

            var ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);

            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            return File(ms.GetBuffer(), @"image/png");
        }

        public ActionResult ReferrersByUrl(string shortUrl, string customUrl, string urlHash)
        {
            var chart = new Chart()
            {
                Width = 350,
                Height = 350,
                ImageType = ChartImageType.Png,
                Palette = ChartColorPalette.BrightPastel,
                BackColor = Color.WhiteSmoke,
                RenderType = RenderType.BinaryStreaming,
                BackGradientStyle = GradientStyle.TopBottom,
                BackSecondaryColor = Color.White,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            //chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            //var title = chart.Titles.Add("Main");
            //title.Text = "Alerts";
            //title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //title.ShadowOffset = 2;
            //title.ForeColor = Color.FromArgb(26, 59, 105);
            //title.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            var area = chart.ChartAreas.Add("ChartArea1");
            area.BorderColor = Color.FromArgb(64, 64, 64, 64);
            area.BackSecondaryColor = Color.White;
            area.BackColor = Color.WhiteSmoke;
            area.ShadowColor = Color.Transparent;
            area.Area3DStyle.PointDepth = 100;
            area.Area3DStyle.PointGapDepth = 100;
            area.Area3DStyle.Rotation = 10;
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 40;
            area.Area3DStyle.IsRightAngleAxes = false;
            area.Area3DStyle.WallWidth = 0;
            area.Area3DStyle.IsClustered = false;
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            //area.AxisY.LabelStyle.Format = "$#,##0";
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            //area.AxisX.LabelStyle.Format = "MM/dd";
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            var legend = chart.Legends.Add("Default");
            legend.Enabled = true;
            legend.IsTextAutoFit = true;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font("Segoe UI", 7f, FontStyle.Regular);
            legend.Docking = Docking.Bottom;

            // Load from the database
            //var history = UrlHistory.GetUrlTotal(shortUrl);
            var history = (from h in _repository.FindUrlHistoryDetails() where h.ShortUrl == shortUrl || h.ShortUrl == customUrl || h.UrlHash == urlHash select h).ToList();
            if (history.Count > 0)
            {
                var chartData = from d in history
                                orderby d.HttpReferer
                                group d by d.HttpReferer into g
                                select new { Count = g.Count(), Referrer = string.IsNullOrWhiteSpace(g.Key) ? "Email clients, IM and Direct" : g.Key };
                chart.DataBindTable(chartData.ToList(), "Referrer");
                chart.Series[0].ChartType = SeriesChartType.Pie;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].Font = new Font("Segoe UI", 7f, FontStyle.Regular);
            }

            var ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);

            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            return File(ms.GetBuffer(), @"image/png");
        }

    }
}
