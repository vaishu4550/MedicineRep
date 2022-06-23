using MedicalRepresentativeSchedule.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalRepresentativeSchedule.Providers
{
    public interface IMedicineStockProvider
    {
        public Task<List<MedicineStock>> GetMedicineStock();
    }
}
