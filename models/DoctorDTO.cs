using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalRepresentativeSchedule.models
{
    public class DoctorDTO
    {
        public string Name { get; set; }
        public int ContactNumber { get; set; }
        public string TreatingAilment { get; set; }
    }
}
