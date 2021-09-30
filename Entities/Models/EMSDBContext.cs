using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Entities.Models
{
    public partial class EMSDBContext : DbContext
    {
        public EMSDBContext()
        {
        }

        public EMSDBContext(DbContextOptions<EMSDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivationToken> ActivationToken { get; set; }
        public virtual DbSet<Barangay> Barangay { get; set; }
        public virtual DbSet<CashAidNonVoter> CashAidNonVoter { get; set; }
        public virtual DbSet<CashAidRecepients> CashAidRecepients { get; set; }
        public virtual DbSet<Cluster> Cluster { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<ModuleControl> ModuleControl { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Policy> Policy { get; set; }
        public virtual DbSet<PolicyModuleControl> PolicyModuleControl { get; set; }
        public virtual DbSet<PolicyRoles> PolicyRoles { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<PollOption> PollOption { get; set; }
        public virtual DbSet<PollOptionAnswer> PollOptionAnswer { get; set; }
        public virtual DbSet<Precinct> Precinct { get; set; }
        public virtual DbSet<TempPrecint> TempPrecint { get; set; }
        public virtual DbSet<TempVoterlist> TempVoterlist { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Voter> Voter { get; set; }
        public virtual DbSet<Zone> Zone { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LPTP2018SD00034\\MSSQLSERVER2017;Initial Catalog=MNL-EMSDB;Persist Security Info=False;User ID=sa;Password=p@$$w0rd;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivationToken>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.ActivationToken)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivationToken_Person");
            });

            modelBuilder.Entity<Barangay>(entity =>
            {
                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.Barangay)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Barangay_Zone");
            });

            modelBuilder.Entity<CashAidNonVoter>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Brgy)
                    .WithMany(p => p.CashAidNonVoter)
                    .HasForeignKey(d => d.BrgyId)
                    .HasConstraintName("FK_CashAidNonVoter_Barangay");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.CashAidNonVoter)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CashAidNonVoter_Event");
            });

            modelBuilder.Entity<CashAidRecepients>(entity =>
            {
                entity.HasOne(d => d.Event)
                    .WithMany(p => p.CashAidRecepients)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CashAidRecepients_Event");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.CashAidRecepients)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CashAidRecepients_Person");
            });

            modelBuilder.Entity<Cluster>(entity =>
            {
                entity.HasOne(d => d.Brgy)
                    .WithMany(p => p.Cluster)
                    .HasForeignKey(d => d.BrgyId)
                    .HasConstraintName("FK_Cluster_Barangay");
            });

            modelBuilder.Entity<ModuleControl>(entity =>
            {
                entity.HasOne(d => d.Module)
                    .WithMany(p => p.ModuleControl)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModuleControl_Module");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne(d => d.Brgy)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.BrgyId)
                    .HasConstraintName("FK_Person_Barangay");

                entity.HasOne(d => d.Cluster)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.ClusterId)
                    .HasConstraintName("FK_Person_Cluster");

                entity.HasOne(d => d.Precinct)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.PrecinctId)
                    .HasConstraintName("FK_Person_Precinct");

                entity.HasOne(d => d.Voter)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.VoterId)
                    .HasConstraintName("FK_Person_Voter");
            });

            modelBuilder.Entity<PolicyModuleControl>(entity =>
            {
                entity.HasOne(d => d.ModuleControl)
                    .WithMany(p => p.PolicyModuleControl)
                    .HasForeignKey(d => d.ModuleControlId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PolicyModuleControl_ModuleControl");

                entity.HasOne(d => d.Policy)
                    .WithMany(p => p.PolicyModuleControl)
                    .HasForeignKey(d => d.PolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PolicyModuleControl_Policy");
            });

            modelBuilder.Entity<PolicyRoles>(entity =>
            {
                entity.HasOne(d => d.Policy)
                    .WithMany(p => p.PolicyRoles)
                    .HasForeignKey(d => d.PolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PolicyRoles_Policy");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.PolicyRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PolicyRoles_UserRole");
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<PollOption>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollOption)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PollOption_Poll");
            });

            modelBuilder.Entity<PollOptionAnswer>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.PollOption)
                    .WithMany(p => p.PollOptionAnswer)
                    .HasForeignKey(d => d.PollOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PollOptionAnswer_PollOption");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PollOptionAnswer)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PollOptionAnswer_User");
            });

            modelBuilder.Entity<Precinct>(entity =>
            {
                entity.HasOne(d => d.Cluster)
                    .WithMany(p => p.Precinct)
                    .HasForeignKey(d => d.ClusterId)
                    .HasConstraintName("FK_Precinct_Cluster");
            });

            modelBuilder.Entity<TempPrecint>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<TempVoterlist>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Person)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_User_Person");
            });

            modelBuilder.Entity<Voter>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Brgy)
                    .WithMany(p => p.Voter)
                    .HasForeignKey(d => d.BrgyId)
                    .HasConstraintName("FK_Voter_Barangay");

                entity.HasOne(d => d.Cluster)
                    .WithMany(p => p.Voter)
                    .HasForeignKey(d => d.ClusterId)
                    .HasConstraintName("FK_Voter_Cluster");

                entity.HasOne(d => d.Precinct)
                    .WithMany(p => p.Voter)
                    .HasForeignKey(d => d.PrecinctId)
                    .HasConstraintName("FK_Voter_Precinct");
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.HasOne(d => d.District)
                    .WithMany(p => p.Zone)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Zone_District");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
