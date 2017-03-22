using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class Document
    {
        public int DocumentId { get; set; }

        public virtual NotarialAction NotarialAction { get; set; }
        public virtual NotarialActionsObject NotarialActionsObject { get; set; }
        public virtual NotarialActionsType NotarialActionsType { get; set; }

        public virtual Client Client { get; set; }
        public decimal Price { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }


        public DateTime LastAccessdate { get { return ModifiedDate.HasValue && ModifiedDate > CreateDate ? ModifiedDate.Value : CreateDate; } }
    }
}
