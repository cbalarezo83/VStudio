using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TestMakerFreeWebApp.Data.Models;

namespace TestMakerFreeWebApp.Data
{
    public class ApplicationDbContext:DbContext
    {
        #region Constructor
        public ApplicationDbContext(DbContextOptions options) :
          base(options)
        {
        }
        #endregion Constructor

        #region Methods

        protected override void OnModelCreating(ModelBuilder modelbuilder) {

            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<ApplicationUser>().ToTable("Users");
            modelbuilder.Entity<ApplicationUser>().HasMany(u => u.Quizzes).WithOne(i => i.User);

            modelbuilder.Entity<Quiz>().ToTable("Quizzes");
            modelbuilder.Entity<Quiz>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelbuilder.Entity<Quiz>().HasOne(i => i.User).WithMany(u => u.Quizzes);
            modelbuilder.Entity<Quiz>().HasMany(q => q.Questions).WithOne(c => c.Quiz);

            modelbuilder.Entity<Question>().ToTable("Questions");
            modelbuilder.Entity<Question>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelbuilder.Entity<Question>().HasOne(o => o.Quiz).WithMany(m => m.Questions);
            modelbuilder.Entity<Question>().HasMany(m => m.Answers).WithOne(m => m.Question);

            modelbuilder.Entity<Answer>().ToTable("Answers");
            modelbuilder.Entity<Answer>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelbuilder.Entity<Answer>().HasOne(o => o.Question).WithMany(m => m.Answers);

            modelbuilder.Entity<Result>().ToTable("Results");
            modelbuilder.Entity<Result>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelbuilder.Entity<Result>().HasOne(o => o.Quiz).WithMany(m => m.Results);




        }
        #endregion  

        #region Properties
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        #endregion Properties
    }
}
