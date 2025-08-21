namespace DTO;

public class ExpiryOptionDataDTO
{
    public DateOnly ExpiryDate { get; set; }
    public decimal TotalOi { get; set; }
    public decimal TotalVolume { get; set; }
    public double LiveStrike { get; set; }
    public DateOnly ExpiryOptionDataDate { get; set; }
    public List<StrikeDTO> Strikes { get; set; }
}