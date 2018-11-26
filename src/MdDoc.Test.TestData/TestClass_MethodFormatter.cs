﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MdDoc.Test.TestData
{
    class TestClass_MethodFormatter
    {
        public TestClass_MethodFormatter()
        {
        }

        public TestClass_MethodFormatter(string foo)
        {
        }

        public TestClass_MethodFormatter(string foo, IEnumerable<string> bar)
        {
        }

        public TestClass_MethodFormatter(string foo, IEnumerable<string> bar, IList<DirectoryInfo> baz)
        {
        }

        public void Method1() { }


        public string Method2() => throw new NotImplementedException();

        public string Method3(string foo) => throw new NotImplementedException();

        public string Method4(IDisposable foo) => throw new NotImplementedException();

        public T Method5<T>(string foo) => throw new NotImplementedException();

        public object Method6<T>(T foo) => throw new NotImplementedException();

        public object Method7<T1, T2>(T1 foo, T2 bar) => throw new NotImplementedException();

    }
}