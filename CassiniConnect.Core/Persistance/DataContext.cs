using CassiniConnect.Core.Models.EventCalendar;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Models.Presentation;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Models.Teaching.Group;
using CassiniConnect.Core.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Core.Persistance
{
    /// <summary>
    /// Adatkontextus - összes tábló elérőhelye, összeköttetés közvetlenül az adatbázissal
    /// Örököl az IdentityDbContext osztályból, egy sajátos User és Role entitást használva
    /// </summary>
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DataContext(DbContextOptions options) : base(options) { }
        #region EventCalendar
        public DbSet<Event> Events { get; set; }
        public DbSet<EventDetail> EventDetails { get; set; }
        #endregion
        #region Teaching
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectName> SubjectNames { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherDescription> TeacherDescriptions { get; set; }
        public DbSet<TeachingLocation> TeachingLocations { get; set; }
        public DbSet<TutoringAppointment> TutoringAppointments { get; set; }
        public DbSet<TutoringSession> TutoringSessions { get; set; }
        #region Teaching.Group
        public DbSet<GroupActivity> GroupActivities { get; set; }
        public DbSet<GroupActivityDetail> GroupActivityDetails { get; set; }
        #endregion
        #endregion
        #region Presentation
        public DbSet<Presenter> Presenters { get; set; }
        public DbSet<PresenterBooking> PresenterBookings { get; set; }
        public DbSet<PresenterDetail> PresenterDetails { get; set; }
        public DbSet<PresenterSession> PresenterSessions { get; set; }
        #endregion
        #region Helpers
        public DbSet<LanguageCode> LanguageCodes { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region User renames
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            #endregion

            #region unique constraints
            builder.Entity<Subject>().HasIndex(s => s.Code).IsUnique();
            builder.Entity<Teacher>().HasIndex(t => t.UserId).IsUnique();
            builder.Entity<Presenter>().HasIndex(p => p.UserId).IsUnique();
            builder.Entity<LanguageCode>().HasIndex(l => l.Code).IsUnique();
            #endregion
        }
    }
}