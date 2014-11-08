/*
   Copyright 2014 David Bordoley

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace SQLitePCL.pretty.tests
{
    [TestFixture]
    public class SQLiteValueTests
    {
        private void compare(ISQLiteValue expected, ISQLiteValue test)
        {
            Assert.AreEqual(expected.Length, test.Length);
            Assert.AreEqual(expected.SQLiteType, test.SQLiteType);

            // FIXME: Testing double equality is imprecise
            //Assert.AreEqual(expected.ToDouble(), test.ToDouble());
            Assert.AreEqual(expected.ToInt64(), test.ToInt64());
            Assert.AreEqual(expected.ToInt(), test.ToInt());
            Assert.AreEqual(expected.ToString(), test.ToString());

            var expectedBlob = expected.ToBlob();
            var testBlog = test.ToBlob();

            if (expectedBlob == null)
            {
                Assert.IsNull(testBlog);
            }
            else 
            {
                Assert.IsNotNull(testBlog);
                CollectionAssert.AreEqual(expectedBlob, testBlog);
            }
        }

        [Test]
        public void TestNullValue()
        {
            using (var db = SQLite3.Open(":memory:"))
            {
                using (var stmt = db.PrepareStatement("SELECT null;"))
                {
                    var expected = stmt.Current.First();
                    compare(expected, SQLiteValue.Null);
                }
            }
        }

        [Test]
        public void TestIntValue()
        {
            long[] tests = 
                { 
                    2147483647, // Max int
                    -2147483648, // Min int
                    9223372036854775807, // Max Long
                    -9223372036854775808, // Min Long
                    -1234     
                };

            using (var db = SQLite3.Open(":memory:"))
            {
                foreach (var test in tests)
                {
                    db.Execute("CREATE TABLE foo (x int);");
                    db.Execute("INSERT INTO foo (x) VALUES (?)", test);

                    var rows = db.Query("SELECT x FROM foo;");
                    foreach (var row in rows)
                    {
                        compare(row.Single(), test.ToSQLiteValue());
                    }

                    db.Execute("DROP TABLE foo;");
                }
            }
        }

        [Test]
        public void TestBlobValue()
        {
            string[] tests = 
                { 
                    "  1234.56", 
                    " 1234.abasd", 
                    "abacdd\u10FFFF", 
                    "2147483647", // Max int
                    "-2147483648", // Min int
                    "9223372036854775807", // Max Long
                    "-9223372036854775808", // Min Long
                    "9923372036854775809", // Greater than max long
                    "-9923372036854775809", // Less than min long
                    "3147483648", // Long
                    "-1234",
                    // "1111111111111111111111" SQLite's result in this case is undefined
                };

            using (var db = SQLite3.Open(":memory:"))
            {
                foreach (var test in tests.Select(test => Encoding.UTF8.GetBytes(test)))
                {
                    db.Execute("CREATE TABLE foo (x blob);");
                    db.Execute("INSERT INTO foo (x) VALUES (?)", test);

                    var rows = db.Query("SELECT x FROM foo;");
                    foreach (var row in rows)
                    {
                        compare(row.Single(), test.ToSQLiteValue());
                    }
                    
                    db.Execute("DROP TABLE foo;");
                }
            }
        }

        [Test]
        public void TestStringValue()
        {
            string[] tests = 
                { 
                    "  1234.56", 
                    " 1234.abasd", 
                    "abacdd\u10FFFF", 
                    "2147483647", // Max int
                    "-2147483648", // Min int
                    "9223372036854775807", // Max Long
                    "-9223372036854775808", // Min Long
                    "9923372036854775809", // Greater than max long
                    "-9923372036854775809", // Less than min long
                    "3147483648", // Long
                    "-1234",
                    // "1111111111111111111111" SQLite's result in this case is undefined                 
                };

            using (var db = SQLite3.Open(":memory:"))
            {
                foreach (var test in tests)
                {
                    db.Execute("CREATE TABLE foo (x text);");
                    db.Execute("INSERT INTO foo (x) VALUES (?)", test);

                    var rows = db.Query("SELECT x FROM foo;");
                    foreach (var row in rows)
                    {
                        compare(row.Single(), test.ToSQLiteValue());
                    }
                    
                    db.Execute("DROP TABLE foo;");
                }
            }
        }
    }
}