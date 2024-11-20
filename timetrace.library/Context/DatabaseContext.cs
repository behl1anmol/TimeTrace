using Microsoft.EntityFrameworkCore;
using timetrace.library.Models;

namespace timetrace.library.Context;
public class DatabaseContext : DbContext
{
    public DbSet<Process> Processes
    {
        get; set;
    }

    public DbSet<ProcessDetail> ProcessDetails
    {
        get; set;
    }

    public DbSet<Image> Images
    {
        get; set;
    }

    public DbSet<ConfigurationSetting> ConfigurationSettings
    {
        get; set;
    }

    public DbSet<ConfigurationSettingDetail> ConfigurationSettingDetails
    {
        get; set;
    }

    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlite("Data Source=timetrace.db;Password=Password12!");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Process
        modelBuilder.Entity<Process>()
            .HasMany(p => p.ProcessDetails)
            .WithOne(pd => pd.Process)
            .HasForeignKey(pd => pd.ProcessId);

        modelBuilder.Entity<Process>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Process>()
            .Property(p => p.DateTimeStamp)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Process>()
            .Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();
        #endregion

        #region Process Detail
        modelBuilder.Entity<ProcessDetail>()
            .HasMany(pd => pd.Images)
            .WithOne(i => i.ProcessDetail)
            .HasForeignKey(i => i.ProcessDetailId);

        modelBuilder.Entity<ProcessDetail>()
            .HasIndex(pd => pd.Description)
            .IsUnique();

        modelBuilder.Entity<ProcessDetail>()
            .Property(p => p.DateTimeStamp)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProcessDetail>()
            .Property(p => p.Description)
            .HasMaxLength(255);

        #endregion

        #region ConfigurationSetting
        modelBuilder.Entity<ConfigurationSetting>()
            .HasMany(cs => cs.ConfigurationSettingDetails)
            .WithOne(csd => csd.ConfigurationSetting)
            .HasForeignKey(csd => csd.ConfigurationSettingId);

        modelBuilder.Entity<ConfigurationSetting>()
            .HasIndex(cs => cs.ConfigurationSettingIndex)
            .IsUnique();

        modelBuilder.Entity<ConfigurationSetting>()
            .Property(p => p.DateTimeStamp)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ConfigurationSetting>()
            .Property(p => p.ConfigurationSettingIndex)
            .HasMaxLength(50)
            .IsRequired();
        #endregion

        #region ConfigurationSettingDetail
        modelBuilder.Entity<ConfigurationSettingDetail>()
            .Property(p => p.DateTimeStamp)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ConfigurationSettingDetail>()
            .Property(p => p.ConfigurationSettingValue)
            .HasMaxLength(50);

        modelBuilder.Entity<ConfigurationSettingDetail>()
            .Property(p => p.ConfigurationSettingKey)
            .HasMaxLength(255);

        modelBuilder.Entity<ConfigurationSettingDetail>()
            .HasIndex(csd => csd.ConfigurationSettingKey)
            .IsUnique();
        #endregion

        #region Image
        modelBuilder.Entity<Image>()
            .HasIndex(i => i.ImageId)
            .IsUnique();

        modelBuilder.Entity<Image>()
            .Property(p => p.DateTimeStamp)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Image>()
            .Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Image>()
            .Property(p => p.ImagePath)
            .HasMaxLength(255);

        modelBuilder.Entity<Image>()
            .Property(p => p.ImageGuid)
            .IsRequired();
        #endregion

    }
}
