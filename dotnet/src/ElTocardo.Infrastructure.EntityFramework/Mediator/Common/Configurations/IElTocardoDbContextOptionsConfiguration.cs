using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;

public interface IElTocardoDbContextOptionsConfiguration : IDbContextOptionsConfiguration<ApplicationDbContext>,
    IElTocardoEntityFrameworkConfiguration;
