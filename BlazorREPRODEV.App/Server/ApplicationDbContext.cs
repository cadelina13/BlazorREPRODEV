using BlazorREPRODEV.App.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Server
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DatabaseConfig> DatabaseConfigs { get; set; }
        public DbSet<ComponentSubject> ComponentSubjects { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<RegisteredStudent> RegisteredStudents { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamRecord> ExamRecords { get; set; }
        public DbSet<ExamStudentRecord> ExamStudentRecord { get; set; }
        public DbSet<AdminAnnouncement> AdminAnnouncements { get; set; }
        public DbSet<UserChat> UserChats { get; set; }
        public DbSet<Materials> Materials { get; set; }

    }
}
