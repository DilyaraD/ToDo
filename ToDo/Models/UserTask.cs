using System;

namespace ToDo.Models
{
    public class UserTask
    {
        public int IdTask { get; set; }
        public string Title { get; set; }
        public int Property { get; set; }
        public string Category { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
