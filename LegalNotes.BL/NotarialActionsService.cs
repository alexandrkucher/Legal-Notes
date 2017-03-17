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
    }
}
