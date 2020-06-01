using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WEBApiCore.Models
{
    public partial class ContactsDBContext : DbContext
    {
        public ContactsDBContext()
        {
        }

        public ContactsDBContext(DbContextOptions<ContactsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ContactModel> Contact { get; set; }
        public virtual DbSet<ContactSkillExpertiseModel> ContactSkillExpertise { get; set; }
        public virtual DbSet<ExpertiseLvLModel> ExpertiseLev { get; set; }
        public virtual DbSet<SkillModel> Skills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=ContactsDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactModel>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("Email")
                    .IsUnique();

                entity.HasIndex(e => e.MobileNum)
                    .HasName("MobileNum")
                    .IsUnique();

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNum)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ContactSkillExpertiseModel>(entity =>
            {
                entity.HasKey(e => e.ContactSkillId);

                entity.Property(e => e.ContactSkillId).HasColumnName("ContactSkillID");

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.ExpertiseLvlid).HasColumnName("ExpertiseLVLID");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                //entity.HasOne(d => d.Contact)
                //    .WithMany(p => p.ContactSkillExpertise)
                //    .HasForeignKey(d => d.ContactId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_ContactSkillExpertise_Contact");

                //entity.HasOne(d => d.ExpertiseLvl)
                //    .WithMany(p => p.ContactSkillExpertise)
                //    .HasForeignKey(d => d.ExpertiseLvlid)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_ContactSkillExpertise_ExpertiseLev");

                //entity.HasOne(d => d.Skill)
                //    .WithMany(p => p.ContactSkillExpertise)
                //    .HasForeignKey(d => d.SkillId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_ContactSkillExpertise_Skills");
            });

            modelBuilder.Entity<ExpertiseLvLModel>(entity =>
            {
                entity.HasKey(e => e.ExpertiseLvlid);

                entity.HasIndex(e => e.ExpertiseLevel)
                    .HasName("ExpertiseLevel")
                    .IsUnique();

                entity.Property(e => e.ExpertiseLvlid).HasColumnName("ExpertiseLVLID");

                entity.Property(e => e.ExpertiseLevel)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SkillModel>(entity =>
            {
                entity.HasKey(e => e.SkillId);

                entity.HasIndex(e => e.SkillName)
                    .HasName("SkillName")
                    .IsUnique();

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.Property(e => e.SkillName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });
        }
    }
}
