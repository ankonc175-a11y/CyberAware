namespace CyberAware.DataAccess.Entities;

public class ContentSection
{
    public int SectionID { get; set; }
    public int ModuleID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentBody { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public Module Module { get; set; } = null!;
}
