namespace ElTocardo.ServiceDefaults;

public class PersistentCertificate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte[] PfxBytes { get; set; } = null!;
    public string Password { get; set; } = null!;
}
