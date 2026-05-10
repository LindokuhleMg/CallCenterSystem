using System;
using System.Collections;
using System.Collections.Generic;
using static CallCenterSystem.Patterns.StatePattern;

namespace CallCenterSystem.Patterns
{
    public class CompositePattern
    {
        //This is a base Component (abstract class)
        public abstract class CallComponent : IEnumerable<CallComponent>
        {
            public abstract void DisplayCall(); // display call information

            public virtual void Add(CallComponent component) { }
            public virtual void Remove(CallComponent component) { }

            public abstract IEnumerator<CallComponent> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        //Composite: a group of calls (or sub-groups)
        public class GroupedCalls : CallComponent
        {
            private readonly List<CallComponent> _children = new List<CallComponent>();
            public string Name { get; private set; }

            public GroupedCalls(string name)
            {
                Name = name;
            }

            public override void Add(CallComponent component)
            {
                _children.Add(component);
            }

            public override void Remove(CallComponent component)
            {
                _children.Remove(component);
            }

            public override void DisplayCall()
            {
                Console.WriteLine($"Group: {Name} ({_children.Count} items)");
                foreach (var child in _children)
                {
                    child.DisplayCall();
                }
            }

            public override IEnumerator<CallComponent> GetEnumerator()
            {
                return _children.GetEnumerator();
            }
        }

        //Leaf: represent a single call
        public class CallLeaf : CallComponent
        {
            public Call Call { get; private set; }

            public CallLeaf(Call call)
            {
                Call = call;
            }

            public override void DisplayCall()
            {
                Console.WriteLine($"Call: {Call.StudentNumber} - {Call.CallerName} [{Call.StateName}]");
            }

            public override IEnumerator<CallComponent> GetEnumerator()
            {
                // Leaves have no children
                return new List<CallComponent>().GetEnumerator();
            }
        }
    }
}
