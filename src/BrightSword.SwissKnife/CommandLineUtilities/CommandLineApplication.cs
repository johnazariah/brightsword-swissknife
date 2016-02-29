using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     A static container for command-line processing extension methods.
    /// </summary>
    [Obsolete("Review and rewrite using dynamic and generics!")]
    [ExcludeFromCodeCoverage]
    public static class CommandLineApplication
    {
        /// <summary>
        ///     A generic extension method to automatically process command line parameters and provide a strongly-typed form to a
        ///     specified processing function
        /// </summary>
        /// <typeparam name="TCallSite">
        ///     The type of the program which needs to process command-line arguments. Typically this class has a function of the
        ///     form
        ///     <code>
        ///         internal static int Main(string[] args)
        ///         {
        ///         ...
        ///         }
        ///     </code>
        /// </typeparam>
        /// <typeparam name="TCommandLineParams"> The strongly typed container which specifies the expected command-line parameters </typeparam>
        /// <param name="_this"> The running instance of the program class. Used as the 'this' parameter to this method </param>
        /// <param name="args"> The array of command-line arguments as passed in from the environment </param>
        /// <param name="action">
        ///     The actual function which does the work to be done inside Main, but with reference to strongly-typed parameters
        ///     instead of <paramref name="args" />
        /// </param>
        /// <returns> The result value of the actual function provided. </returns>
        /// <example>
        ///     Consider a hypothetical app which requires a few command-line parameters, some optional, and some flags: We can
        ///     create a type to encapsulate the arguments, and implement the Main function as follows:
        ///     <code>
        ///       [CommandLineApplication("A funky application to create the zero-th version of something")]
        ///       internal class Parameters
        ///       {
        ///           [CommandLineArgument("database", "The full path and name of the database")]
        ///           public string DatabasePath { get; set; }
        ///     
        ///           [CommandLineArgument("clobber", "Should this database be clobbered", IsFlag = true)]
        ///           public bool Clobber { get; set; }
        ///     
        ///           [CommandLineArgument("create", "Should this database be created", true)]
        ///           public bool Create { get; set; }
        ///       }
        ///     
        ///       internal class Program
        ///       {
        ///           internal static int Main(string[] args)
        ///           {
        ///               return (new Program()).Run&lt;Program,Parameters&gt;(args, _ => RunAction(_)) ;
        ///           }
        ///     
        ///           internal static int RunAction(Parameters parameters)
        ///           {
        ///               Console.WriteLine(parameters.DatabasePath);
        ///               return 0;
        ///           }
        ///       }
        ///  </code>
        ///     Running this:
        ///     C:/&gt; runprog --database="foo.sdb" foo.sdb
        ///     Press any key to continue . . .
        ///     Further, we get two arguments for free:
        ///     * --help will print the usage parameters and quit
        ///     * --verbose will print the effective parameters used for execution, and perform the execution
        ///     C:/&gt; runprog --verbose --database="foo.sdb" --clobber
        ///     ***********************************************************************************************
        ///     ***
        ///     *** runprog.exe - A funky application to create the zero-th version of something
        ///     ***
        ///     *** Run Started: 15/07/2011 8:05:20 PM
        ///     ***
        ///     *** The effective value of --help is [null]
        ///     *** The effective value of --verbose is [null]
        ///     *** The effective value of --database is [foo.sdb]
        ///     *** The effective value of --clobber is [True]
        ///     *** The effective value of --create is [True]
        ///     ***
        ///     ***********************************************************************************************
        ///     Press any key to continue . . .
        /// </example>
        public static int Run<TCallSite, TCommandLineParams>(
            this TCallSite _this,
            string[] args,
            Func<TCommandLineParams, int> action = null) where TCallSite : class
        {
            try {
                return Run(args, action);
            }
            catch (CommandLineParameterException commandLineParameterException)
            {
                CommandLineApplicationClosure<TCommandLineParams>.Error(
                    commandLineParameterException,
                    "Validation failed",
                    -1);

                // should never reach this, because Usage() calls Environment.Exit()
                throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
            }
        }

        /// <summary>
        ///     A generic extension method to automatically process command line parameters and provide a strongly-typed form to a
        ///     specified processing function
        /// </summary>
        /// <typeparam name="TCallSite">
        ///     The type of the program which needs to process command-line arguments. Typically this class has a function of the
        ///     form
        ///     <code>
        ///         internal static int Main(string[] args)
        ///         {
        ///         ...
        ///         }
        ///     </code>
        /// </typeparam>
        /// <typeparam name="TCommandLineParams"> The strongly typed container which specifies the expected command-line parameters </typeparam>
        /// <param name="_this"> The running instance of the program class. Used as the 'this' parameter to this method </param>
        /// <param name="args"> The array of command-line arguments as passed in from the environment </param>
        /// <param name="action">
        ///     The actual function which does the work to be done inside Main, but with reference to strongly-typed parameters
        ///     instead of <paramref name="args" />
        /// </param>
        /// <example>
        ///     Consider a hypothetical app which requires a few command-line parameters, some optional, and some flags: We can
        ///     create a type to encapsulate the arguments, and implement the Main function as follows:
        ///     <code>
        ///       [CommandLineApplication("A funky application to create the zero-th version of something")]
        ///       internal class Parameters
        ///       {
        ///           [CommandLineArgument("database", "The full path and name of the database")]
        ///           public string DatabasePath { get; set; }
        ///     
        ///           [CommandLineArgument("clobber", "Should this database be clobbered", IsFlag = true)]
        ///           public bool Clobber { get; set; }
        ///     
        ///           [CommandLineArgument("create", "Should this database be created", true)]
        ///           public bool Create { get; set; }
        ///       }
        ///     
        ///       internal class Program
        ///       {
        ///           internal static void Main(string[] args)
        ///           {
        ///               (new Program()).Run&lt;Program,Parameters&gt;(args, _ => RunAction(_)) ;
        ///           }
        ///     
        ///           internal static void RunAction(Parameters parameters)
        ///           {
        ///               Console.WriteLine(parameters.DatabasePath);
        ///               return 0;
        ///           }
        ///       }
        ///  </code>
        ///     Running this:
        ///     C:/&gt; runprog --database="foo.sdb" foo.sdb
        ///     Press any key to continue . . .
        ///     Further, we get two arguments for free:
        ///     * --help will print the usage parameters and quit
        ///     * --verbose will print the effective parameters used for execution, and perform the execution
        ///     C:/&gt; runprog --verbose --database="foo.sdb" --clobber
        ///     ***********************************************************************************************
        ///     ***
        ///     *** runprog.exe - A funky application to create the zero-th version of something
        ///     ***
        ///     *** Run Started: 15/07/2011 8:05:20 PM
        ///     ***
        ///     *** The effective value of --help is [null]
        ///     *** The effective value of --verbose is [null]
        ///     *** The effective value of --database is [foo.sdb]
        ///     *** The effective value of --clobber is [True]
        ///     *** The effective value of --create is [True]
        ///     ***
        ///     ***********************************************************************************************
        ///     Press any key to continue . . .
        /// </example>
        public static void Run<TCallSite, TCommandLineParams>(
            this TCallSite _this,
            string[] args,
            Action<TCommandLineParams> action = null) where TCallSite : class
        {
            try {
                Run(args, action);
            }
            catch (CommandLineParameterException commandLineParameterException)
            {
                CommandLineApplicationClosure<TCommandLineParams>.Error(
                    commandLineParameterException,
                    "Validation failed",
                    -1);
            }
        }

        /// <summary>
        ///     A generic method to automatically process command line parameters and provide a strongly-typed form to a specified
        ///     processing function
        /// </summary>
        /// <typeparam name="TCommandLineParams"> The strongly typed container which specifies the expected command-line parameters </typeparam>
        /// <param name="args"> The array of command-line arguments as passed in from the environment </param>
        /// <param name="action">
        ///     The actual function which does the work to be done inside Main, but with reference to strongly-typed parameters
        ///     instead of
        ///     <paramref
        ///         name="args" />
        /// </param>
        /// <returns> The result value of the actual function provided. </returns>
        /// <example>
        ///     Consider a hypothetical app which requires a few command-line parameters, some optional, and some flags: We can
        ///     create a type to encapsulate the arguments, and implement the Main function as follows:
        ///     <code>
        ///       [CommandLineApplication("A funky application to create the zero-th version of something")]
        ///       internal class Parameters
        ///       {
        ///           [CommandLineArgument("database", "The full path and name of the database")]
        ///           public string DatabasePath { get; set; }
        ///     
        ///           [CommandLineArgument("clobber", "Should this database be clobbered", IsFlag = true)]
        ///           public bool Clobber { get; set; }
        ///     
        ///           [CommandLineArgument("create", "Should this database be created", true)]
        ///           public bool Create { get; set; }
        ///       }
        ///     
        ///       internal class Program
        ///       {
        ///           internal static void Main(string[] args)
        ///           {
        ///               (new Program()).Run&lt;Program,Parameters&gt;(args, _ => RunAction(_)) ;
        ///           }
        ///     
        ///           internal static void RunAction(Parameters parameters)
        ///           {
        ///               Console.WriteLine(parameters.DatabasePath);
        ///               return 0;
        ///           }
        ///       }
        ///  </code>
        ///     Running this:
        ///     C:/&gt; runprog --database="foo.sdb" foo.sdb
        ///     Press any key to continue . . .
        ///     Further, we get two arguments for free:
        ///     * --help will print the usage parameters and quit
        ///     * --verbose will print the effective parameters used for execution, and perform the execution
        ///     C:/&gt; runprog --verbose --database="foo.sdb" --clobber
        ///     ***********************************************************************************************
        ///     ***
        ///     *** runprog.exe - A funky application to create the zero-th version of something
        ///     ***
        ///     *** Run Started: 15/07/2011 8:05:20 PM
        ///     ***
        ///     *** The effective value of --help is [null]
        ///     *** The effective value of --verbose is [null]
        ///     *** The effective value of --database is [foo.sdb]
        ///     *** The effective value of --clobber is [True]
        ///     *** The effective value of --create is [True]
        ///     ***
        ///     ***********************************************************************************************
        ///     Press any key to continue . . .
        /// </example>
        public static int Run<TCommandLineParams>(this string[] args, Func<TCommandLineParams, int> action = null)
        {
            try
            {
                action = action ?? (_ => 0);

                var _params = CommandLineApplicationClosure<TCommandLineParams>.ProcessArguments(args);

                return action(_params);
            }
            catch (CommandLineParameterException commandLineParameterException)
            {
                CommandLineApplicationClosure<TCommandLineParams>.Error(
                    commandLineParameterException,
                    "Validation failed",
                    -1);

                // should never reach this, because Usage() calls Environment.Exit()
                throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
            }
        }

        /// <summary>
        ///     A generic extension method to automatically process command line parameters and provide a strongly-typed form to a
        ///     specified processing method
        /// </summary>
        /// <typeparam name="TCommandLineParams"> The strongly typed container which specifies the expected command-line parameters </typeparam>
        /// <param name="args"> The array of command-line arguments as passed in from the environment </param>
        /// <param name="action">
        ///     The actual function which does the work to be done inside Main, but with reference to strongly-typed parameters
        ///     instead of
        ///     <paramref
        ///         name="args" />
        /// </param>
        /// <example>
        ///     Consider a hypothetical app which requires a few command-line parameters, some optional, and some flags: We can
        ///     create a type to encapsulate the arguments, and implement the Main function as follows:
        ///     <code>
        ///       [CommandLineApplication("A funky application to create the zero-th version of something")]
        ///       internal class Parameters
        ///       {
        ///           [CommandLineArgument("database", "The full path and name of the database")]
        ///           public string DatabasePath { get; set; }
        ///     
        ///           [CommandLineArgument("clobber", "Should this database be clobbered", IsFlag = true)]
        ///           public bool Clobber { get; set; }
        ///     
        ///           [CommandLineArgument("create", "Should this database be created", true)]
        ///           public bool Create { get; set; }
        ///       }
        ///     
        ///       internal class Program
        ///       {
        ///           internal static void Main(string[] args)
        ///           {
        ///               (new Program()).Run&lt;Program,Parameters&gt;(args, _ => RunAction(_)) ;
        ///           }
        ///     
        ///           internal static void RunAction(Parameters parameters)
        ///           {
        ///               Console.WriteLine(parameters.DatabasePath);
        ///               return 0;
        ///           }
        ///       }
        ///  </code>
        ///     Running this:
        ///     C:/&gt; runprog --database="foo.sdb" foo.sdb
        ///     Press any key to continue . . .
        ///     Further, we get two arguments for free:
        ///     * --help will print the usage parameters and quit
        ///     * --verbose will print the effective parameters used for execution, and perform the execution
        ///     C:/&gt; runprog --verbose --database="foo.sdb" --clobber
        ///     ***********************************************************************************************
        ///     ***
        ///     *** runprog.exe - A funky application to create the zero-th version of something
        ///     ***
        ///     *** Run Started: 15/07/2011 8:05:20 PM
        ///     ***
        ///     *** The effective value of --help is [null]
        ///     *** The effective value of --verbose is [null]
        ///     *** The effective value of --database is [foo.sdb]
        ///     *** The effective value of --clobber is [True]
        ///     *** The effective value of --create is [True]
        ///     ***
        ///     ***********************************************************************************************
        ///     Press any key to continue . . .
        /// </example>
        public static void Run<TCommandLineParams>(this string[] args, Action<TCommandLineParams> action = null)
        {
            try
            {
                action = action ?? (_ => { });

                var _params = CommandLineApplicationClosure<TCommandLineParams>.ProcessArguments(args);

                action(_params);
            }
            catch (CommandLineParameterException commandLineParameterException)
            {
                CommandLineApplicationClosure<TCommandLineParams>.Error(
                    commandLineParameterException,
                    "Validation failed",
                    -1);

                // should never reach this, because Usage() calls Environment.Exit()
                throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
            }
        }

        #region Nested type: CommandLineApplicationClosure

        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        internal static class CommandLineApplicationClosure<TCommandLineParams>
        {
            private const BindingFlags C_BINDING_FLAGS =
                BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public;

            static CommandLineApplicationClosure()
            {
                Parameters = (TCommandLineParams) Activator.CreateInstance(typeof (TCommandLineParams));

                ApplicationAttribute = typeof (TCommandLineParams).GetCustomAttribute<CommandLineApplicationAttribute>();

                DefaultParameterTemplates = new Dictionary<string, CommandLineArgumentAttribute>
                                            {
                                                {
                                                    "help",
                                                    new CommandLineArgumentAttribute(
                                                    "help",
                                                    "Prints this message and quits",
                                                    true)
                                                },
                                                {
                                                    "verbose",
                                                    new CommandLineArgumentAttribute(
                                                    "verbose",
                                                    "Emits more information",
                                                    true)
                                                }
                                            };

                ParameterTemplates = BuildParameterTemplates();

                ParameterNames = BuildParameterNames();

                Name = (new FileInfo(Environment.GetCommandLineArgs()[0])).Name;

                Description = (string.IsNullOrEmpty(ApplicationAttribute?.Description)
                                   ? "A cool but nondescript application"
                                   : ApplicationAttribute.Description);

                ApplyDefaultValues();
            }

            internal static TCommandLineParams Parameters { get; set; }
            internal static CommandLineApplicationAttribute ApplicationAttribute { get; set; }
            internal static IDictionary<string, CommandLineArgumentAttribute> DefaultParameterTemplates { get; set; }
            internal static IDictionary<string, CommandLineArgumentAttribute> ParameterTemplates { get; set; }
            internal static string ParameterNames { get; set; }
            internal static string Name { get; set; }
            internal static bool Verbose { get; set; }
            internal static string Description { get; set; }

            public static TCommandLineParams ProcessArguments(IEnumerable<string> args)
            {
                var arguments = BuildArgumentPairs(args);

                if (arguments.ContainsKey("help"))
                {
                    Usage();

                    // should never reach this, because Usage() calls Environment.Exit()
                    throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
                }

                Verbose = arguments.ContainsKey("verbose");

                try {
                    SetParameterValues(arguments, Parameters);
                }
                catch (Exception ex)
                {
                    Error(ex, null, -1);

                    // should never reach this, because Usage() calls Environment.Exit()
                    throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
                }

                if (!ValidateSuccessful())
                {
                    Error(null, "Validation failed", -1);

                    // should never reach this, because Usage() calls Environment.Exit()
                    throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
                }

                if (Verbose) { VerboseReport(); }

                return Parameters;
            }

            internal static IDictionary<string, CommandLineArgumentAttribute> BuildParameterTemplates()
            {
                var templates = new Dictionary<string, CommandLineArgumentAttribute>(DefaultParameterTemplates);

                var propertyInfos = typeof (TCommandLineParams).GetProperties(C_BINDING_FLAGS);
                var propertyAttributes =
                    propertyInfos.Select(
                        _mi =>
                        _mi.GetCustomAttributeValue<CommandLineArgumentAttribute, CommandLineArgumentAttribute>(
                            _ =>
                            {
                                // inject the PropertyName and ParamType properties
                                _.PropertyName = _mi.Name;
                                _.ParamType = _mi.PropertyType;
                                return _;
                            }));

                foreach (var _claa in propertyAttributes.Where(_ => _ != null)) { templates.Add(_claa.Name, _claa); }

                var fieldInfos = typeof (TCommandLineParams).GetFields(C_BINDING_FLAGS);
                var fieldAttributes =
                    fieldInfos.Select(
                        _mi =>
                        _mi.GetCustomAttributeValue<CommandLineArgumentAttribute, CommandLineArgumentAttribute>(
                            _ =>
                            {
                                // inject the PropertyName and ParamType properties
                                _.PropertyName = _mi.Name;
                                _.ParamType = _mi.FieldType;
                                return _;
                            }));

                foreach (var _claa in fieldAttributes.Where(_ => _ != null)) { templates.Add(_claa.Name, _claa); }

                return templates;
            }

            internal static string BuildParameterNames()
            {
                var argumentNames = ParameterTemplates.Values.Select(_ => _.ArgumentDescriptor);
                return argumentNames.Aggregate(string.Empty, (_r, _c) => $"{_r} {_c}");
            }

            internal static void ApplyDefaultValues()
            {
                foreach (var claa in ParameterTemplates.Values) { claa.SetValue(Parameters, claa.DefaultValue, true); }
            }

            internal static IDictionary<string, string> BuildArgumentPairs(IEnumerable<string> args)
            {
                var arguments = new Dictionary<string, string>();

                foreach (var _arg in args)
                {
                    var _parts = _arg.Split('=');
                    var _name = _parts[0].Replace("--", "")
                                         .ToLower();
                    var _value = _parts.Length > 1
                                     ? _parts[1]
                                     : null;

                    arguments[_name] = _value;
                }

                return arguments;
            }

            internal static void SetParameterValues(
                IEnumerable<KeyValuePair<string, string>> arguments,
                TCommandLineParams commandLineParams)
            {
                foreach (var _argPair in arguments.Where(_ => ParameterTemplates.ContainsKey(_.Key))) {
                    ParameterTemplates[_argPair.Key].SetValue(commandLineParams, _argPair.Value);
                }
            }

            internal static bool ValidateSuccessful()
            {
                return !(from claa in ParameterTemplates.Values
                         let value = claa.GetValue(Parameters)
                         let found = (!string.IsNullOrEmpty(value as string))
                         where !claa.IsOptional && !claa.IsFlag && !found
                         select claa).Any();
            }

            internal static void VerboseReport()
            {
                Console.WriteLine(
                    "***********************************************************************************************");
                Console.WriteLine("***");
                Console.WriteLine("*** {0} - {1}", Name, Description);
                Console.WriteLine("***");
                Console.WriteLine("*** Run Started: {0}", DateTime.Now);
                Console.WriteLine("***");
                foreach (var claa in ParameterTemplates.Values)
                {
                    Console.WriteLine(
                        "***\t\t The effective value of --{0} is [{1}]",
                        claa.Name,
                        claa.GetValue(Parameters) ?? "null");
                }
                Console.WriteLine("***");
                Console.WriteLine(
                    "***********************************************************************************************");
            }

            internal static void Error(Exception exception = null, string error = null, int errorCode = 0)
            {
                var exceptionMessage = exception == null
                                           ? null
                                           : " : " + exception.Message;

                var message = exceptionMessage ?? error ?? "Unknown error";

                Console.WriteLine(
                    "***********************************************************************************************");
                Console.WriteLine("***");
                Console.WriteLine("*** {0} - {1}", Name, Description);
                Console.WriteLine("***");
                Console.WriteLine("*** ERROR");
                Console.WriteLine("*** ERROR {0}", message);
                Console.WriteLine("*** ERROR");
                Console.WriteLine("***");
                Console.WriteLine("***");
                Console.WriteLine("*** ABORTING");
                Console.WriteLine("***");
                PrintUsageMessage();
                Console.WriteLine("***");
                Console.WriteLine(
                    "***********************************************************************************************");

                Environment.Exit(errorCode);
            }

            internal static void PrintUsageMessage()
            {
                Console.WriteLine("*** Usage: {0} {1}", Name, ParameterNames);
                Console.WriteLine("***");
                foreach (var claa in ParameterTemplates.Values)
                {
                    var optional = claa.IsOptional
                                       ? "[Optional] "
                                       : "";

                    Console.WriteLine("***\t {0,-18} : {1}{2}", claa.ArgumentDescriptor, optional, claa.Description);

                    if ((claa.ParamType != null)
                        && (claa.ParamType.IsEnum))
                    {
                        Console.WriteLine(
                            "***\t\t --{0} should be one of [{1}]",
                            claa.Name,
                            string.Join(", ", Enum.GetNames(claa.ParamType)));
                    }

                    if (claa.DefaultValue != null) {
                        Console.WriteLine("***\t\t --{0} has a default value of [{1}]", claa.Name, claa.DefaultValue);
                    }

                    Console.WriteLine(
                        "***\t\t The effective value of --{0} is [{1}]",
                        claa.Name,
                        claa.GetValue(Parameters) ?? "null");

                    Console.WriteLine("***");
                }
                Console.WriteLine("***");
                Console.WriteLine("*** Ensure that there are no spaces except between arguments.");
                Console.WriteLine("*** Quote values that embed a space.");
                Console.WriteLine("*** Use '--arg=value' and not '--arg = value'.");
                Console.WriteLine("***");
                Console.WriteLine("*** Exiting...");
            }

            internal static void Usage(int errorCode = 0)
            {
                Console.WriteLine(
                    "***********************************************************************************************");
                Console.WriteLine("***");
                Console.WriteLine("*** {0} - {1}", Name, Description);
                Console.WriteLine("***");
                Console.WriteLine("*** Usage: {0} {1}", Name, ParameterNames);
                Console.WriteLine("***");
                PrintUsageMessage();
                Console.WriteLine("***");
                Console.WriteLine(
                    "***********************************************************************************************");

                Environment.Exit(errorCode);
            }
        }

        #endregion
    }
}