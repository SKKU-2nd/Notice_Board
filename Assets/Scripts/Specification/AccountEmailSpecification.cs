using System.Text.RegularExpressions;
using UnityEngine;

public class AccountEmailSpecification : ISpecification<string>
{
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    public string ErrorMessage { get; private set; }

    public bool IsSatisfiedBy(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ErrorMessage = "Email cannot be null or empty";
            return false;
        }

        if(!EmailRegex.IsMatch(email))
        {
            ErrorMessage = "Email format is invalid";
            return false;
        }

        return true;
    }

    public string GetErrorMessage(string email)
    {
        if (string.IsNullOrEmpty(email))
            return "Email cannot be null or empty";
        if (!EmailRegex.IsMatch(email))
            return "Email format is invalid";
        return string.Empty;
    }
}
