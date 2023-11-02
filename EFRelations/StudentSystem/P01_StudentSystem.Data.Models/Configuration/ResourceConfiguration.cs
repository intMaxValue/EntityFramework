using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StudentSystem.P01_StudentSystem.Data.Models.Configuration
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {

            builder
                .Property(r => r.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true);

            builder
                .Property(r => r.Url)
                    .IsUnicode(false);
        }
    }
}
