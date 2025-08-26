namespace NseApiDTO;

public class NseStrikeDTO
{
    public double Strike { get; set; }
    public NsePutDTO? PutData { get; set; }
    public NseCallDTO? CallDTO { get; set; }
}