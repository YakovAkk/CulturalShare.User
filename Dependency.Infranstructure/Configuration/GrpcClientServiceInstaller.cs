using AuthenticationBackProto;
using AuthenticationProto;
using CulturalShare.Foundation.AspNetCore.Extensions.GrpcInterceptors;
using CulturalShare.Foundation.EnvironmentHelper.EnvHelpers;
using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;

namespace Dependency.Infranstructure.Configuration;

public class GrpcClientServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddSingleton<ValidationInterceptor>();

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<ValidationInterceptor>();
        }).AddJsonTranscoding();

        var sortOutCredentialsHelper = new SortOutCredentialsHelper(builder.Configuration);
        var grpcClientsUrlConfiguration = sortOutCredentialsHelper.GetGrpcClientsUrlConfiguration();

        builder.Services.AddGrpcClient<AuthenticationBackGrpcService.AuthenticationBackGrpcServiceClient>(options =>
        {
            options.Address = new Uri(grpcClientsUrlConfiguration.AuthClientUrl);
        });

        logger.Information($"{nameof(GrpcClientServiceInstaller)} installed.");
    }
}
