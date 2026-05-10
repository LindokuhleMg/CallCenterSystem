using System.Collections.Generic;
using System.Linq;
using static CallCenterSystem.Patterns.StatePattern;

namespace CallCenterSystem.Patterns
{
    public class IteratorPattern
    {
        //This is an Iterator Base
        public abstract class CallLogIteratorBase
        {
            public abstract Call First();
            public abstract Call Next();
            public abstract Call Previous();
            public abstract Call CurrentItem();
            public abstract bool IsDone();

            //helps when user is searching
            public abstract Call FindByStudentNumber(string studentNumber);
            public abstract Call FindByCallerName(string callerName);
        }

        //This is an Aggregate Base
        public abstract class CallLogBase
        {
            public abstract CallLogIteratorBase CreateIterator();
        }

        //This is an Concrete Aggregate
        public class CallLog : CallLogBase
        {
            private List<Call> _calls = new List<Call>();

            public override CallLogIteratorBase CreateIterator()
            {
                return new CallLogIterator(this);
            }

            public void Add(Call call)
            {
                _calls.Add(call);
            }

            public Call this[int index]
            {
                get { return _calls[index]; }
            }

            public int Count
            {
                get { return _calls.Count; }
            }

            public List<Call> GetAll() => _calls;
        }

        //This is an Concrete Iterator
        public class CallLogIterator : CallLogIteratorBase
        {
            private CallLog _callLog;
            private int _position;

            public CallLogIterator(CallLog callLog)
            {
                _callLog = callLog;
                _position = 0;
            }

            public override Call First()
            {
                _position = 0;
                return CurrentItem();
            }

            public override Call Next()
            {
                _position++;
                return CurrentItem();
            }

            public override Call Previous()
            {
                if (_position > 0) 
                    _position--;
                return CurrentItem();
            }

            public override Call CurrentItem() =>
                _position < _callLog.Count ? _callLog[_position] : null;

            public override bool IsDone() => _position >= _callLog.Count;

            public override Call FindByStudentNumber(string studentNumber)
            {
                var call = _callLog.GetAll().FirstOrDefault(c => c.StudentNumber == studentNumber);
                if (call != null) _position = _callLog.GetAll().IndexOf(call);
                return call;
            }

            public override Call FindByCallerName(string callerName)
            {
                var call = _callLog.GetAll().FirstOrDefault(c => c.CallerName == callerName);
                if (call != null) _position = _callLog.GetAll().IndexOf(call);
                return call;
            }
        }
    }
}
