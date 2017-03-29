using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using LegalNotes.DTO;

namespace LegalNotes.UI.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel(Document document)
        {
            this.Document = document;
        }

        public Document Document { get; set; }
        public Brush Brush { get; set; }
    }
}
