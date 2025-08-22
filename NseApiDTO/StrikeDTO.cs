namespace NseApiDTO;

public class StrikeDTO
{
    public double Strike { get; set; }
    public PutDTO? PutData { get; set; }
    public CallDTO? CallDTO { get; set; }
}