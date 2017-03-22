using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegalNotes.DAL.Services;
using LegalNotes.DTO;

namespace LegalNotes.BL
{
    public class NotarialActionsService
    {
        private readonly NotarialActionsDataService notarialActionsDataService;

        public NotarialActionsService()
        {
            this.notarialActionsDataService = new NotarialActionsDataService();
        }

        public IEnumerable<NotarialAction> GetNotarialActions()
        {
            return this.notarialActionsDataService.GetNotarialActions();
        }

        public void CreateDocument(Document document)
        {
            notarialActionsDataService.AddDocument(document);
        }

        public void UpdateDocument(Document document)
        {
            notarialActionsDataService.UpdateDocument(document);
        }

        public IEnumerable<Document> GetDocuments(Filters filters)
        {
            return this.notarialActionsDataService.GetDocuments(filters);
        }

        public int GetNewDocumentId()
        {
            return this.notarialActionsDataService.GetNextDocumentRecordNumber();
        }

        public void Delete(Document document)
        {
            this.notarialActionsDataService.DeleteDocument(document);
        }
    }
}
