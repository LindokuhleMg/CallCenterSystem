using System;

namespace CallCenterSystem.Patterns
{
    public class StatePattern
    {
        // This is the state base which defines the behaviour for call states (We are using an Interface)
        public interface ICallState
        {
            void Speak(Call call);
            void Hold(Call call);
            void Resume(Call call);
            void HangUp(Call call);
            string GetName(); // used to tell which state we are in.
        }

        // ConcreteState: OnCall which inherit from ICallState (User can move to different states based upon this current state)
        public class OnCallState : ICallState
        {
            public void Speak(Call call) { } // User already speaking
            public void Hold(Call call)
            {
                call.SetState(new OnHoldState());
            }
            public void Resume(Call call) { } // User already active
            public void HangUp(Call call)
            {
                call.SetState(new HungUpState());
            }
            public string GetName() => "On Call (User can Speak)";
        }

        // ConcreteState: OnHold which inherit from ICallState (User can move to different states based upon this current state)
        public class OnHoldState : ICallState
        {
            public void Speak(Call call) { } // User cannot speak
            public void Hold(Call call) { }  // Already on hold
            public void Resume(Call call)
            {
                call.SetState(new OnCallState());
            }
            public void HangUp(Call call)
            {
                call.SetState(new HungUpState());
            }
            public string GetName() => "On Hold (Cannot speak)";
        }

        // ConcreteState: HungUp/Ended which inherit from ICallState (User can not move to different states)
        public class HungUpState : ICallState
        {
            public void Speak(Call call) { }  // cannot speak
            public void Hold(Call call) { }   // cannot hold
            public void Resume(Call call) { } // cannot resume
            public void HangUp(Call call) { } // already hung up
            public string GetName() => "Call Ended";
        }

        // Context: Call
        public class Call
        {
            private ICallState _state;   // current state object

            // Call original properties
            public string StudentNumber { get; private set; }
            public string CallerName { get; private set; }   // the "name" provided when the Call was created
            public bool FromTechnician { get; private set; }
            public DateTime CreatedAt { get; private set; }
            public DateTime? EndedAt { get; private set; }

            // NEW: optional initiator info (keeps backward compatibility)
            // When a technician initiates a call, store who initiated it.
            public string InitiatorNumber { get; private set; }
            public string InitiatorName { get; private set; }

            // Active-time tracking
            private TimeSpan _accumulatedActiveTime = TimeSpan.Zero;
            private DateTime _lastStateChangeTime;

            //Constructor 
            public Call(string studentNumber, string callerName, bool fromTechnician, string initiatorNumber = null, string initiatorName = null)
            {
                StudentNumber = studentNumber;
                CallerName = callerName;
                FromTechnician = fromTechnician;
                CreatedAt = DateTime.Now;
                _lastStateChangeTime = DateTime.Now;

                InitiatorNumber = initiatorNumber;
                InitiatorName = initiatorName;

                // initial state is "on call"
                _state = new OnCallState();
            }

            public string StateName => _state.GetName();

            // Change state safely and accumulate active time when leaving OnCall
            public void SetState(ICallState state)
            {
                var now = DateTime.Now;

                // Accumulate active speaking time if previous state was OnCall
                if (_state is OnCallState)
                    _accumulatedActiveTime += now - _lastStateChangeTime;

                // Record when state changed
                _lastStateChangeTime = now;

                // If the new state is HungUp, record ended time
                if (state is HungUpState)
                {
                    EndedAt = now;
                }

                _state = state;
            }

            // These delegate actions to current state
            public void Speak()
            {
                _state.Speak(this);
            }
            public void Hold()
            {
                _state.Hold(this);
            }
            public void Resume()
            {
                _state.Resume(this);
            }
            public void HangUp()
            {
                _state.HangUp(this);
            }

            // Returns total active (speaking) time as string
            public string DurationString()
            {
                TimeSpan duration = _accumulatedActiveTime;
                if (_state is OnCallState)
                    duration += DateTime.Now - _lastStateChangeTime;
                return duration.ToString(@"hh\:mm\:ss");
            }

            // Returns total active (speaking) time as TimeSpan which is useful for UI timers
            public TimeSpan GetCurrentDuration()
            {
                TimeSpan duration = _accumulatedActiveTime;
                if (_state is OnCallState)
                    duration += DateTime.Now - _lastStateChangeTime;
                return duration;
            }
        }
    }
}
