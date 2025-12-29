using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment.Models;


namespace StudentPerformanceManagment
{
    public class ApplicationDbContext:IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Subject> Subjects {  get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Student>()
                .HasOne(s => s.CourseGroup)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.CourseGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tasks>()
                .HasOne(t => t.Course)
                .WithMany()
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tasks>()
                .HasOne(t => t.Subject)
                .WithMany()
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tasks>()
                .HasOne(t => t.CourseGroup)
                .WithMany()
                .HasForeignKey(t => t.CourseGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tasks>()
                .HasOne(t => t.Staff)
                .WithMany(t => t.Tasks)
                .HasForeignKey(t => t.StaffId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Mark>()
                .HasOne(m => m.Tasks)
                .WithMany()
                .HasForeignKey(m => m.TasksId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Mark>()
                .HasIndex(t => t.SubjectId)
                .IsUnique(false);

            builder.Entity<Mark>()
                .HasIndex(m => new { m.SubjectId, m.StudentId })
                .IsUnique();
        }


    }

   
}
