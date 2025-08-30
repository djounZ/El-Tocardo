using ElTocardo.Infrastructure.Mediator.EntityFramework.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Configurations;

public interface IElTocardoDbContextOptionsConfiguration : IDbContextOptionsConfiguration<ApplicationDbContext>,
    IElTocardoEntityFrameworkConfiguration;
