using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ToDoListAPI.Entities
{
    // Entity Framework's context setup
    public partial class TasklistContext : DbContext
    {
        public TasklistContext()
        {
        }

        public TasklistContext(DbContextOptions<TasklistContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TableTasksList> TableTasksLists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-DSP6D9G\\SQLEXPRESS;Database=Tasklist;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TableTasksList>(entity =>
            {
                entity.ToTable("Table_TasksList");

                entity.Property(e => e.EntryDate).HasColumnType("datetime");

                entity.Property(e => e.Task).HasMaxLength(300);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
