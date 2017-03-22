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

                query = query.OrderByDescending(x => x.ModifiedDate.HasValue && x.ModifiedDate > x.CreateDate ? x.ModifiedDate : x.CreateDate);

                return query.ToList();
            }
        }

        private IQueryable<DTO.Document> GetFilteredQuery(LegalNotesEntities dbContext, Filters filters)
        {
            var query = GetDocumentsQuery(dbContext);
            if (filters.StartDate.HasValue)
                query = query.Where(x => x.ModifiedDate.HasValue && x.ModifiedDate > x.CreateDate ?
                                            x.ModifiedDate >= filters.StartDate :
                                            x.CreateDate >= filters.StartDate);
            if (filters.EndDate.HasValue)
            {
                var endDate = filters.EndDate.Value.AddDays(1);
                query = query.Where(x => x.ModifiedDate.HasValue && x.ModifiedDate > x.CreateDate ?
                                            x.ModifiedDate <= endDate :
                                            x.CreateDate <= endDate);
            }
            if (filters.NotarialActionId.HasValue)
                query = query.Where(x => x.NotarialAction.NotarialActionId == filters.NotarialActionId);
            if (filters.NotarialActionTypeId.HasValue)
                query = query.Where(x => x.NotarialActionsType.NotarialActionTypeId == filters.NotarialActionTypeId);
            if (filters.NotarialActionObjectId.HasValue)
                query = query.Where(x => x.NotarialActionsObject.NotarialActionObjectId == filters.NotarialActionObjectId);

            return query;
        }

        public IEnumerable<NotarialActionSum> GetNotarialActionsSums(Filters filters)
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
                            select new NotarialActionSum
                            {
                                NotarialAction = docsGroup.Key.NotarialAction,
                                NotarialActionsType = docsGroup.Key.NotarialActionsType,
                                NotarialActionsObject = docsGroup.Key.NotarialActionsObject,
                                Sum = docsGroup.Sum(x => x.Price)
                            };

                return query.ToList();
            }
        }

        public void AddDocument(DTO.Document newDocument)
        {
            using (var dbContext = new LegalNotesEntities())
            {
                var newDBDoc = new DAL.DB.Document
                {
                    CreateDate = DateTime.UtcNow,
                    Client = new DAL.DB.Client
                    {
                        CreateDate = DateTime.UtcNow,
                    }
                };

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

                existingDocument.ModifiedDate = DateTime.UtcNow;
                existingDocument.Client.ModifiedDate = DateTime.UtcNow;

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
            existingDocument.RecordNumber = document.RecordNumber;
            existingDocument.Price = document.Price;
            existingDocument.NotarialActionId = document.NotarialAction.NotarialActionId;
            existingDocument.NotarialActionTypeId = document.NotarialActionsType?.NotarialActionTypeId;
            existingDocument.NotarialActionObjectId = document.NotarialActionsObject?.NotarialActionObjectId;
            existingDocument.Client.PassportNumber = document.Client.PassportNumber;
            existingDocument.Client.PassportData = document.Client.PassportData;
            existingDocument.Client.Address = document.Client.Address;
            existingDocument.Client.Name = document.Client.Name;
            existingDocument.Client.LastName = document.Client.LastName;
            existingDocument.Client.MiddleName = document.Client.MiddleName;
        }

        private IQueryable<DTO.Document> GetDocumentsQuery(LegalNotesEntities dbContext)
        {
            return
                dbContext.Documents.Select(source => new DTO.Document
                {
                    DocumentId = source.DocumentId,
                    RecordNumber = source.RecordNumber,
                    CreateDate = source.CreateDate,
                    ModifiedDate = source.ModifiedDate,
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
                    Client = new DTO.Client
                    {
                        ClientId = source.Client.ClientId,
                        CreateDate = source.CreateDate,
                        PassportNumber = source.Client.PassportNumber,
                        PassportData = source.Client.PassportData,
                        Address = source.Client.Address,
                        Name = source.Client.Name,
                        LastName = source.Client.LastName,
                        MiddleName = source.Client.MiddleName,
                        ModifiedDate = source.Client.ModifiedDate
                    }
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
