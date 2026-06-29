namespace API.Services
{
    public class AuthorizationResponse
    {
        public string? Token { get; set; }
        public bool Result { get; set; }
        public string? Msj { get; set; }
        public string? Email { get; set; }
        public DateTime? Expira { get; set; }
    }
}
