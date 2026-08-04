using Microsoft.EntityFrameworkCore;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasKey(x => x.Id);

            modelBuilder.Entity<Project>().Property(x => x.ProjectName).IsRequired();

            modelBuilder.Entity<Project>().Property(x => x.Description).IsRequired();
            
            modelBuilder.Entity<Board>().HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tasks>().HasOne(x => x.Board).WithMany(x => x.Tasks).HasForeignKey(x => x.BoardId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sprint>().HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        }
    
        public DbSet<Board> Boards { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Notification> Notifications { get; set; }

    }
}
