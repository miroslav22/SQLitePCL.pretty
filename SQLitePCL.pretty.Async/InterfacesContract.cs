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
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SQLitePCL.pretty
{
    [ContractClassFor(typeof(IAsyncDatabaseConnection))]
    internal abstract class IAsyncDatabaseConnectionContract : IAsyncDatabaseConnection
    {
        public abstract IObservable<DatabaseTraceEventArgs> Trace { get; }

        public abstract IObservable<DatabaseProfileEventArgs> Profile { get; }

        public abstract IObservable<DatabaseUpdateEventArgs> Update { get; }

        public IObservable<T> Use<T>(Func<IDatabaseConnection, IEnumerable<T>> f)
        {
            Contract.Requires(f != null);

            return default(IObservable<T>);
        }

        public abstract void Dispose();
    }

    [ContractClassFor(typeof(IAsyncStatement))]
    internal abstract class IAsyncStatementContract : IAsyncStatement
    {
        public IObservable<T> Use<T>(Func<IStatement, IEnumerable<T>> f)
        {
            Contract.Requires(f != null);

            return default(IObservable<T>);
        }

        public abstract void Dispose();
    }
}