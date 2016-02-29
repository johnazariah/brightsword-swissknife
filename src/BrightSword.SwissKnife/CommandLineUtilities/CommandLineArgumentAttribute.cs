using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     Use this attribute to mark individual properties on an implementation of ICommandLineParams. This is used to bind
    ///     command-line parameters provided to individual, strongly-typed properties on the object. This attribute can mark up
    ///     either properties (suggested usage) or fields (legacy style)
    /// </summary>
    /// <example>
    ///     See the individual constructor signatures for specific examples of usage
    /// </example>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CommandLineArgumentAttribute : Attribute
    {
        private const BindingFlags C_BINDING_FLAGS =
            BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public;

        private bool _flag;

        internal CommandLineArgumentAttribute(string name, string description, bool flag)
        {
            Name = name;
            Description = description;
            IsFlag = flag;
        }

        /// <summary>
        ///     Specifies the property to be a mandatory command-line argument. When the argument is specified on the command line,
        ///     the caller must provide an appropriate value for the argument
        /// </summary>
        /// <param name="name"> The name of the argument string specified on the command line, without the '--' </param>
        /// <param name="description"> The description of the argument </param>
        /// <example>
        ///     <code>[CommandLineArgument("database", "The full path and name of the database")]
        ///     public string DatabasePath { get; set; }</code>
        ///     will require the application include a command-line argument of the form --database=somevalue If the command-line
        ///     argument is to be a flag, (i.e without a value specified), use this constructor and provide the named parameter
        ///     'IsFlag'
        ///     <code>[CommandLineArgument("clobber", "Flag argument specifying if the database should be clobbered", IsFlag = true)]
        ///                                                                                                                                                                                                                                                                                                  public bool Clobber { get; set; }</code>
        /// </example>
        public CommandLineArgumentAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Specifies the property to be a (possibly) optional command-line argument. When the argument is specified on the
        ///     command line, the caller may provide an value for the argument If no argument is specified, the default argument
        ///     specified here will be applied.
        /// </summary>
        /// <param name="name"> The name of the argument string specified on the command line, without the '--' </param>
        /// <param name="description"> The description of the argument </param>
        /// <param name="defaultValue">
        ///     The default value to be applied if none is specified. Ensure that this value is of the same
        ///     type of the property the attribute is defined on. If this value is set to 'null', the argument is treated as
        ///     mandatory.
        /// </param>
        /// <example>
        ///     <code>
        ///         [CommandLineArgument("exchange_rate", "The exchange rate to apply for the calculation", "1.0")]
        ///         public double ExchangeRate { get; set; }
        ///     </code>
        ///     will allow the application to include a command-line argument of the form --exchange_rate=1.1675.
        ///     However, if no --exchange_rate argument is provided, it is as if --exchange_rate=1.0 was specified.
        ///     <code>
        ///         [CommandLineArgument("clobber", "Flag argument specifying if the database should be clobbered", IsFlag = true)]
        ///         public bool Clobber { get; set; }
        ///     </code>
        /// </example>
        public CommandLineArgumentAttribute(string name, string description, object defaultValue)
            : this(name, description)
        {
            DefaultValue = defaultValue;

            // if a default value has been provided, then it's not optional
            IsOptional |= (defaultValue != null);
        }

        /// <summary>
        ///     Can be used as a named parameter to specify that the argument is optional
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        ///     Can be used as a named parameter to specify that the argument is a flag and requires no value. Flags are always
        ///     optional, so setting this to true will also set IsOptional to true.
        /// </summary>
        /// <example>
        ///     Use these named parameters with care, as it is possible to create meaningless parameters. Consider:
        ///     <code>[CommandLineArgument("create", "Should this database be created", true, IsFlag=true)]
        ///                                                                                                         public bool Create { get; set; }</code>
        ///     which results in *** [--create] : [Optional] Should this database be created *** --create has a default value of
        ///     [True] *** The effective value of --create is [True] This effectively means that --create will result in the same
        ///     behaviour whether it is specified or not!
        /// </example>
        public bool IsFlag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                if (_flag)
                {
                    // flags are optional always
                    IsOptional = true;
                }
            }
        }

        internal string Name { get; set; }
        internal string Description { get; set; }
        internal object DefaultValue { get; set; }

        internal string ArgumentDescriptor => string.Format(
            IsOptional
                ? "[{0}]"
                : "{0}",
            $"--{Name}{(IsFlag ? "" : "=<value>")}");

        internal string PropertyName { get; set; }
        internal Type ParamType { get; set; }

        internal object GetValue<TCommandLineParams>(TCommandLineParams _params)
        {
            var name = PropertyName;

            // ignore default parameters
            if (string.IsNullOrEmpty(name)) { return null; }

            // try and get a Property
            var pi = typeof (TCommandLineParams).GetProperty(name, C_BINDING_FLAGS);
            if (pi != null) { return pi.GetValue(_params, null); }

            // try and get a Field
            var fi = typeof (TCommandLineParams).GetField(name, C_BINDING_FLAGS);
            return fi?.GetValue(_params);
        }

        internal void SetValue<TCommandLineParams>(
            TCommandLineParams _params,
            object value,
            bool fSettingDefault = false)
        {
            var name = PropertyName;

            // ignore default parameters
            if (string.IsNullOrEmpty(name)) { return; }

            if (fSettingDefault) {
                value = value ?? DefaultValue;
            }
            else
            {
                if (IsFlag)
                {
                    // the parameter has been specified, but no value is provided because it's a flag
                    // so we should set the value to true
                    value = true;
                }
            }

            if (IsFlag && !fSettingDefault)
            {
                // the parameter has been specified, but no value is provided because it's a flag
                // so we should set the value to true
                value = true;
            }

            // try and get a Property Mutator
            var pi = typeof (TCommandLineParams).GetProperty(name, C_BINDING_FLAGS);
            if (pi != null)
            {
                var typedValue = value.CoerceType(pi.PropertyType, null);
                pi.SetValue(_params, typedValue, null);
                return;
            }

            // try and get a Field
            var fi = typeof (TCommandLineParams).GetField(name, C_BINDING_FLAGS);
            if (fi != null)
            {
                var typedValue = value.CoerceType(fi.FieldType, null);
                fi.SetValue(_params, typedValue);
            }
        }
    }
}