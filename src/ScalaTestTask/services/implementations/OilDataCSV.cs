using ScalaTestTask.models;
using ScalaTestTask.services.interfaces;
using System;
using System.Globalization;
using System.Text;

namespace ScalaTestTask.services.implementations
{
    public class OilDataCSV : IDbService
    {
        private readonly OilPriceInfo[] _oilInfoes;
        private static async Task DownloadFileAsync(Uri uri, string csvFile)
        {
            if (File.Exists(csvFile))
            {
                return;
            }
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(uri);
            using var fs = File.Create(csvFile);
            await response.Content.CopyToAsync(fs);
        }
        private static void DownloadFile(Uri uri, string csvFile)
        {
            if (File.Exists(csvFile))
            {
                return;
            }
            HttpClient client = new HttpClient();
            var responseTask = client.GetAsync(uri);
            responseTask.Wait();
            var response = responseTask.Result;
            using var fs = File.Create(csvFile);
            response.Content.CopyTo(fs, null, new CancellationToken());
        }
        public OilDataCSV(string csvFile)
        {
            _oilInfoes = CsvParcer(csvFile);
        }
        public OilDataCSV(Uri uri, string csvFile)
        {
            DownloadFile(uri, csvFile);
            _oilInfoes = CsvParcer(csvFile);
        }

        public OilPriceInfo[] OilPrices => _oilInfoes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        /// /// <exception cref="FormatException">when things go wrong.</exception>
        private OilPriceInfo[] CsvParcer(string csvFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using StreamReader sr = new StreamReader(csvFile, Encoding.GetEncoding("windows-1251"));

            var oilPrices = new List<OilPriceInfo>();

            string headerLine = sr.ReadLine()!;
            string line;
            while ((line = sr.ReadLine()!) is not null)
            {
                var oilRecord = ParceOilPriceInfo(line);
                oilPrices.Add(oilRecord);
            }
            return oilPrices.ToArray();
        }        
        private OilPriceInfo ParceOilPriceInfo(string csvLine) 
        {
            try
            {
                var values = csvLine.Split(';');
                var begin = DateParser(values[0]);
                var end = DateParser(values[1]);
                var pr = Convert.ToDecimal(values[2]);
                return new OilPriceInfo ( new DateTimeRange { Start = begin,End = end}, pr );
            }
            catch (FormatException)
            {              
                throw new FormatException("Invalid date format file");
            }
        }
        private DateTime DateParser(string s)
        {
            // using because of the strange work of the method DateTime.ParseExact("15.сен.13","dd.MMM.yy",new CultureInfo("ru-RU"))
            if (DateTime.TryParse(s, out DateTime date))
            {
                return date;
            }
            if (s.Contains("янв"))
            {
                return DateTime.Parse(s.Replace("янв", "jan"));
            }
            if (s.Contains("фев"))
            {
                return DateTime.Parse(s.Replace("фев", "feb"));
            }
            if (s.Contains("сен"))
            {
                return DateTime.Parse(s.Replace("сен", "sep"));
            }
            if (s.Contains("окт"))
            {
                return DateTime.Parse(s.Replace("окт", "oct"));
            }
            if (s.Contains("ноя"))
            {
                return DateTime.Parse(s.Replace("ноя", "nov"));
            }
            if (s.Contains("дек"))
            {
                return DateTime.Parse(s.Replace("дек", "dec"));
            }
            throw new NotImplementedException();
        }
        //public OilPriceInfo[] CsvParcer1(string csvFile)
        //{
        //    // csvFile - Windows-1251
        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //    Encoding utf8 = Encoding.GetEncoding("UTF-8");
        //    Encoding win1251 = Encoding.GetEncoding("Windows-1251");

        //    byte[] win1251Bytes = File.ReadAllBytes(csvFile);
        //    byte[] utf8Bytes = Encoding.Convert(win1251, utf8, win1251Bytes);

        //    var oilPrices = new List<OilPriceInfo>();
        //    using StreamReader sr = new StreamReader(new MemoryStream(utf8Bytes));
        //    string headerLine = sr.ReadLine()!;
        //    string line;
        //    var excList = new List<Exception>();
        //    while ((line = sr.ReadLine()!) is not null)
        //    {
        //        try
        //        {
        //            CultureInfo culture = new CultureInfo("ru-RU");
        //            string format = "dd.MMM.yy";

        //            var v = line.Split(';');
        //            var d1 = DateTime.ParseExact(v[0], format, culture);
        //            var d2 = DateTime.ParseExact(v[1], format, culture);
        //            var pr = Convert.ToDecimal(v[2]);
        //            var oilRecord = new OilPriceInfo { StartDate = d1, EndDate = d2, AvgPrice = pr };
        //            oilPrices.Add(oilRecord);
        //            //
        //            //DateTime.ParseExact()
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //            excList.Add(ex);
        //            //throw;
        //        }

        //        //var oilRecord = new OilPriceInfo 
        //        //{
        //        //    StartDate = Convert.ToDateTime(v[0]),
        //        //    EndDate = Convert.ToDateTime(v[1]),
        //        //    AvgPrice = Convert.ToDecimal(v[2])
        //        //};
        //        //oilPrices.Add(oilRecord);
        //    }

        //    return oilPrices.ToArray();


        //    //Console.WriteLine(utf8.GetString(utf8Bytes));
        //    throw new NotImplementedException();
        //}
        //public void TestParce()
        //{
        //    string s1 = "15.авг.13";
        //    string s2 = "15.сен.13";
        //    string s3 = "15.дек.13";
        //    string format = "dd.MMM.yy";
        //    CultureInfo culture= new CultureInfo("ru-RU");

        //    //DateTime date1 = DateTime.ParseExact(s1, format, culture);
        //    var d1 = DateParser(s1);
        //    d1 = DateParser(s2);
        //    d1 = DateParser(s3);
        //}
    }
}
