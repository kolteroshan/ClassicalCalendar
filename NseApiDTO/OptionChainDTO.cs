namespace NseApiDTO;

public class OptionChainDTO
{
    public DateOnly DateOfOptionChain { get; set; }

    public List<ExpiryOptionDataDTO> Expires { get; set; }
}
