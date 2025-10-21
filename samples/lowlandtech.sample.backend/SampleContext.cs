namespace LowlandTech.Sample.Backend;

public class SampleContext(DbContextOptions options) : DbContext(options)
{
    public SampleContext() : this(new DbContextOptions<SampleContext>()) { }

    public DbSet<Sample> Samples => Set<Sample>();
}
