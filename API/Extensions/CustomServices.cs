using API.Services;
using API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class CustomServices
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            // Adding Environment Service
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            
            // Adding AuthService along with HttpClient Service
            services.AddHttpClient<IAuthService, AuthService>();

            // Add Services For Profile
            services.AddScoped<IProfileService, ProfileService>();

            // Add Google Cloud Storage
            services.AddSingleton<ICloudStorage, GoogleCloudStorage>();

            //Adding QRCode Creation to the service
            services.AddSingleton<IQRCodeGeneration, QRCodeGeneration>();

            //Adding CipherRepository to the service
            services.AddSingleton<ICipherService, CipherService>();

        }
    }
}