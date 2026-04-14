using BankAccountSimulation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankAccountSimulation.Domain.Enums;


namespace BankAccountSimulation.Infra.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("numeric(18,2)") //pg dùng numeric
               .HasDefaultValue(0);

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        
    }
}
