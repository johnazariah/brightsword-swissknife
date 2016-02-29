namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     Marker interface for classes which represent command line parameters
    /// </summary>
    /// <example>
    ///     Consider a hypothetical app which requires a few command-line parameters, some optional, and some flags:
    ///     We can create a type to encapsulate the arguments, and implement the Main function as follows:
    ///     <code>
    /// 
    ///     [CommandLineApplication("A funky application to create the zero-th version of something")]
    ///     internal class Parameters : ICommandLineParams
    ///     {
    ///         [CommandLineArgument("database", "The full path and name of the database", false, false)]
    ///         public string DatabasePath { get; set; }
    /// 
    ///         [CommandLineArgument("clobber", "Should this database be clobbered", true, false)]
    ///         public bool Clobber { get; set; }
    /// 
    ///         [CommandLineArgument("create", "Should this database be created", true, true, DefaultValue = true)]
    ///         public bool Create { get; set; }
    ///     }
    ///  
    ///     internal class Program
    ///     {
    ///         private static void Main(string[] args)
    ///         {
    ///             (new Program()).Run&lt;Program, Parameter&gt;(args, _ => RunAction(_));
    ///         }
    ///             
    ///         private static void RunAction(Parameters parameters)
    ///         {
    ///             Console.WriteLine(parameters.DatabasePath);
    ///         }
    ///     }
    /// 
    /// </code>
    ///     Running this:
    ///     C:/> runprog --database="foo.sdb"
    ///     foo.sdb
    ///     Press any key to continue . . .
    /// </example>
    public interface ICommandLineParams
    {
    }
}