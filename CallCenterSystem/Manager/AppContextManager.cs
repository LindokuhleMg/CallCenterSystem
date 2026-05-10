
namespace CallCenterSystem.Manager
{
    /// <summary>
    /// Simulates the logged-in student context.
    /// These values would normally be retrieved after login.
    /// </summary>
    public static class AppContext
    {
        public static string CurrentStudentNumber { get; set; }
        public static string CurrentStudentName { get; set; }
        public static string CurrentTechnicianNumber { get; set; }
        public static string CurrentTechnicianName { get; set; }

        // Initialize with default values (simulate login)
        static AppContext()
        {
            CurrentStudentNumber = "202500123";
            CurrentStudentName = "Alice Mokoena";
            CurrentTechnicianNumber = "TechA";
            CurrentTechnicianName = "John Sbiko";
        }
    }
}
