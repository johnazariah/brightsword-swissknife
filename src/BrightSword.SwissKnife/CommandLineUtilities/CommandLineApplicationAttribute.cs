using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     Use this attribute to mark up the struct or class which encapsulates the command line parameters of the
    ///     application.
    /// </summary>
    /// <example>
    ///     Consider a hypothetical app which requires a few command-line parameters, some optional, and some flags: We can
    ///     create a type to encapsulate the arguments as follows:
    ///     <code>[CommandLineApplication("A funky application to create the zero-th version of something")]
    ///                                                                                                                                                                            internal class Parameters
    ///                                                                                                                                                                            {
    ///                                                                                                                                                                            [CommandLineArgument("database", "The full path and name of the database", false, false)]
    ///                                                                                                                                                                            public string DatabasePath { get; set; }
    /// 
    ///                                                                                                                                                                            [CommandLineArgument("clobber", "Should this database be clobbered", true, false)]
    ///                                                                                                                                                                            public bool Clobber { get; set; }
    /// 
    ///                                                                                                                                                                            [CommandLineArgument("create", "Should this database be created", true, true, DefaultValue = true)]
    ///                                                                                                                                                                            public bool Create { get; set; }
    ///                                                                                                                                                                            }</code>
    ///     In this example, the short description provided is sufficient to provide friendly diagnostic information
    /// </example>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class CommandLineApplicationAttribute : Attribute
    {
        /// <summary>
        ///     Constructor for the attribute with a specified (optional) description.
        /// </summary>
        /// <param name="description">
        ///     The description of this application. Although optional, if no description is provided, a
        ///     snarky message is used instead, so provide one!
        /// </param>
        public CommandLineApplicationAttribute(string description = null)
        {
            Description = description;
        }

        /// <summary>
        ///     A description for the application. Displayed when --help or --verbose is specified.
        /// </summary>
        public string Description { get; set; }
    }
}