using MedicalRepresentativeSchedule.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MedicalRepresentativeSchedule.Providers
{
    public class MedicineStockProvider:IMedicineStockProvider
    {
        List<MedicineStock> _stockData = new List<MedicineStock>() { };
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(RepScheduleProvider));
        public async Task<List<MedicineStock>> GetMedicineStock()
        {
            try
            {
                _log.Info("Getting medicine stock");
                var client = new HttpClient();
                using (var response = await client.GetAsync("https://localhost:44394/MedicineStockInformation"))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        string stringData = response.Content.ReadAsStringAsync().Result;
                        _stockData = JsonConvert.DeserializeObject<List<MedicineStock>>(stringData);
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        _stockData = null;
                    }
                    else
                    {
                        throw new Exception("Internal error in api called");
                    }
                }
                return _stockData;
            }
            catch (Exception e)
            {
                _log.Error("Error in getting RepScheduleProvider while getting medicine stock - " + e.Message);
                throw;
            }
        }


    }
}
