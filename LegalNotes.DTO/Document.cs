﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int RecordNumber { get; set; }

        public virtual NotarialAction NotarialAction { get; set; }
        public virtual NotarialActionsObject NotarialActionsObject { get; set; }
        public virtual NotarialActionsType NotarialActionsType { get; set; }

        public decimal Price { get; set; }
        public DateTime Date { get; set; }

        public Guid? ClientId { get; set; }
    }
}
