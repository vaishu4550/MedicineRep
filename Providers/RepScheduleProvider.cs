using MedicalRepresentativeSchedule.models;
using MedicalRepresentativeSchedule.repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq.Language;
using Castle.Core.Internal;

namespace MedicalRepresentativeSchedule.Providers
{
    public class RepScheduleProvider : IRepScheduleProvider
    {
        private List<MedicineStock> _stockData=new List<MedicineStock>();
        private readonly List<DateTime> _dates = new List<DateTime>();
        private List<DoctorDTO> _docNames = new List<DoctorDTO>();
        private List<RepresentativeDetailsDTO> _repNames = new List<RepresentativeDetailsDTO>();
        private List<MedicineStock> _stock = new List<MedicineStock>();
        private readonly List<RepSchedule> _repSchedule = new List<RepSchedule>();
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(RepScheduleProvider));
        private readonly IRepScheduleRepository _repScheduleRepository;
        private readonly IMedicineStockProvider _medicineStockProvider;

        public RepScheduleProvider(IRepScheduleRepository repScheduleRepository,IMedicineStockProvider medicineStockProvider)
        {
            _repScheduleRepository = repScheduleRepository;
            _medicineStockProvider = medicineStockProvider;
        }

        public List<DoctorDTO> GetDoctors()
        {
            try
            {
                _log.Info("Getting list of doctors");
                return _repScheduleRepository.GetDoctorDetails();
            }
            catch (Exception e)
            {
                _log.Error("Error in getting RepScheduleProvider while getting list of doctors - "+e.Message);
                throw;
            }
        }
        public List<RepresentativeDetailsDTO> GetRepresentatives()
        {
            try
            {
                _log.Info("Getting list of representatives");
                return _repScheduleRepository.GetRepresentativesDetails();
            }
            catch (Exception e)
            {
                _log.Error("Error in getting RepScheduleProvider while getting list of representatives - " + e.Message);
                throw;
            }
        }
        public async Task<List<RepSchedule>> GetRepScheduleAsync(DateTime startDate)
        {
            try
            {
                if (_dates.Count > 0)
                {
                    _dates.Clear();
                }
            
                DateTime start = startDate;
                DateTime end = start.AddDays(6);
                int workDays = 0;
                while (start != end)
                {
                    if (start.DayOfWeek != DayOfWeek.Sunday)
                    {
                        _dates.Add(start);
                        workDays++;
                    }
                    if(workDays==5)
                    {
                        break;
                    }
                    start = start.AddDays(1);
                }
                _repNames = GetRepresentatives();
                _stock = await _medicineStockProvider.GetMedicineStock();
                _docNames = GetDoctors();
                if (_repNames.IsNullOrEmpty() || _stock.IsNullOrEmpty() || _docNames.IsNullOrEmpty())
                {
                    _log.Error("Could not get RepName or Stock or DocName");
                    return null;
                }
                for (var i=0;i<_dates.Count;i++)
                {
                    var rep = new RepSchedule
                    {
                        RepName = _repNames[(i % _repNames.Count)].RepresentativeName,
                        DoctorName = _docNames[i].Name,
                        TreatingAilment = _docNames[i].TreatingAilment,
                        MeetingSlot = "1pm-2pm",
                        DateOfMeeting = _dates[i].ToShortDateString()
                    };
                    var meds = (from s in _stock where s.TargetAilment.Contains(_docNames[i].TreatingAilment) select s.Name).ToList();
                    rep.Medicine = string.Join(",", meds);
                    rep.DoctorContactNumber = _docNames[i].ContactNumber;
                    _repSchedule.Add(rep);
                }
                _log.Info("returning schedule");
                return _repSchedule;
            }
            catch (Exception e)
            {
                _log.Error("Error while scheduling - "+e.Message);
                throw;
            }
        }
    }
}
