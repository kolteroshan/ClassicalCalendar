namespace DTO;

public class OptionChainDto
{
    public DateOnly DateOfOptionChain { get; set; }

    public List<ExpiryOptionDataDTO> Expires { get; set; }
}