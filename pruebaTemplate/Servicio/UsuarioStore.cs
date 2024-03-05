using Microsoft.AspNetCore.Identity;
using PlanillaPM.Models;


namespace PlanillaPM.Servicio
{
   
        public class UsuarioStore : IUserRoleStore<TipoRol>
        {

        private readonly IRepositorioRoles repositorioRoles;

        public UsuarioStore(IRepositorioRoles repositorioRoles)
        {
            this.repositorioRoles = repositorioRoles;
        }

        public Task AddToRoleAsync(TipoRol user, string roleName, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

        public async Task<IdentityResult> CreateAsync(TipoRol user, CancellationToken cancellationToken)
        {
            user.Id = await repositorioRoles.AgregarRol(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(TipoRol user, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
               
            }

            public Task<TipoRol> FindByIdAsync(string userId, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public async Task<TipoRol> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
            {
            return await repositorioRoles.BuscarRolPorNombre(normalizedUserName);
            }

            public Task<string> GetNormalizedUserNameAsync(TipoRol user, CancellationToken cancellationToken)
            {
            throw new NotImplementedException();
        }

            public Task<IList<string>> GetRolesAsync(TipoRol user, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<string> GetUserIdAsync(TipoRol user, CancellationToken cancellationToken)
            {
                return Task.FromResult(user.Id.ToString());
            }

            public Task<string> GetUserNameAsync(TipoRol user, CancellationToken cancellationToken)
            {
                return Task.FromResult(user.Name);
            }

            public Task<IList<TipoRol>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<bool> IsInRoleAsync(TipoRol user, string roleName, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task RemoveFromRoleAsync(TipoRol user, string roleName, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task SetNormalizedUserNameAsync(TipoRol user, string normalizedName, CancellationToken cancellationToken)
            {
            throw new NotImplementedException();
            }

            public Task SetUserNameAsync(TipoRol user, string userName, CancellationToken cancellationToken)
            {
                user.Name = userName;
                return Task.CompletedTask;
        }

            public Task<IdentityResult> UpdateAsync(TipoRol user, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    
}
