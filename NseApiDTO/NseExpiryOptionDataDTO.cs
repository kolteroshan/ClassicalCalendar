namespace NseApiDTO;

public class NseExpiryOptionDataDTO
{
    public DateOnly ExpiryDate { get; set; }
    public decimal TotalOi { get; set; }
    public decimal TotalVolume { get; set; }
    public double LiveStrike { get; set; }
    public DateOnly ExpiryOptionDataDate { get; set; }
    public List<NseStrikeDTO> Strikes { get; set; }
}