using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegalNotes.DTO;

namespace LegalNotes.UI.ViewModels
{
    public class Report
    {
        public NotarialAction NotarialAction { get; set; }
        public NotarialActionsType NotarialActionsType { get; set; }
        public NotarialActionsObject NotarialActionsObject { get; set; }
        public decimal Sum { get; set; }
    }
}
