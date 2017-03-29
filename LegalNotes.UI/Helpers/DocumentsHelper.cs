using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using LegalNotes.DTO;
using LegalNotes.UI.ViewModels;

namespace LegalNotes.UI.Helpers
{
    public static class DocumentsHelper
    {
        static public IEnumerable<DocumentViewModel> ConvertToViewModels(IEnumerable<Document> documents)
        {
            ColourGenerator generator = new ColourGenerator();

            var docsClientsIds = documents.Where(x => x.ClientId.HasValue).Select(x => x.ClientId).Distinct();
            var coloursMap = new Dictionary<Guid, string>();

            foreach (var docClientId in docsClientsIds)
                coloursMap.Add(docClientId.Value, generator.NextColour());

            var result = new List<DocumentViewModel>();

            foreach (var doc in documents)
            {
                var docViewModel = new DocumentViewModel(doc);
                if (doc.ClientId.HasValue)
                {
                    var color = (Color)ColorConverter.ConvertFromString("#30" + coloursMap[doc.ClientId.Value]);
                    docViewModel.Brush = new SolidColorBrush(color);
                }

                result.Add(docViewModel);
            }

            return result;
        }
    }
}
