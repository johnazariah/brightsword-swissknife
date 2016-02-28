namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("BrightSword.SwissKnife")>]
[<assembly: AssemblyProductAttribute("BrightSword.SwissKnife")>]
[<assembly: AssemblyDescriptionAttribute("A collection of general programming utilities")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
