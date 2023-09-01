
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using CsvHelper;
using System.IO;

namespace WebAutomationAPIClient
{
    class Program
    {
        private const string API = "https://api.webautomation.io/api";
        private const string USER = "Your_username";
        private const string PASSWORD = "Your_password";
        private static readonly HttpClient client = new HttpClient();

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes($"{USER}:{PASSWORD}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            await GetAllBatches();
            await GetBatchDetails();
            await GetDataOfBatch();
            await GetDataOfOneRequestInBatch();
            await InsertYourBatchFromCSV();
        }

        public static async System.Threading.Tasks.Task GetAllBatches()
        {
            var response = await client.GetAsync($"{API}/batch/");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public static async System.Threading.Tasks.Task GetBatchDetails()
        {
            var data = new
            {
                options = new { batchid = "1693464873416946832" }
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{API}/batch/status/", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public static async System.Threading.Tasks.Task GetDataOfBatch()
        {
            var data = new
            {
                options = new { batchid = "1693569365463776731" }
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{API}/batch/data/", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public static async System.Threading.Tasks.Task GetDataOfOneRequestInBatch()
        {
            var data = new
            {
                options = new { requestid = "1693569365463776731-7" }
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{API}/batch/data/", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
        public async Task InsertYourBatchFromCSV()
        {
            var url = $"{API}/batch/";
            var dataList = new List<Dictionary<string, object>>();

            using (var reader = new StreamReader("/app/Example_Skyscanner_AU_scrape.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Dictionary<string, string>>();
                foreach (var record in records)
                {
                    dataList.Add(CreateDataDict(record));
                }
            }

            var content = new StringContent(JsonConvert.SerializeObject(dataList), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            Console.WriteLine($"resp status: {response.StatusCode}");
            Console.WriteLine($"resp status: {response.IsSuccessStatusCode}");
            Console.WriteLine($"resp test: {await response.Content.ReadAsStringAsync()}");
        }






        public static dynamic CreateDataDict(dynamic row)
        {
            string startDate = $"{row.PickupDate}T{row.PickupTime}";
            string endDate = $"{row.DropoffDate}T{row.DropoffTime}";

            return new
            {
                url = new List<string> { "https://www.skyscanner.com.au" },
                use_js_rendering = true,
                return_raw_data = false,
                options = new
                {
                    domain = "www.skyscanner.com",
                    job_id = row.Request_id,
                    site = "https://www.skyscanner.com.au/",
                    language = row.Language.ToLower(),
                    airport_code = row.PickupLocationCode,
                    currency = row.currency,
                    start_date = startDate,
                    end_date = endDate
                }
            };
        }
    }
}
