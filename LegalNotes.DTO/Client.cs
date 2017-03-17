using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PassportNumber { get; set; }
        public string PassportData { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
