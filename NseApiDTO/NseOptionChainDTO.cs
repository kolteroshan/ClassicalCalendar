namespace NseApiDTO;

public class NseOptionChainDTO
{
    public DateOnly DateOfOptionChain { get; set; }

    public List<NseExpiryOptionDataDTO> Expires { get; set; }
}
