public class AccountPasswordSpecification : ISpecification<string>
{
    public string ErrorMessage { get; private set; }

    public bool IsSatisfiedBy(string password)
    {
        return !string.IsNullOrEmpty(password)
            && password.Length >= 6 && password.Length <= 12;
    }

    public string GetErrorMessage(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "Password cannot be null or empty";
        if (password.Length < 6 || password.Length > 12)
            return "Password must be between 6 and 12 characters long";
        return string.Empty;
    }
}

public class AccountHashPasswordSpecification : ISpecification<string>
{
    public string ErrorMessage { get; private set; }
    public string GetErrorMessage(string value)
    {
        throw new System.NotImplementedException();
    }

    public bool IsSatisfiedBy(string value)
    {
        throw new System.NotImplementedException();
    }
}