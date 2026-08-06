using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<Tasks> Tasks => Set<Tasks>();
        public DbSet<Sprint> Sprints => Set<Sprint>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ProjectName).IsRequired();
                e.Property(x => x.Description).IsRequired();
            });

            modelBuilder.Entity<Board>()
                .HasOne(x => x.Project)
                .WithMany(p => p.Boards)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tasks>(e =>
            {
                e.HasOne(x => x.Board)
                    .WithMany(b => b.Tasks)
                    .HasForeignKey(x => x.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Sprint)
                    .WithMany(s => s.Tasks)
                    .HasForeignKey(x => x.SprintId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.AssignedEmployee)
                    .WithMany(emp => emp.AssignedTasks)
                    .HasForeignKey(x => x.AssignedEmployeeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Sprint>()
                .HasOne(x => x.Project)
                .WithMany(p => p.Sprints)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>(e =>
            {
                e.HasOne(x => x.Project)
                    .WithMany(p => p.Members)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Employee)
                    .WithMany(emp => emp.ProjectMembers)
                    .HasForeignKey(x => x.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.ProjectId, x.EmployeeId }).IsUnique();
            });

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.TaskItem)
                .WithMany(t => t.Comments)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attachment>(e =>
            {
                e.HasOne(x => x.TaskItem)
                    .WithMany(t => t.Attachments)
                    .HasForeignKey(x => x.TaskItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Project)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
