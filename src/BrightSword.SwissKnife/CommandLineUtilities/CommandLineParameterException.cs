using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     Throw an instance of this, or a derived, exception to signal the unsuitability of a user-supplied command line
    ///     parameter value
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CommandLineParameterException : Exception
    {
        /// <summary>
        ///     Create an instance of this exception to throw
        /// </summary>
        /// <param name="parameterName"> The name of the parameter with the unsuitable value </param>
        /// <param name="message"> Helpful text which will be displayed in the error message </param>
        public CommandLineParameterException(string parameterName, string message)
            : base($"{parameterName} -- {message}")
        {
        }
    }
}