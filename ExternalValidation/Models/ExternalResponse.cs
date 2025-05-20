namespace ExternalValidation.Models;

public class ExternalResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int Statuscode { get; set; }
}
