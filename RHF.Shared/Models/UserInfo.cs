using System.Security.Claims;

namespace RHF.Shared;
public class UserInfo{
    /// <summary>
    /// The email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// A value indicating whether the email has been confirmed yet.
    /// </summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>
    /// The list of roles for the user.
    /// </summary>
    public IList<CustomClaim> Claims { get; set; } = [];
}

