namespace LowlandTech.Sample.Backend;

public class Sample
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
