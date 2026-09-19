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
        public void TestMethod2_IsValidEmail()
        {
            Assert.IsTrue(_service.IsValidEmail("user.com"));
        }
    }
}
