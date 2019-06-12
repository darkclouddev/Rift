using System;
using System.Collections.Generic;

namespace Rift.Data.Auth
{
    public class WebUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public virtual ICollection<WebCredential> Credentials { get; set; }
    }
}
