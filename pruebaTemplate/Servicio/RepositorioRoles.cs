using Dapper;
using Microsoft.Data.SqlClient;
using PlanillaPM.Models;


namespace PlanillaPM.Servicio
{

    public interface IRepositorioRoles
    {
        Task<TipoRol> BuscarRolPorNombre(string nombreNormalizado);
        Task<string> AgregarRol(TipoRol rol);
        Task<int> EliminarRol(string nombreNormalizado);
    }

    public class RepositorioRoles : IRepositorioRoles
    {
        private readonly string connectionString;

        public RepositorioRoles(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<string> AgregarRol(TipoRol rol)
        {
            using var connection = new SqlConnection(connectionString);
            var rolId = await connection.QuerySingleAsync<int>(@"
                    INSERT INTO AspNetRoles (Id, [Name], NormalizedName)
                    VALUES (@Id, @Name, @NormalizedName);
                    SELECT SCOPE_IDENTITY();
                    ", rol);

            return rol.Id;
        }

        public async Task<int> EliminarRol(string nombreNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            var rowsAffected = await connection.ExecuteAsync(@"
                    DELETE FROM AspNetRoles WHERE NormalizedName = @nombreNormalizado;
                    ", new { nombreNormalizado });

            return rowsAffected;
        }

        public async Task<TipoRol> BuscarRolPorNombre(string nombreNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            var rol = await connection.QuerySingleOrDefaultAsync<TipoRol>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName = @nombreNormalizado",
                new { nombreNormalizado });

            return rol;
        }
    }




}
