using BankAccountSimulation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Principal;
using BankAccountSimulation.Domain.Enums;

namespace BankAccountSimulation.Infra.Persistence.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("bank_accounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Balance)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0);

        builder.Property(x => x.Role)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .HasDefaultValue("User");

        builder.Property(x => x.PasswordHash)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(AccountStatus.Active);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(x => x.AccountNumber)
            .IsUnique();

        //không thiết lập mối quan hệ mà query qua linq để tránh n+1 query
        //builder.HasOne<BankAccount>()            
        //   .WithMany(a => a.)
        //   .HasForeignKey(t => t.AccountId) 
        //   .OnDelete(DeleteBehavior.Cascade);

    }
}
