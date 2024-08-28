using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Models;

namespace TodoListApi.Data;

public partial class TodoListContext : DbContext
{
    public TodoListContext()
    {
    }

    public TodoListContext(DbContextOptions<TodoListContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Todo> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prioriti__3214EC07BF4A828B");

            entity.Property(e => e.Color)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3214EC079634D469");

            entity.ToTable("Status");

            entity.Property(e => e.Color)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07E97A8CE2");

            entity.Property(e => e.Color)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Todos__3214EC07947B0242");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Priority).WithMany(p => p.Todos)
                .HasForeignKey(d => d.PriorityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Todos_Priorities");

            entity.HasOne(d => d.Status).WithMany(p => p.Todos)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Todos_Status");

            entity.HasMany(d => d.Tags).WithMany(p => p.Todos)
                .UsingEntity<Dictionary<string, object>>(
                    "TodoTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_TodoTags_Tags"),
                    l => l.HasOne<Todo>().WithMany()
                        .HasForeignKey("TodoId")
                        .HasConstraintName("FK_TodoTags_Todos"),
                    j =>
                    {
                        j.HasKey("TodoId", "TagId").HasName("PK__TodoTags__43D1EAC8652C39B3");
                        j.ToTable("TodoTags");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
