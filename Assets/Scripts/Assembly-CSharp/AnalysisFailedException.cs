using System;

public sealed class AnalysisFailedException : Exception
{
    public AnalysisFailedException() { }
    public AnalysisFailedException(string message) : base() { } // base(message) { }
    public AnalysisFailedException(string message, Exception inner) : base() { } // base(message, inner) { }
}
