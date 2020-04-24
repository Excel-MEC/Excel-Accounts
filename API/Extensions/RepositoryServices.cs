using API.Data;
using API.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class RepositoryServices
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            // Add Authrepository
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Add Profile
            services.AddScoped<IProfileRepository, ProfileRepository>();

            //Adding InstitutionRepository to the service
            services.AddScoped<IInstitutionRepository, InstitutionRepository>();

            //Adding AmbassadorRepository to the service
            services.AddScoped<IAmbassadorRepository, AmbassadorRepository>();
        }
    }
}