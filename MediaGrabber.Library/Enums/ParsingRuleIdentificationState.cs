using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Enums
{
    public enum ParsingRuleIdentificationState
    {
        NotStarted = 1,
        Started = 2,
        Success = 3,
        Failed = 4,
        NeedsToBeDoneAgain = 5
    }
}
