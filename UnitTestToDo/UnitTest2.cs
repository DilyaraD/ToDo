using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToDo.Models;
using ToDo.Services;

namespace UnitTestToDo
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1_IsCompletedFalse()
        {
            var task = new UserTask();
            Assert.IsFalse(task.IsCompleted);
        }

        [TestMethod]
        public void TestMethod2_DueDateNull()
        {
            var task = new UserTask();
            Assert.IsNull(task.DueDate);
        }

        [TestMethod]
        public void TestMethod3_SetTitle()
        {
            var task = new UserTask { Title = "Test" };
            Assert.AreEqual("Test", task.Title);
        }

        [TestMethod]
        public void TestMethod4_SetDueDate()
        {
            var date = DateTime.Today.AddDays(1);
            var task = new UserTask { DueDate = date };
            Assert.AreEqual(date, task.DueDate);
        }
    }
}
