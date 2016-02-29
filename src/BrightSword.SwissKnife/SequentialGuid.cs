using System;
using System.Diagnostics.CodeAnalysis;
using BrightSword.SwissKnife.Properties;

namespace BrightSword.SwissKnife
{
    /// <summary>
    ///     A sequential guid generator.
    ///     We want to generate an ID which is unique across servers and also monotonically increasing, so that performance
    ///     as a database primary key will be better than that of a truly unique Guid, without requiring the database to
    ///     generate the primary key.
    ///     The first 64 bytes of the Guid will be configured to be unique to an execution context consisting of a Realm,
    ///     Server and Application.
    ///     The next 8 bytes will be configured from a timestamp.
    ///     The default behaviour of the key generator ensures that the Realm, Server and Application are set-up by default the
    ///     very first time
    ///     a context is used. It then ensures that the same values are used even after server reboots.
    ///     If desired, any or all of the context values can be forced to specific values. These values would then be used even
    ///     after the server reboots.
    /// </summary>
    public static class SequentialGuid
    {
        /*
         * We are going to use
         *      public Guid(int _a, short _b, short _c, byte[] d)
         * to generate a unique guid that is also sequential
         *
         * 
         * int _a, short _b and short _c come from the application configuration
         * byte[] d will be constructed from a monotonically increasing sequence such as a timestamp
         * 
         */

        //private const string C_REALM_UNIQUEID = "Realm_UniqueID";
        //private const string C_SERVER_UNIQUEID = "Server_UniqueID";
        //private const string C_APPLICATION_UNIQUEID = "Application_UniqueID";

        private static int _a;
        private static short _b;
        private static short _c;

        static SequentialGuid()
        {
            InitializeFromSettings();
        }

        public static Guid NewSequentialGuid(int a = -1, short b = -1, short c = -1)
        {
            return new Guid(
                a == -1
                    ? _a
                    : a,
                b == -1
                    ? _b
                    : b,
                c == -1
                    ? _c
                    : c,
                UniqueSequenceGenerator.NextAscendingUniqueValue.GetReversedBytes());
        }

        public static Guid NewReverseSequentialGuid(int a = -1, short b = -1, short c = -1)
        {
            return new Guid(
                a == -1
                    ? _a
                    : a,
                b == -1
                    ? _b
                    : b,
                c == -1
                    ? _c
                    : c,
                UniqueSequenceGenerator.NextDescendingUniqueValue.GetReversedBytes());
        }

        private static void SaveSettings()
        {
            Settings.Default.Realm_UniqueID = (uint) _a;
            Settings.Default.Server_UniqueID = (ushort) _b;
            Settings.Default.Application_UniqueID = (ushort) _c;

            Settings.Default.Save();
        }

        [ExcludeFromCodeCoverage]
        private static void InitializeFromSettings()
        {
            var randomNumberGenerator = new Random();

            _a = (int) Settings.Default.Realm_UniqueID;
            while (_a == 0) { _a = randomNumberGenerator.Next(); }

            _b = (short) Settings.Default.Server_UniqueID;
            while (_b == 0) { _b = (short) randomNumberGenerator.Next(short.MaxValue); }

            _c = (short) Settings.Default.Application_UniqueID;
            while (_c == 0) { _c = (short) randomNumberGenerator.Next(short.MaxValue); }

            SaveSettings();
        }

        /// <summary>
        ///     WARNING!!!
        ///     WARNING!!! Use this method to force the realm, server and application id to known values.
        ///     WARNING!!! The normal method of simply allowing the static initializer to set up random values for the context is
        ///     preferred.
        ///     WARNING!!!
        ///     Generally speaking, this function should ONLY be used when keys generated in one { realm, server, application }
        ///     context
        ///     need to be re-used (perhaps because the server had to be reinstalled and the application needs to be able to resume
        ///     its old sequences)
        /// </summary>
        /// <param name="a">The Unique ID representing the Realm.</param>
        /// <param name="b">The Unique ID representing the Server.</param>
        /// <param name="c">The Unique ID representing the Application.</param>
        [ExcludeFromCodeCoverage]
        public static void Initialize(uint a, ushort b, ushort c)
        {
            _a = (int) a;
            _b = (short) b;
            _c = (short) c;

            SaveSettings();
        }
    }
}