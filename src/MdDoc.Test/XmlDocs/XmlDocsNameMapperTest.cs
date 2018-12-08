using MdDoc.Test.TestData;
using MdDoc.XmlDocs;
using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MdDoc.Test.XmlDocs
{
    public class XmlDocsNameMapperTest : TestBase
    {        
        [Theory]
        [InlineData("TestClass_Type", "T:MdDoc.Test.TestData.TestClass_Type")]
        [InlineData("TestClass_GenericType`1", "T:MdDoc.Test.TestData.TestClass_GenericType`1")]
        public void Type_names_are_mapped_as_expected(string typeName, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();
            var types = m_AssemblyDocumentation.MainModuleDocumentation.Definition.Types.ToArray();
            var typeDefinition = m_AssemblyDocumentation.MainModuleDocumentation.Definition.GetTypes().Single(t => t.Name == typeName);
            
            // ACT
            var actualName = mapper.GetXmlDocName(typeDefinition);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }

        [Theory]
        [InlineData(typeof(TestClass_Methods), "TestMethod1", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod1")]
        [InlineData(typeof(TestClass_Methods), "TestMethod2", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod2(System.String)")]
        [InlineData(typeof(TestClass_Methods), "TestMethod3", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod3(System.String)")]
        [InlineData(typeof(TestClass_Methods), "TestMethod4", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod4(System.String,System.String)")]
        [InlineData(typeof(TestClass_Methods), "TestMethod5", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod5``1(``0,System.String)")]
        [InlineData(typeof(TestClass_Methods), "TestMethod6", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod6(System.Collections.Generic.IEnumerable{System.String})")]
        [InlineData(typeof(TestClass_Methods), "TestMethod7", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod7``2(``0,``1)")]
        [InlineData(typeof(TestClass_Methods), "TestMethod8", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod8``2(``0,``1)")]
        [InlineData(typeof(TestClass_Methods), "TestMethod9", "M:MdDoc.Test.TestData.TestClass_Methods.TestMethod9``2(``1,``0)")]
        [InlineData(typeof(TestClass_GenericType<>), "TestMethod1", "M:MdDoc.Test.TestData.TestClass_GenericType`1.TestMethod1(`0)")]
        [InlineData(typeof(TestClass_GenericType<>), "TestMethod2", "M:MdDoc.Test.TestData.TestClass_GenericType`1.TestMethod2``1(``0)")]
        [InlineData(typeof(TestClass_GenericType<>), "TestMethod3", "M:MdDoc.Test.TestData.TestClass_GenericType`1.TestMethod3``1(``0,`0)")]
        public void Method_names_are_mapped_as_expected(Type type, string methodName, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();
            
            var method = GetTypeDefinition(type)
                .Methods
                .Single(m => m.Name == methodName);

            // ACT
            var actualName = mapper.GetXmlDocName(method);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }


        [Theory]
        [InlineData("op_UnaryPlus", "M:MdDoc.Test.TestData.TestClass_Operators.op_UnaryPlus(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_UnaryNegation", "M:MdDoc.Test.TestData.TestClass_Operators.op_UnaryNegation(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_LogicalNot", "M:MdDoc.Test.TestData.TestClass_Operators.op_LogicalNot(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_OnesComplement", "M:MdDoc.Test.TestData.TestClass_Operators.op_OnesComplement(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Increment", "M:MdDoc.Test.TestData.TestClass_Operators.op_Increment(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Decrement", "M:MdDoc.Test.TestData.TestClass_Operators.op_Decrement(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_True", "M:MdDoc.Test.TestData.TestClass_Operators.op_True(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_False", "M:MdDoc.Test.TestData.TestClass_Operators.op_False(MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Addition", "M:MdDoc.Test.TestData.TestClass_Operators.op_Addition(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Subtraction", "M:MdDoc.Test.TestData.TestClass_Operators.op_Subtraction(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Multiply", "M:MdDoc.Test.TestData.TestClass_Operators.op_Multiply(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Division", "M:MdDoc.Test.TestData.TestClass_Operators.op_Division(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Modulus", "M:MdDoc.Test.TestData.TestClass_Operators.op_Modulus(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_BitwiseAnd", "M:MdDoc.Test.TestData.TestClass_Operators.op_BitwiseAnd(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_BitwiseOr", "M:MdDoc.Test.TestData.TestClass_Operators.op_BitwiseOr(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_ExclusiveOr", "M:MdDoc.Test.TestData.TestClass_Operators.op_ExclusiveOr(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_LeftShift", "M:MdDoc.Test.TestData.TestClass_Operators.op_LeftShift(MdDoc.Test.TestData.TestClass_Operators,System.Int32)")]
        [InlineData("op_RightShift", "M:MdDoc.Test.TestData.TestClass_Operators.op_RightShift(MdDoc.Test.TestData.TestClass_Operators,System.Int32)")]
        [InlineData("op_Equality", "M:MdDoc.Test.TestData.TestClass_Operators.op_Equality(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Inequality", "M:MdDoc.Test.TestData.TestClass_Operators.op_Inequality(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_LessThan", "M:MdDoc.Test.TestData.TestClass_Operators.op_LessThan(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_GreaterThan", "M:MdDoc.Test.TestData.TestClass_Operators.op_GreaterThan(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_LessThanOrEqual", "M:MdDoc.Test.TestData.TestClass_Operators.op_LessThanOrEqual(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_GreaterThanOrEqual", "M:MdDoc.Test.TestData.TestClass_Operators.op_GreaterThanOrEqual(MdDoc.Test.TestData.TestClass_Operators,MdDoc.Test.TestData.TestClass_Operators)")]
        [InlineData("op_Implicit", "M:MdDoc.Test.TestData.TestClass_Operators.op_Implicit(MdDoc.Test.TestData.TestClass_Operators)~System.String")]
        [InlineData("op_Explicit", "M:MdDoc.Test.TestData.TestClass_Operators.op_Explicit(MdDoc.Test.TestData.TestClass_Operators)~System.Int32")]
        public void Operator_overloads_are_mapped_as_expected(string methodName, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();

            var method = GetTypeDefinition(typeof(TestClass_Operators))
                .Methods
                .Single(m => m.Name == methodName);

            // ACT
            var actualName = mapper.GetXmlDocName(method);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }

        [Theory]
        [InlineData(0, "M:MdDoc.Test.TestData.TestClass_Constructors.#ctor")]
        [InlineData(1, "M:MdDoc.Test.TestData.TestClass_Constructors.#ctor(System.String)")]
        public void Constructors_are_mapped_as_expected(int parameterCount, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();
            
            var method = GetTypeDefinition(typeof(TestClass_Constructors))
                .Methods
                .Single(m => m.IsConstructor && m.Parameters.Count == parameterCount);

            // ACT
            var actualName = mapper.GetXmlDocName(method);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }

        [Theory]
        [InlineData("Field1", "F:MdDoc.Test.TestData.TestClass_Fields.Field1")]
        public void Fields_are_mapped_as_expected(string fieldName, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();

            var field = GetTypeDefinition(typeof(TestClass_Fields))
                .Fields
                .Single(f => f.Name == fieldName);

            // ACT
            var actualName = mapper.GetXmlDocName(field);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }

        [Theory]
        [InlineData("Property1", "P:MdDoc.Test.TestData.TestClass_Properties.Property1")]
        [InlineData("Property2", "P:MdDoc.Test.TestData.TestClass_Properties.Property2")]
        public void Properties_are_mapped_as_expected(string propertyName, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();

            var property = GetTypeDefinition(typeof(TestClass_Properties))
                .Properties
                .Single(p => p.Name == propertyName);

            // ACT
            var actualName = mapper.GetXmlDocName(property);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }

        [Theory]
        [InlineData(1, "P:MdDoc.Test.TestData.TestClass_Properties.Item(System.Int32)")]
        [InlineData(2, "P:MdDoc.Test.TestData.TestClass_Properties.Item(System.Int32,System.Double)")]
        public void Indexers_are_mapped_as_expected(int parameterCount, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();

            var indexer = GetTypeDefinition(typeof(TestClass_Properties))
                .Properties
                .Single(p => p.Name == "Item" && p.Parameters.Count == parameterCount);

            // ACT
            var actualName = mapper.GetXmlDocName(indexer);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }


        [Theory]
        [InlineData("Event1", "E:MdDoc.Test.TestData.TestClass_Events.Event1")]
        [InlineData("Event2", "E:MdDoc.Test.TestData.TestClass_Events.Event2")]
        [InlineData("Event3", "E:MdDoc.Test.TestData.TestClass_Events.Event3")]
        public void Events_are_mapped_as_expected(string propertyName, string expectedName)
        {
            // ARRANGE
            var mapper = new XmlDocsNameMapper();

            var @event = GetTypeDefinition(typeof(TestClass_Events))
                .Events
                .Single(e => e.Name == propertyName);

            // ACT
            var actualName = mapper.GetXmlDocName(@event);

            // ASSERT
            Assert.Equal(expectedName, actualName);
        }
    }
}