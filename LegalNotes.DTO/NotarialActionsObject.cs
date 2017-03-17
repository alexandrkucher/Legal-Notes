using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class NotarialActionsObject
    {
        public int NotarialActionObjectId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
