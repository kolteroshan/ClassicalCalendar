namespace NseApiDTO;

public class StrikeSnapshotDTO
{
    public string Strike { get; set; }
    public string Name { get; set; }
    public List<LtpDto> LtpDtos { get; set; }
    public decimal ClosePrice { get; set; }
    public string Type { get; set; }
    public DateOnly Date { get; set; }
}
