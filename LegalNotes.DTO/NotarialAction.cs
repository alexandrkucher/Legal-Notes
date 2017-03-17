using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalNotes.DTO
{
    public class NotarialAction
    {
        public int NotarialActionId { get; set; }
        public string Name { get; set; }

        public IEnumerable<NotarialActionsType> NotarialActionsTypes { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
