namespace CourierManagementSystem.Helpers
{
    public static class PasswordHelper
    {
        // This converts "123" into a long random string (Hash)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }




        // This checks if the login password matches the stored hash
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
