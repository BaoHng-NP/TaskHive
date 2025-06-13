using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Enums;

namespace TaskHive.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Override SaveChangesAsync để set CreatedAt / UpdatedAt cho User
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (User)entry.Entity;
                entity.UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        #region DbSets for all Entities

        public DbSet<User> Users => Set<User>();
        public DbSet<Freelancer> Freelancers => Set<Freelancer>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<JobPost> JobPosts => Set<JobPost>();
        public DbSet<Application> Applications => Set<Application>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Membership> Memberships => Set<Membership>();
        public DbSet<UserMembership> UserMemberships => Set<UserMembership>();
        public DbSet<SlotPurchase> SlotPurchases => Set<SlotPurchase>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<UserSkill> UserSkills => Set<UserSkill>();

        public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------ Composite Keys ------

            // ConversationMember: composite key { ConversationId, UserId }
            modelBuilder.Entity<ConversationMember>()
                .HasKey(cm => new { cm.ConversationId, cm.UserId });

            // UserSkill: composite key { CategoryId, UserId }
            modelBuilder.Entity<UserSkill>()
                .HasKey(us => new { us.CategoryId, us.UserId });

            // === Cấu hình precision cho decimal ===

            modelBuilder.Entity<Application>()
                .Property(a => a.BidAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<JobPost>()
                .Property(j => j.SalaryMin)
                .HasPrecision(18, 2);

            modelBuilder.Entity<JobPost>()
                .Property(j => j.SalaryMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Membership>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SlotPurchase>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // === Các quan hệ đặc biệt ===

            // Review ↔ User (Reviewer, Reviewee)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.ReviewsWritten)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message ↔ User (Sender)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 1. Freelancer – UserSkill (n‐n qua bảng trung gian)
            //
            // Thay vì cố gắng tìm User.UserSkills, ta khai báo explicitly trên Freelancer
            modelBuilder.Entity<Freelancer>()
                .HasMany(f => f.UserSkills)
                .WithOne(us => us.User)       // UserSkill.User là kiểu User (thực ra phải gán vào Freelancer)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Category)
                .WithMany(c => c.UserSkills)
                .HasForeignKey(us => us.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 2. User – UserMembership (1‐n)
            //
            modelBuilder.Entity<UserMembership>()
                .HasOne(um => um.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMembership>()
                .HasOne(um => um.Membership)
                .WithMany(m => m.UserMemberships)
                .HasForeignKey(um => um.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 3. User – SlotPurchase (1‐n)
            //
            modelBuilder.Entity<SlotPurchase>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SlotPurchases)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 4. User – Payment (1‐n)
            //
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Membership)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.SlotPurchase)
                .WithMany(sp => sp.Payments)
                .HasForeignKey(p => p.SlotPurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 5. Category – JobPost (1‐n)
            //
            modelBuilder.Entity<JobPost>()
                .HasOne(j => j.Category)
                .WithMany(c => c.JobPosts)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 6. Client (kế thừa User) – JobPost (1‐n)
            //
            modelBuilder.Entity<JobPost>()
                .HasOne(j => j.Employer)
                .WithMany(c => c.JobPosts)
                .HasForeignKey(j => j.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 7. Application – JobPost & Freelancer
            //
            modelBuilder.Entity<Application>()
                .HasOne(a => a.JobPost)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Freelancer)
                .WithMany(f => f.Applications)
                .HasForeignKey(a => a.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 8. Review – JobPost
            //
            modelBuilder.Entity<Review>()
                .HasOne(r => r.JobPost)
                .WithMany(j => j.Reviews)
                .HasForeignKey(r => r.JobPostId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 9. BlogPost – User (Author)
            //
            modelBuilder.Entity<BlogPost>()
                .HasOne(b => b.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 10. Conversation – User (Creator)
            //
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.CreatedConversations)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // 11. Message – Conversation
            //
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            //
            // 12. ConversationMember – Conversation, User, Message (LastReadMessage)
            //
            modelBuilder.Entity<ConversationMember>()
                .HasOne(cm => cm.Conversation)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ConversationMemberships)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ưu tiên tránh cascade nhiều nhánh
            modelBuilder.Entity<ConversationMember>()
                .HasOne(cm => cm.LastReadMessage)
                .WithMany(m => m.ReadByMembers)
                .HasForeignKey(cm => cm.LastReadMessageId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc NoAction


            // Nếu bạn còn mapping nào khác, cứ thêm ở đây…


            modelBuilder.Entity<EmailVerificationToken>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Token)
                    .IsUnique();

                entity.HasIndex(e => new { e.UserId, e.Email });

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
