using System;

namespace Select_Lucky_Dog.Services
{
    public class SuspendAndResumeArgs : EventArgs
    {
        public SuspensionState SuspensionState { get; set; }

        public Type Target { get; private set; }

        public SuspendAndResumeArgs(SuspensionState suspensionState, Type target)
        {
            SuspensionState = suspensionState;
            Target = target;
        }
    }
}
