namespace Rift.Data.Auth
{
    public class WebCredential
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CredentialTypeId { get; set; }
        public string Identifier { get; set; }
        public string Secret { get; set; }
        
        public virtual WebUser User { get; set; }
        public virtual WebCredentialType CredentialType { get; set; }
    }
}
