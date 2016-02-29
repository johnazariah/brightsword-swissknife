using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BrightSword.SwissKnife;
using NUnit.Framework;

namespace Tests.BrightSword.SwissKnife
{
    [TestFixture]
    public class ExceptionMonadTests
    {
        private static IEnumerable<string> GetStringEnumerable_NoThrow()
        {
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
        }

        private static IEnumerable<string> GetStringEnumerable_Throws()
        {
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
            throw new NotImplementedException();
        }

        private static IEnumerable<Try<string>> GetMonadCollection()
        {
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
            yield return "Hello";
            yield return new NotImplementedException();
        }

        [Test]
        public void Test_IndexedBind_NoThrow()
        {
            Func<IEnumerable<string>> func = GetStringEnumerable_NoThrow;

            var result = func.MakeSafe()()
                .ToList();

            var itemsAreEqual = result.Select((_item, _index) => _item == "Hello");

            foreach (var item in itemsAreEqual) { item.Do(Assert.IsTrue); }

            Func<Try<string>, int, Try<int>> getLengthFunc = (_item, _index) => _item.Bind(_ => _.Length);

            var lengths = result.Select(getLengthFunc);

            foreach (var item in lengths) { item.Do(_ => Assert.AreEqual("Hello".Length, _)); }
        }

        [Test]
        public void Test_IndexedBind_Throws()
        {
            Func<IEnumerable<string>> func = GetStringEnumerable_Throws;

            var safeFunc = func.MakeSafe();

            var result = safeFunc()
                .ToList();

            var itemsAreEqual = result.Select((_item, _index) => _item == "Hello");

            foreach (var item in itemsAreEqual)
            {
                var _item = item;
                Assert.Throws<NotImplementedException>(() => _item.Do(Assert.IsTrue));
            }

            Func<Try<string>, int, Try<int>> getLengthFunc = (_item, _index) => _item.Bind(_ => _.Length);

            var lengths = result.Where(_ => !_.IsException)
                                .Select(getLengthFunc);

            foreach (var item in lengths) { item.Do(_ => Assert.AreEqual("Hello".Length, _)); }
        }

        [Test]
        public void Test_IndexedSelectorFunction_NoThrow()
        {
            Func<string, int, bool> indexedSelector = (_item, _index) => _item == "Hello";
            var safeIndexedSelector = indexedSelector.MakeSafe();

            var result = GetMonadCollection()
                .Select(safeIndexedSelector)
                .Single(_ => _.IsException);

            Assert.IsNotNull(result.GetException());
            Assert.IsInstanceOf<NotImplementedException>(result.GetException());
        }

        [Test]
        public void Test_LoadAssemblies()
        {
            Func<string, Assembly> loadAssembly = Assembly.LoadFile;
            var safeLoadAssembly = loadAssembly.MakeSafe();

            var path = Environment.CurrentDirectory;

            Console.WriteLine("Without Try - MethodChain");
            try
            {
                foreach (var value in new DirectoryInfo(path).GetFiles()
                                                             .Reverse()
                                                             .Select(_ => loadAssembly(_.FullName))
                                                             .SelectMany(_ => _.GetExportedTypes())
                                                             .SelectMany(_ => _.GetMethods())
                                                             .Select(_ => _.Name)) {
                                                                 Console.WriteLine(value);
                                                             }
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // the standard LINQ call may throw an Exception - which is expected!
            }

            Console.WriteLine("With Try - MethodChain");
            try
            {
                foreach (var value in new DirectoryInfo(path).GetFiles()
                                                             .Reverse()
                                                             .Select(_ => safeLoadAssembly(_.FullName))
                                                             .SelectMany(_ => _.GetExportedTypes())
                                                             .SelectMany(_ => _.GetMethods())
                                                             .Select(_ => _.Name))
                {
                    try {
                        value.Do(Console.WriteLine);
                    }
                    catch (Exception exception) {
                        Console.WriteLine("Saved an Exception {0}", exception.Message);
                    }
                }
            }
            catch (Exception) {
                Assert.Fail("Should not throw exception");
            }

            Console.WriteLine("Without Try - LINQ");
            try
            {
                foreach (var value in from file in new DirectoryInfo(path).GetFiles()
                                                                          .Reverse()
                                      let assembly = loadAssembly(file.FullName)
                                      from type in assembly.GetExportedTypes()
                                      from method in type.GetMethods()
                                      select method.Name) {
                                          Console.WriteLine(value);
                                      }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // the standard LINQ call may throw an Exception - which is expected!
            }

            Console.WriteLine("With Try - LINQ");
            try
            {
                foreach (var value in from file in new DirectoryInfo(path).GetFiles()
                                                                          .Reverse()
                                      let assembly = safeLoadAssembly(file.FullName)
                                      where !assembly.IsException
                                      from type in assembly.BindMany(_ => _.GetExportedTypes())
                                      from method in type.BindMany(_ => _.GetMethods())
                                      select method.Bind(_ => _.Name)) {
                                          value.Do(Console.WriteLine);
                                      }
            }
            catch (Exception) {
                Assert.Fail("Should not throw exception");
            }
        }

        [Test]
        public void Test_MakeImplicitSafeCallOnActionWithArgumentShouldPerformAction()
        {
            var _result = 27;

            Func<int> generatorFunction = () => 3;
            Action<int> throwingAction = _ => { _result = _; };

            generatorFunction.SafeCall(throwingAction);

            Assert.AreNotEqual(27, _result);
            Assert.AreEqual(3, _result);
        }

        [Test]
        public void Test_MakeSafeCall_FunctionReturningEnumerable_NoThrow()
        {
            var input = new[]
                        {
                            2,
                            3,
                            5,
                            7,
                            11
                        };
            Func<IEnumerable<int>> f = () => input;

            var result = f.MakeSafe()()
                .ToList();

            Assert.IsInstanceOf<IEnumerable<Try<int>>>(result);

            Assert.AreEqual(input.Length, result.Count);

            var itemsAreEqual = result.Select((_item, _index) => _item == input[_index]);

            foreach (var item in itemsAreEqual) { item.Do(Assert.IsTrue); }
        }

        [Test]
        public void Test_MakeSafeCall_FunctionReturningEnumerable_Throws()
        {
            Func<IEnumerable<int>> f = () => { throw new NotImplementedException(); };

            var result = f.MakeSafe()();
            Assert.IsInstanceOf<IEnumerable<Try<int>>>(result);
            foreach (var item in result)
            {
                var _item = item;
                Assert.Throws<NotImplementedException>(() => _item.Do(_ => { }));
            }
        }

        [Test]
        public void Test_MakeSafeCall_FunctionWithArgumentReturningEnumerable_NoThrow()
        {
            Func<int, IEnumerable<string>> f = _ => Enumerable.Repeat("hello", _);

            var result = f.MakeSafe()(4);
            Assert.IsInstanceOf<IEnumerable<Try<string>>>(result);

            foreach (var item in result) { item.Do(_ => Assert.AreEqual("hello", _)); }
        }

        [Test]
        public void Test_MakeSafeCall_FunctionWithArgumentReturningEnumerable_Throws()
        {
            Func<int, IEnumerable<string>> f = _ => { throw new NotImplementedException(); };

            var result = f.MakeSafe()(4);
            Assert.IsInstanceOf<IEnumerable<Try<string>>>(result);

            foreach (var item in result)
            {
                var _item = item;
                Assert.Throws<NotImplementedException>(() => _item.Do(_ => { }));
            }
        }

        [Test]
        public void Test_MakeSafeCall_FunctionWithThrowingArgumentReturningEnumerable()
        {
            Func<int, IEnumerable<string>> f = _ => Enumerable.Repeat("hello", _);
            Func<int> arg = () => { throw new ArgumentException(); };

            var result = f.MakeSafe()(arg.MakeSafe()());
            Assert.IsInstanceOf<IEnumerable<Try<string>>>(result);

            foreach (var item in result)
            {
                var _item = item;
                Assert.Throws<ArgumentException>(() => _item.Do(_ => { }));
            }
        }

        [Test]
        public void Test_MakeSafeCallOnActionShouldNotThrowWhenActionThrows()
        {
            Action throwingAction = () => { throw new NotImplementedException(); };
            throwingAction.MakeSafe()();

            Assert.IsTrue(true);
        }

        [Test]
        public void Test_MakeSafeCallOnActionShouldPerformAction()
        {
            var _result = 27;

            Action throwingAction = () => { _result = 3; };
            throwingAction.MakeSafe()();

            Assert.AreNotEqual(27, _result);
            Assert.AreEqual(3, _result);
        }

        [Test]
        public void Test_MakeSafeCallOnActionWithArgShouldNotThrowWhenActionThrows()
        {
            Action<int> throwingAction = _ => { throw new NotImplementedException(); };
            throwingAction.MakeSafe()(3);

            Assert.IsTrue(true);
        }

        [Test]
        public void Test_MakeSafeCallOnActionWithArgShouldPerformAction()
        {
            var _result = 27;

            Action<int> throwingAction = _ => { _result = _; };
            throwingAction.MakeSafe()(3);

            Assert.AreNotEqual(27, _result);
            Assert.AreEqual(3, _result);
        }

        [Test]
        public void Test_MakeSafeCallOnActionWithArgumentShouldNotThrowWhenActionThrows()
        {
            Func<int> generatorFunction = () => 3;
            Action<int> throwingAction = _ => { throw new NotImplementedException(); };

            generatorFunction.SafeCall(throwingAction);

            Assert.IsTrue(true);
        }

        [Test]
        public void Test_MakeSafeCallOnActionWithArgumentShouldNotThrowWhenArgumentIsAnException()
        {
            Func<int> generatorFunction = () => { throw new ArgumentException(); };
            Action<int> throwingAction = _ => { throw new NotImplementedException(); };
            generatorFunction.SafeCall(throwingAction);

            Assert.IsTrue(true);
        }

        [Test]
        public void Test_MakeSafeCallOnActionWithArgumentShouldPerformAction()
        {
            var _result = 27;

            Func<int> generatorFunction = () => 3;
            Action<int> throwingAction = _ => { _result = _; };

            generatorFunction.MakeSafe()()
                .Bind(throwingAction);

            Assert.AreNotEqual(27, _result);
            Assert.AreEqual(3, _result);
        }

        [Test]
        public void Test_MakeSafeCallOnFunctionShouldNotThrowWhenFunctionDoesNotThrow()
        {
            Func<int> nonThrowingFunction = () => 42;
            var safeFunction = nonThrowingFunction.MakeSafe();

            var result = safeFunction();

            Assert.IsNotNull(result);

            result.Do(_ => Assert.AreEqual(42, _));
            Assert.AreEqual(42, result.GetValueOrDefault());
        }

        [Test]
        public void Test_MakeSafeCallOnFunctionShouldNotThrowWhenFunctionThrows()
        {
            Func<int> throwingFunction = () => { throw new NotImplementedException(); };
            var result = throwingFunction.MakeSafe()();

            Assert.IsNotNull(result);
            Assert.Throws<NotImplementedException>(() => result.Do(_ => { }));
            Assert.AreEqual(default(int), result.GetValueOrDefault());
            Assert.AreEqual(-1, result.GetValueOrDefault(-1));
        }

        [Test]
        public void Test_MakeSafeCallOnFunctionShouldNotThrowWhenFunctionWithArgDoesNotThrow()
        {
            Func<int, double> nonThrowingFunction = _ => _*Math.PI;

            var result = nonThrowingFunction.MakeSafe()(2);

            Assert.IsNotNull(result);

            result.Do(_ => Assert.AreEqual(2*Math.PI, _));
        }

        [Test]
        public void Test_MakeSafeCallOnFunctionShouldNotThrowWhenFunctionWithArgThrows()
        {
            Func<int, string> throwingFunction = _ => { throw new NullReferenceException(); };

            var result = throwingFunction.MakeSafe()(2);

            Assert.IsNotNull(result);

            Assert.Throws<NullReferenceException>(() => result.Do(_ => { }));
        }

        [Test]
        public void Test_Safe_ActionWithArgument_HandlesExceptionArgument()
        {
            Action<int> throwingAction = _ => { throw new NotImplementedException(); };
            var safeThrowingAction = throwingAction.MakeSafe();

            safeThrowingAction(new ArgumentException());

            Assert.IsTrue(true);
        }

        [Test]
        public void Test_Safe_ActionWithArgument_HandlesNonExceptionArgument()
        {
            var _result = 27;
            Action<int> throwingAction = _ => { _result = _; };
            var safeThrowingAction = throwingAction.MakeSafe();

            safeThrowingAction(3);

            Assert.AreEqual(3, _result);
        }

        [Test]
        public void Test_SafeCallChaining_Implicit_NoExceptions()
        {
            Func<int> getNumberOfRevolutions = () => 2;
            Func<int, double> getNumberOfRadians = _ => _*2*Math.PI;
            Func<double, double> toDegrees = _ => _*180/Math.PI;

            var safeResult = getNumberOfRevolutions.SafeCall(getNumberOfRadians);
            safeResult.Do(_ => Assert.AreEqual(4*Math.PI, _));

            var degrees = safeResult.Bind(toDegrees);
            degrees.Do(
                _ =>
                {
                    Console.WriteLine(_);
                    Assert.AreEqual(720, _);
                });

            Assert.AreEqual(720, degrees.GetValueOrDefault());
        }

        [Test]
        public void Test_SafeCallChaining_InnerException()
        {
            Func<int> getNumberOfRevolutions = () => { throw new ArgumentOutOfRangeException(); };
            Func<int, double> getNumberOfRadians = _ => _*2*Math.PI;
            Func<double, double> toDegrees = _ => _*180/Math.PI;

            Func<double> composedFunction = () => getNumberOfRadians(getNumberOfRevolutions());
            Assert.Throws<ArgumentOutOfRangeException>(() => composedFunction());

            var safeResult = getNumberOfRevolutions.SafeCall(getNumberOfRadians);

            Assert.Throws<ArgumentOutOfRangeException>(() => safeResult.Do(_ => { }));
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(safeResult.GetException());

            var degrees = safeResult.Bind(toDegrees);
            Assert.Throws<ArgumentOutOfRangeException>(() => degrees.Do(_ => { }));
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(degrees.GetException());
        }

        [Test]
        public void Test_SafeCallChaining_NoExceptions()
        {
            Func<int> getNumberOfRevolutions = () => 2;
            Func<int, double> getNumberOfRadians = _ => _*2*Math.PI;
            Func<double, double> toDegrees = _ => _*180/Math.PI;

            var safeResult = getNumberOfRevolutions.MakeSafe()()
                .Bind(getNumberOfRadians);

            safeResult.Do(_ => Assert.AreEqual(4*Math.PI, _));

            var degrees = safeResult.Bind(toDegrees);
            degrees.Do(
                _ =>
                {
                    Console.WriteLine(_);
                    Assert.AreEqual(720, _);
                });

            Assert.AreEqual(720, degrees.GetValueOrDefault());
        }

        [Test]
        public void Test_SafeCallChaining_OuterExceptions()
        {
            Func<int> getNumberOfRevolutions = () => 2;
            Func<int, double> getNumberOfRadians = _ => { throw new ArithmeticException(); };
            Func<double, Try<double>> toDegrees = _ => _*180/Math.PI;

            Func<double> composedFunction = () => getNumberOfRadians(getNumberOfRevolutions());
            Assert.Throws<ArithmeticException>(() => composedFunction());

            var safeGetNumberOfRevolutions = getNumberOfRevolutions.MakeSafe();
            var safeGetNumberOfRadians = getNumberOfRadians.MakeSafe();

            Func<Try<double>> safeComposedFunction = () => safeGetNumberOfRadians(safeGetNumberOfRevolutions());

            var safeResult = safeComposedFunction();
            Assert.Throws<ArithmeticException>(() => safeResult.Do(_ => { }));
            Assert.IsInstanceOf<ArithmeticException>(safeResult.GetException());

            var degrees = safeResult.Bind(toDegrees);
            Assert.Throws<ArithmeticException>(() => degrees.Do(_ => { }));
            Assert.IsInstanceOf<ArithmeticException>(degrees.GetException());
        }
    }
}