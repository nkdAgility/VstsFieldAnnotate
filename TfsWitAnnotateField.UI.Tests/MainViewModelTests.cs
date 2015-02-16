using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsWitAnnotateField.UI.ViewModel;

namespace TfsWitAnnotateField.UI.Tests
{
    [TestClass]
    public class MainViewModelTests
    {

        private MainViewModel vm;
        private int workItemToLoad = 25;

        [TestInitialize]
        public void Setup()
        {
            this.vm = new MainViewModel(new MockCollectionSelector());
        }

        private void LoadWorkItem()
        {
            this.vm.ConnectCommand.Execute(null);
            this.vm.SelectedWorkItemId = workItemToLoad;
        }

        [TestMethod]
        public void TestConnection()
        {
            this.vm.ConnectCommand.Execute(null);
            Assert.AreEqual<bool>(true, this.vm.IsConnected);
        }

        [TestMethod]
        public void TestIsWorkItemSelected()
        {
            this.LoadWorkItem();
            Assert.AreEqual<bool>(true, this.vm.IsWorkItemSelected);
        }

        [TestMethod]
        public void TestWorkItemIdSet()
        {
            this.LoadWorkItem();
            Assert.AreEqual<int>(workItemToLoad, this.vm.SelectedWorkItemId);
        }

        //[TestMethod]
        //public void TestWorkItemSelected()
        //{
        //    this.LoadWorkItem();
        //    Assert.IsNotNull(vm.SelectedWorkItem);
        //}
    }
}
