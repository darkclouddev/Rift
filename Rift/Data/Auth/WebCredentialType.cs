using System.Collections.Generic;

namespace Rift.Data.Auth
{
    public class WebCredentialType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Position { get; set; }
        
        public virtual ICollection<WebCredential> Credentials { get; set; }
    }
}
