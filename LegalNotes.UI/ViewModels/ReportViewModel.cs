using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegalNotes.DTO;

namespace LegalNotes.UI.ViewModels
{
    public class ReportViewModel
    {
        public IEnumerable<NotarialActionStat> NotarialActionsStats { get; set; }
        public Filters Filters { get; set; }
    }
}
