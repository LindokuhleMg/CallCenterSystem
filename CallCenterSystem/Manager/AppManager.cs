using static CallCenterSystem.Patterns.IteratorPattern;
using static CallCenterSystem.Patterns.ProxyPattern;

namespace CallCenterSystem.Manager
{
    // - CallLog (Iterator)
    // - PhoneProxy (Proxy which wraps RealPhone)
    public static class AppManager
    {
        public static CallLog CallLog { get; private set; }
        public static PhoneProxy PhoneProxy { get; private set; }

        public static void Initialize()
        {
            CallLog = new CallLog();
            PhoneProxy = new PhoneProxy(new RealPhone(), CallLog);

            if (string.IsNullOrEmpty(AppContext.CurrentStudentNumber))
            {
                AppContext.CurrentStudentNumber = "202500123";
                AppContext.CurrentStudentName = "Alice Mokoena";
            }
        }
    }
}
