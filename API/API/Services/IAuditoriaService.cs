namespace API.Services
{
    public interface IAuditoriaService
    {
        void Registrar(string? usuario, string? accion, string? entidad, string? detalle, string? ip);
    }
}
