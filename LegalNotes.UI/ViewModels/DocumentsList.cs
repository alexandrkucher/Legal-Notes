using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LegalNotes.DTO;

namespace LegalNotes.UI.ViewModels
{
    public class DocumentsList
    {
        public DocumentsList()
        {
            NotarialActions = new List<NotarialAction>();
            DocumentsViewModels = new ObservableCollection<DocumentViewModel>();
        }

        public IEnumerable<NotarialAction> NotarialActions { get; set; }
        public ObservableCollection<DocumentViewModel> DocumentsViewModels{ get; set; }
    }
}
