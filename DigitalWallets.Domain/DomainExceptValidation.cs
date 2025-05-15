namespace DigitalWallets.Domain;

internal class DomainExceptValidation : Exception
{
    public DomainExceptValidation(string error) : base(error)
    {
    }
    public static void When(bool hasError, string error)
    {
        if (hasError)
            throw new DomainExceptValidation(error);
    }
}
