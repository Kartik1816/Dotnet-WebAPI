namespace TestAPI.Domain.ViewModels;

public static class DatabaseObject
{
     // Stored Procedure Names
    public const string SpSaveUser = "sp_save_user";
    public const string SpDeleteUser = "sp_delete_user";

    // Function Names
    public const string FnGetUserById = "fn_get_user_by_id";
    public const string FnGetAllUsers = "fn_get_all_users";
}
