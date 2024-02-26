public class CustomClaim  {
    public string Type { get; set; }
    public string Value { get; set; }

    // Add other properties if needed

    public CustomClaim(string type, string value)
    {
        Type = type;
        Value = value;
    }
}