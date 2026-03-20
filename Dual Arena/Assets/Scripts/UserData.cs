[System.Serializable]
public class User
{
    public string username;
    public string password;
}

[System.Serializable]
public class UserListWrapper
{
    public System.Collections.Generic.List<User> users;
}