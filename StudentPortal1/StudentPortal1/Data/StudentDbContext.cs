using Microsoft.EntityFrameworkCore;
namespace StudentPortal1.Data
{
    public class StudentDbContext:DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Student);
                /*.WithMany(s => s.Payments)*/
                /*.HasForeignKey(p => p.StudentId);*/
        }



    }
}
