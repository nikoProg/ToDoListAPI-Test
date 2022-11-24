using System;
using System.Collections.Generic;

#nullable disable

namespace ToDoListAPI.Entities
{
    public partial class TableTasksList
    {
        public int? Id { get; set; } // Id is set to nullable because the DB itself automatically increments Id, deleted Ids are never repeated
        public string Task { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
