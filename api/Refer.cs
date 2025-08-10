namespace Web
{
    public enum Status
    {
        Success = 0,
        GeneralError = 1,
        LoginFail = 2,
        NotFound = 3,
        Duplicate = 4,
        DuplicateEmail = 5,
        Expired = 6,
        NotEmpty = 7,
        NotEnough = 8,
        WaitVerify = 9,
        Banned = 10,
        Empty = 11,
        AccountBlocked = 12,
        NeedVerifyEmail = 13,
        Forbidden = 14,
        AccountLocked = 15,
        DuplicateOldPassword = 16,
        LowPasswordComplexity = 17,
    }
}
