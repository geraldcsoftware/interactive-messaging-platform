using MessageInteractionService.Storage.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageInteractionService.Storage.Configuration;

public class MenuDefinitionEntityConfiguration : IEntityTypeConfiguration<MenuDefinition>
{
    public void Configure(EntityTypeBuilder<MenuDefinition> builder)
    {
        builder.ToTable("MenuDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);
        builder.Property(x => x.ParentMenuId).IsRequired(false);
        builder.Property(x => x.DisplayText).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.OptionText).IsRequired(false).HasMaxLength(200);
        builder.Property(x => x.DisplayOrder).IsRequired().HasDefaultValueSql("0");
        builder.Property(x => x.HandlerName).IsRequired(false).IsUnicode().HasMaxLength(400);
    }
}