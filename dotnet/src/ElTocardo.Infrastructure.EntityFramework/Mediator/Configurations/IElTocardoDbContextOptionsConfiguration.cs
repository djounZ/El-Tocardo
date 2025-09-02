using ElTocardo.Infrastructure.EntityFramework.Mediator.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Configurations;

public interface IElTocardoDbContextOptionsConfiguration : IDbContextOptionsConfiguration<ApplicationDbContext>,
    IElTocardoEntityFrameworkConfiguration;
