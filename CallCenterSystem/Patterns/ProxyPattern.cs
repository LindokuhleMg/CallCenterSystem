using static CallCenterSystem.Patterns.IteratorPattern;
using static CallCenterSystem.Patterns.StatePattern;

namespace CallCenterSystem.Patterns
{
    public class ProxyPattern
    {
        //IPhone is the subject interface.
        public interface IPhone
        {
            void MakeCall(Call call);
            void DropCall(Call call);
            void HoldCall(Call call);
            void ResumeCall(Call call);
        }

        //RealPhone does the real actions on a Call(delegates to Call context).
        public class RealPhone : IPhone
        {
            public void MakeCall(Call call)
            {
                call.Speak();
            }
            public void DropCall(Call call)
            {
                call.HangUp();
            }
            public void HoldCall(Call call)
            {
                call.Hold();
            }
            public void ResumeCall(Call call)
            {
                call.Resume();
            }
        }

        //PhoneProxy also provides convenience method MakeCall(callId) that can lazy-load a Call from CallLog (virtual-proxy).
        public class PhoneProxy : IPhone
        {
            private RealPhone _realPhone;
            private CallLog _callLog;

            public PhoneProxy(RealPhone realPhone, CallLog callLog)
            {
                _realPhone = realPhone;
                _callLog = callLog;
            }

            public void MakeCall(Call call)
            {
                _realPhone.MakeCall(call);
                _callLog.Add(call);
            }

            public void DropCall(Call call)
            {
                if (_realPhone == null)
                    _realPhone= new RealPhone();

                _realPhone.DropCall(call);
            }
            public void HoldCall(Call call)
            {
                if (_realPhone == null)
                    _realPhone = new RealPhone();

                _realPhone.HoldCall(call);
            }
            public void ResumeCall(Call call)
            {
                if (_realPhone == null)
                    _realPhone = new RealPhone();

                _realPhone.ResumeCall(call);
            }
        }
    }
}
