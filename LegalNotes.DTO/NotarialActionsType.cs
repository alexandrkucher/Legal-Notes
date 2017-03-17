using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class NotarialActionsType
    {
        public int NotarialActionTypeId { get; set; }
        public string Name { get; set; }

        public IEnumerable<NotarialActionsObject> NotarialActionsObjects { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
