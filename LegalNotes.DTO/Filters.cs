﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class Filters
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NotarialActionId { get; set; }
        public int? NotarialActionTypeId { get; set; }
        public int? NotarialActionObjectId { get; set; }
    }
}
