using System;

namespace TagInterpreterR
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string message) : base(message)
        {
        }

        public static InterpreterException InvalidCharacter(char c, int i)
        {
            return new InterpreterException("Invalid character " + c + " at " + i);
        }
    }
}
