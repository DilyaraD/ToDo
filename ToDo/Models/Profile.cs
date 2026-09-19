using System.Collections.Generic;

namespace ToDo.Models
{
    public class Profile
    {
        public int IdProfile { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}
