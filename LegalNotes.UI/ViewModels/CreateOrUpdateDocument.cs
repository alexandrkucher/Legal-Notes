using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegalNotes.DTO;

namespace LegalNotes.UI.ViewModels
{
    public class CreateOrUpdateDocument
    {
        public IEnumerable<NotarialAction> NotarialActions { get; set; }
        public Document Document { get; set; }
    }
}
