using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Commivoyager;

public partial class TspContext : DbContext
{
    public TspContext()
    {
    }

    public TspContext(DbContextOptions<TspContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<UserException> UserExceptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TSP;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Requests_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BestPath).HasColumnType("character varying");
            entity.Property(e => e.Matrix).HasMaxLength(128);
        });

        modelBuilder.Entity<UserException>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserException_pkey");

            entity.ToTable("UserException");

            entity.Property(e => e.DateTimeExc)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dateTimeExc");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
