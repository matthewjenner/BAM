using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StargateAPI.Business.Data;

public class StargateContext : DbContext
{
    public IDbConnection Connection => Database.GetDbConnection();
    public DbSet<Person> People { get; set; }
    public DbSet<AstronautDetail> AstronautDetails { get; set; }
    public DbSet<AstronautDuty> AstronautDuties { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }

    public StargateContext(DbContextOptions<StargateContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StargateContext).Assembly);

        // Only seed data in development environment, not during testing
        if (!IsTestEnvironment())
        {
            SeedData(modelBuilder);
        }

        base.OnModelCreating(modelBuilder);
    }

    private static bool IsTestEnvironment()
    {
        // Check if we're running in a test environment
        bool testAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .Any(assembly => assembly.FullName?.Contains("xunit") == true || 
                            assembly.FullName?.Contains("testhost") == true ||
                            assembly.FullName?.Contains("Microsoft.TestPlatform") == true);
        
        return testAssembly;
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Only seed data if the database is empty
        // This is a development convenience - in production, use proper data migration strategies
        modelBuilder.Entity<Person>()
            .HasData(
                new Person
                {
                    Id = 1,
                    Name = "John Doe"
                },
                new Person
                {
                    Id = 2,
                    Name = "Jane Doe"
                }
            );

        modelBuilder.Entity<AstronautDetail>()
            .HasData(
                new AstronautDetail
                {
                    Id = 1,
                    PersonId = 1,
                    CurrentRank = "1LT",
                    CurrentDutyTitle = "Commander",
                    CareerStartDate = DateTime.Now
                }
            );

        modelBuilder.Entity<AstronautDuty>()
            .HasData(
                new AstronautDuty
                {
                    Id = 1,
                    PersonId = 1,
                    DutyStartDate = DateTime.Now,
                    DutyTitle = "Commander",
                    Rank = "1LT"
                }
            );
    }
}