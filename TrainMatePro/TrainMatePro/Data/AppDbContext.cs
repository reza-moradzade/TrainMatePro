using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrainMatePro.Models;

namespace TrainMatePro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
        public DbSet<WorkoutWeek> WorkoutWeeks { get; set; }
        public DbSet<WorkoutDay> WorkoutDays { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseRef> ExerciseRefs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // تنظیم برای تمام خواص DateTime
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Unspecified
                                ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                                : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                }
            }
            // رابطه مربی-شاگرد (self-reference)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Coach)
                .WithMany(u => u.Students)
                .HasForeignKey(u => u.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            // رابطه User و StudentProfile (یک به یک)
            modelBuilder.Entity<User>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<StudentProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه StudentProfile و WorkoutProgram
            modelBuilder.Entity<WorkoutProgram>()
                .HasOne(w => w.Student)
                .WithMany(s => s.WorkoutPrograms)
                .HasForeignKey(w => w.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه Coach و WorkoutProgram
            modelBuilder.Entity<WorkoutProgram>()
                .HasOne(w => w.Coach)
                .WithMany(u => u.CoachPrograms)
                .HasForeignKey(w => w.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            // رابطه WorkoutProgram و WorkoutWeek
            modelBuilder.Entity<WorkoutWeek>()
                .HasOne(w => w.Program)
                .WithMany(p => p.WorkoutWeeks)
                .HasForeignKey(w => w.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه WorkoutWeek و WorkoutDay
            modelBuilder.Entity<WorkoutDay>()
                .HasOne(d => d.Week)
                .WithMany(w => w.WorkoutDays)
                .HasForeignKey(d => d.WeekId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه WorkoutDay و Exercise
            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Day)
                .WithMany(d => d.Exercises)
                .HasForeignKey(e => e.DayId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه Exercise و ExerciseRef
            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.ExerciseRef)
                .WithMany(r => r.Exercises)
                .HasForeignKey(e => e.ExerciseRefId)
                .OnDelete(DeleteBehavior.SetNull);

            // ایندکس برای جستجوی سریع
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ExerciseRef>()
                .HasIndex(e => e.ExerciseId)
                .IsUnique();
        }
    }
}