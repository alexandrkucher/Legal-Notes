using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegalNotes.DAL.DB;
using LegalNotes.DTO;
using DTO = LegalNotes.DTO;

namespace LegalNotes.DAL.Services
{
    public class NotarialActionsDataService
    {
        public IEnumerable<DTO.NotarialAction> GetNotarialActions()
        {
            using (var dbContext = new LegalNotesEntities())
            {
                return GetQuery(dbContext).ToList();
            }
        }

        private IQueryable<DTO.NotarialAction> GetQuery(LegalNotesEntities dbContext)
        {
            return dbContext.NotarialActions.Select(source => new DTO.NotarialAction
            {
                NotarialActionId = source.NotarialActionId,
                Name = source.Name,
                NotarialActionsTypes = source.NotarialActionsTypes.Select(type => new DTO.NotarialActionsType
                {
                    NotarialActionTypeId = type.NotarialActionTypeId,
                    Name = type.Name,
                    NotarialActionsObjects = type.NotarialActionsObjects.Select(obj => new DTO.NotarialActionsObject
                    {
                        NotarialActionObjectId = obj.NotarialActionObjectId,
                        Name = obj.Name
                    })
                })
            });
        }
    }
}
