using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities;
using ShyrochenkoPatterns.Domain.Entities.Chat;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Domain.Entities.Logging;
using ShyrochenkoPatterns.Domain.Entities.Payment;
using ShyrochenkoPatterns.Domain.Entities.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.DAL
{
    public class DataContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(500);
        }

        public virtual DbSet<UserToken> UserTokens { get; set; }
        public virtual DbSet<VerificationToken> VerificationTokens { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<SMSLog> SMSLogs { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<PushNotificationLog> PushNotificationLogs { get; set; }
        public virtual DbSet<ApplicationUserRole> AspNetUserRoles { get; set; }

        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<StripeSubscription> StripeSubscriptions { get; set; }
        public virtual DbSet<EmailRecipient> EmailRecipients { get; set; }
        public virtual DbSet<Poem> Poems { get; set; }
        public virtual DbSet<Proverb> Proverbs { get; set; }
        public virtual DbSet<Story> Stories { get; set; }

        #region Queries

        #endregion

        #region Chat

        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatUser> ChatUsers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        #endregion

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        #region Fluent API

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }

        #endregion

    }
}
