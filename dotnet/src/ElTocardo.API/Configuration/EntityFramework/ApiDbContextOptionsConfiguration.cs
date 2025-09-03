using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;
using Microsoft.EntityFrameworkCore;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.API.Configuration.EntityFramework;

public sealed class ApiDbContextOptionsConfiguration : ElTocardoDbContextOptionsConfiguration
{
    public override void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(PostgresDatabaseResourceName));
    }

}
