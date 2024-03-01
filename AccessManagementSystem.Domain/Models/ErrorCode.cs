namespace AccessManagementSystem.Domain.Models
{
    public enum ErrorCode
    {
        Error,
        ValidationError,
        InvalidLoginError,
        ExisitingAccountError,
        RequiredEmailError,
        NotRegisteredUser,
        NotRegisteredRole,
        NotRegisteredDoor
    }
}