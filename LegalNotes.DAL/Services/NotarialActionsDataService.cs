using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

        public int GetNextDocumentRecordNumber()
        {
            using (var dbContext = new LegalNotesEntities())
            {
                return dbContext.Documents.Any() ? dbContext.Documents.Max(x => x.RecordNumber) + 1 : 1;
            }
        }

        public IEnumerable<DTO.Document> GetDocuments(Filters filters)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var query = GetFilteredQuery(dbContext, filters);

                query = query.OrderByDescending(x => x.RecordNumber);

                return query.ToList();
            }
        }

        private IQueryable<DTO.Document> GetFilteredQuery(LegalNotesEntities dbContext, Filters filters)
        {
            var query = GetDocumentsQuery(dbContext);
            if (filters.StartDate.HasValue)
                query = query.Where(x => x.Date >= filters.StartDate);
            if (filters.EndDate.HasValue)
            {
                var endDate = filters.EndDate.Value.AddDays(1);
                query = query.Where(x => x.Date <= endDate);
            }
            if (filters.NotarialActionId.HasValue)
                query = query.Where(x => x.NotarialAction.NotarialActionId == filters.NotarialActionId);
            if (filters.NotarialActionTypeId.HasValue)
                query = query.Where(x => x.NotarialActionsType.NotarialActionTypeId == filters.NotarialActionTypeId);
            if (filters.NotarialActionObjectId.HasValue)
                query = query.Where(x => x.NotarialActionsObject.NotarialActionObjectId == filters.NotarialActionObjectId);

            return query;
        }

        public void GroupDocuments(IEnumerable<int> docsIds)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var newClientId = Guid.NewGuid();
                var dbDocs = dbContext.Documents.Where(x => docsIds.Contains(x.DocumentId)).ToList();
                foreach (var doc in dbDocs)
                    doc.ClientId = newClientId;

                dbContext.SaveChanges();
            }
        }

        public void ClearGroupingForDocuments(IEnumerable<int> docsIds)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var dbDocsByIds = dbContext.Documents.Where(x => docsIds.Contains(x.DocumentId)).ToList();
                var clientsIds = dbDocsByIds.Where(x => x.ClientId.HasValue).Select(x => x.ClientId).Distinct();
                var dbDocsByClientsIds = dbContext.Documents.Where(x => clientsIds.Contains(x.ClientId)).ToList();
                var dbDocsToProcess = dbDocsByIds.Concat(dbDocsByClientsIds).Distinct();

                foreach (var doc in dbDocsToProcess)
                    doc.ClientId = null;

                dbContext.SaveChanges();
            }
        }

        public IEnumerable<NotarialActionStat> GetNotarialActionsSums(Filters filters)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var query = from d in GetFilteredQuery(dbContext, filters)
                            group d by new
                            {
                                d.NotarialAction,
                                d.NotarialActionsType,
                                d.NotarialActionsObject
                            } into docsGroup
                            select new NotarialActionStat
                            {
                                NotarialAction = docsGroup.Key.NotarialAction,
                                NotarialActionsType = docsGroup.Key.NotarialActionsType,
                                NotarialActionsObject = docsGroup.Key.NotarialActionsObject,
                                Sum = docsGroup.Sum(x => x.Price),
                                ClientsCount = docsGroup.Where(x => !x.ClientId.HasValue).Count() + docsGroup.Where(x => x.ClientId.HasValue).GroupBy(x => x.ClientId).Count()
                            };

                return query.ToList();
            }
        }

        public void AddDocument(DTO.Document newDocument)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var newDBDoc = new DAL.DB.Document();

                UpdateDBDocumentFromModel(newDBDoc, newDocument);

                dbContext.Documents.Add(newDBDoc);
                dbContext.SaveChanges();
            }
        }

        public void UpdateDocument(DTO.Document document)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var existingDocument = dbContext.Documents.FirstOrDefault(x => x.DocumentId == document.DocumentId);

                UpdateDBDocumentFromModel(existingDocument, document);

                dbContext.SaveChanges();
            }
        }

        public void DeleteDocument(DTO.Document document)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var existingDocument = dbContext.Documents.FirstOrDefault(x => x.DocumentId == document.DocumentId);
                dbContext.Documents.Remove(existingDocument);
                dbContext.SaveChanges();
            }
        }

        private void UpdateDBDocumentFromModel(DB.Document existingDocument, DTO.Document document)
        {
            existingDocument.Date = document.Date;
            existingDocument.RecordNumber = document.RecordNumber;
            existingDocument.Price = document.Price;
            existingDocument.NotarialActionId = document.NotarialAction.NotarialActionId;
            existingDocument.NotarialActionTypeId = document.NotarialActionsType?.NotarialActionTypeId;
            existingDocument.NotarialActionObjectId = document.NotarialActionsObject?.NotarialActionObjectId;
        }

        private IQueryable<DTO.Document> GetDocumentsQuery(LegalNotesEntities dbContext)
        {
            return
                dbContext.Documents.Select(source => new DTO.Document
                {
                    DocumentId = source.DocumentId,
                    RecordNumber = source.RecordNumber,
                    Date = source.Date,
                    Price = source.Price,
                    NotarialAction = source.NotarialActionId.HasValue ? new DTO.NotarialAction
                    {
                        NotarialActionId = source.NotarialAction.NotarialActionId,
                        Name = source.NotarialAction.Name
                    } : null,
                    NotarialActionsType = source.NotarialActionTypeId.HasValue ? new DTO.NotarialActionsType
                    {
                        NotarialActionTypeId = source.NotarialActionsType.NotarialActionTypeId,
                        Name = source.NotarialActionsType.Name
                    } : null,
                    NotarialActionsObject = source.NotarialActionObjectId.HasValue ? new DTO.NotarialActionsObject
                    {
                        NotarialActionObjectId = source.NotarialActionsObject.NotarialActionObjectId,
                        Name = source.NotarialActionsObject.Name
                    } : null,
                    ClientId = source.ClientId
                });
        }

        private IQueryable<DTO.NotarialAction> GetQuery(LegalNotesEntities dbContext)
        {
            return
                dbContext.NotarialActions.Select(source => new DTO.NotarialAction
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
