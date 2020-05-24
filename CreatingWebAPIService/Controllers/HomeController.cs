using CreatingWebAPIService.Models;
using MaxMind.GeoIP2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Configuration;
using System.Threading.Tasks;
using CreatingWebAPIService.Helper;
using System.Reflection;
using MaxMind.GeoIP2.Responses;

namespace CreatingWebAPIService.Controllers
{
    public class HomeController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        public bool CheckCountryEnableability(RequestModel requestModel)
        {
            bool returnIndicator = false;
            _log.Info("CheckCountryEnableability started");

            // validating input

            // checking if site api key is valid - this is extra security for current API implementation
            if (!String.Equals(requestModel.SiteApiKey, ConfigurationManager.AppSettings["SiteApiKey"]))
            {
                _log.Info($"SiteApiKey does not match <{requestModel.SiteApiKey}>");
                return false;
            }

            if(string.IsNullOrEmpty(requestModel.Ip))
            {
                _log.Info($"Ip Address is missing <{requestModel.Ip}>");
                return false;
            }

            if (requestModel.WhiteList == null || !requestModel.WhiteList.Any())
            {
                _log.Info($"Country White List is missing <{requestModel.WhiteList}>");
                return false;
            }

            // If we are here - All input items are valid

            returnIndicator = CountryIPLookup(requestModel.Ip, requestModel.WhiteList);

            _log.Info($"CheckCountryEnableability existing with result <{returnIndicator}>");
           
            // We do not return anything but indicator for security reason
            return returnIndicator;
        }

        public bool CountryIPLookup(string ip, List<Country> countries)
        {
            // call 3rd party and get country name from ip
            return countries.Where(a => a.CountryName == GetCountryFromIP(ip)).ToList().Count > 0 ? true : false;
        }

        private string GetCountryFromIP(string ip)
        {
            // check if CountryFileIs out of the date
            // TODO: move it somewhere to call it less frequently
            Task.Run(() => CheckIfFileIsOutOfDate());

            string fileName = ConfigurationManager.AppSettings["GeoLiteFileName"];

            //FIND ALL FOLDERS IN FOLDER with in own project
            string location = System.Web.HttpContext.Current.Server.MapPath("../../Data") + $"/{fileName}";
            var fi = new FileInfo(location);
            if (!fi.Exists)
            {
                _log.Error($"File {fileName} do not exists");
                // TODO: add logic to wait for the file to be downloaded
                return "error";
            }

            CountryResponse response = null;
            try
            {
                DatabaseReader reader = new DatabaseReader(location);
                response = reader.Country(ip);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception Getting the country {ex}");
                _log.Error($"Exception Message Getting the country {ex.Message}");
                // swallowing the exception
            }

            // Console.WriteLine(response.Country.Name);
            _log.Info($"GetCountryFromIP returnes <{response.Country.Name}> from ip <{ip}>");
            return response.Country.Name;
        }

        static void CheckIfFileIsOutOfDate()
        {
            var fi = new FileInfo(ConfigurationManager.AppSettings["GeoLiteFileName"]);
            if (!fi.Exists || (DateTime.Now - fi.LastWriteTime).TotalDays > Convert.ToDouble(ConfigurationManager.AppSettings["GeoLite2-Country-Days"]))
            {
                DownloadGeoliteDB();
            }
        }

        private static void DownloadGeoliteDB()
        {
            var wc = new WebClient();
            byte[] bData = null;
            try
            {
                bData = wc.DownloadData(ConfigurationManager.AppSettings["GeoLiteFileURL"]);
                MemoryStream zippedStream = new MemoryStream(bData);
                var files = Tar.ExtractTarGzEntries(zippedStream);
                var db = files.First(zippedFile => zippedFile.FileName.EndsWith("mmdb"));
                File.WriteAllBytes("GeoLite2-Country.mmdb", db.Contents);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception Downloading the file {ex}");
                _log.Error($"Exception Downloading the file {ex.Message}");
                // swallowing the exception
            }
        }
    }
}

