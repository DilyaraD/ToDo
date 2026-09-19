using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDo.Services;

namespace UnitTestToDo
{
    [TestClass]
    public class UnitTest1
    {
        private LoginService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new LoginService();
        }

        [TestMethod]
        public void TestMethod1_IsValidEmail()
        {
            Assert.IsTrue(_service.IsValidEmail("user@mail.com"));
        }

        [TestMethod]
        public void TestMethod2_IsValidEmail_NoAtSymbol()
        {
            Assert.IsFalse(_service.IsValidEmail("user.com"));
        }

        [TestMethod]
        public void TestMethod3_IsValidEmail_NoDomain() 
        {
            Assert.IsFalse(_service.IsValidEmail("user@"));
        }

        [TestMethod]
        public void TestMethod4_IsValidEmail_Empty() 
        {
            Assert.IsFalse(_service.IsValidEmail(""));
        }

        [TestMethod]
        public void TestMethod5_IsValidEmail_Whitespace() 
        {
            Assert.IsFalse(_service.IsValidEmail("    "));
        }
    }
}
