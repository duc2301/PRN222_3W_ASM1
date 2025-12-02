using ClubManagement.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ClubManagement.Repository.DbContexts;

public partial class ClubManagementContext : DbContext
{
    public ClubManagementContext()
    {
    }

    public ClubManagementContext(DbContextOptions<ClubManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityParticipant> ActivityParticipants { get; set; }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<Fee> Fees { get; set; }

    public virtual DbSet<JoinRequest> JoinRequests { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("server=(local); database=ClubManagement; uid=sa; pwd=12345; TrustServerCertificate=True;");

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Activiti__482FBD63931E4626");

            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.ActivityName)
                .HasMaxLength(150)
                .HasColumnName("activity_name");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");

            entity.HasOne(d => d.Club).WithMany(p => p.Activities)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Activitie__club___5CD6CB2B");
        });

        modelBuilder.Entity<ActivityParticipant>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("PK__Activity__4E037806CB23A165");

            entity.HasIndex(e => new { e.ActivityId, e.UserId }, "UQ_Activity_User").IsUnique();

            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.CheckInTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("check_in_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Attended")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityParticipants)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActivityP__activ__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActivityP__user___6383C8BA");
        });

        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__Clubs__BCAD3DD992300BF8");

            entity.HasIndex(e => e.ClubName, "UQ__Clubs__D48E100B70F6318E").IsUnique();

            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.ClubName)
                .HasMaxLength(100)
                .HasColumnName("club_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LeaderId).HasColumnName("leader_id");

            entity.HasOne(d => d.Leader).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.LeaderId)
                .HasConstraintName("FK__Clubs__leader_id__403A8C7D");
        });

        modelBuilder.Entity<Fee>(entity =>
        {
            entity.HasKey(e => e.FeeId).HasName("PK__Fees__A19C8AFB9BBE95AD");

            entity.Property(e => e.FeeId).HasColumnName("fee_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.Club).WithMany(p => p.Fees)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Fees__club_id__534D60F1");
        });

        modelBuilder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__JoinRequ__18D3B90F9EA3210B");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("request_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Club).WithMany(p => p.JoinRequests)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JoinReque__club___4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.JoinRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JoinReque__user___4E88ABD4");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__Membersh__CAE49DDD7F86C700");

            entity.HasIndex(e => new { e.UserId, e.ClubId }, "UQ_Member").IsUnique();

            entity.Property(e => e.MembershipId).HasColumnName("membership_id");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Member")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Club).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Membershi__club___48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Membershi__user___47DBAE45");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EA338AFF14");

            entity.HasIndex(e => new { e.FeeId, e.UserId }, "UQ_Payment").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.FeeId).HasColumnName("fee_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Paid")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Fee).WithMany(p => p.Payments)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__fee_id__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__user_i__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F0F357DEB");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164E99192E7").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572A1688C1A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Student")
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
