﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DG.Some.Namespace;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using DG.XrmFramework.BusinessDomain.ServiceContext;

namespace DG.XrmMockupTest {
    [TestClass]
    public class TestStates : UnitTestBase {
        [TestMethod]
        public void TestSetState() {
            var acc = new Account() {
                StateCode = AccountState.Active,
                StatusCode = Account_StatusCode.Active
            };
            acc.Id = orgAdminUIService.Create(acc);
            acc.SetState(orgAdminUIService, AccountState.Inactive, Account_StatusCode.Inactive);

            var retrieved = orgAdminUIService.Retrieve(Account.EntityLogicalName, acc.Id, new ColumnSet(true)) as Account;
            Assert.AreEqual(AccountState.Inactive, retrieved.StateCode);
        }

#if !(XRM_MOCKUP_TEST_2011 || XRM_MOCKUP_TEST_2013)
        [TestMethod]
        public void TestStatusTransitions()
        {
            var man = new dg_man() {
                statecode = dg_manState.Active,
                statuscode = dg_man_statuscode.Active
            };
            man.Id = orgAdminUIService.Create(man);
            man.SetState(orgAdminUIService, dg_manState.Inactive, dg_man_statuscode.Inactive);

            var retrieved = orgAdminUIService.Retrieve(dg_man.EntityLogicalName, man.Id, new ColumnSet(true)) as dg_man;
            Assert.AreEqual(dg_manState.Inactive, retrieved.statecode);
            Assert.AreEqual(dg_man_statuscode.Inactive, retrieved.statuscode);

            try {
                man.SetState(orgAdminUIService, dg_manState.Active, dg_man_statuscode.Active);
                Assert.Fail();
            } catch(Exception e) {
                Assert.IsInstanceOfType(e, typeof(FaultException));
            }
        }
#endif

        [TestMethod]
        public void TestStateStatuscodeCreate() {
            var incidentId = orgAdminService.Create(new Incident { });
            var incident = orgAdminService.Retrieve(Incident.EntityLogicalName, incidentId, new ColumnSet(true)).ToEntity<Incident>();
            Assert.AreEqual(IncidentState.Active, incident.StateCode);
            Assert.AreEqual(Incident_StatusCode.OnHold, incident.StatusCode);

            var accId = orgAdminService.Create(new Account { StateCode = AccountState.Inactive });
            var acc = orgAdminService.Retrieve(Account.EntityLogicalName, accId, new ColumnSet(true)).ToEntity<Account>();
            Assert.AreEqual(AccountState.Active, acc.StateCode);
            Assert.AreEqual(Account_StatusCode.Somestatus, acc.StatusCode);

            accId = orgAdminService.Create(new Account { StatusCode = Account_StatusCode.Active });
            acc = orgAdminService.Retrieve(Account.EntityLogicalName, accId, new ColumnSet(true)).ToEntity<Account>();
            Assert.AreEqual(AccountState.Active, acc.StateCode);
            Assert.AreEqual(Account_StatusCode.Active, acc.StatusCode);
        }


        [TestMethod]
        public void TestStateStatuscodeUpdate() {
            var acc = new Account { };
            acc.Id = orgAdminService.Create(acc);
            var retrieved = orgAdminService.Retrieve(Account.EntityLogicalName, acc.Id, new ColumnSet(true)).ToEntity<Account>();
            Assert.AreEqual(AccountState.Active, retrieved.StateCode);
            Assert.AreEqual(Account_StatusCode.Somestatus, retrieved.StatusCode);

            acc.StateCode = AccountState.Inactive;
            orgAdminService.Update(acc);
            
            retrieved = orgAdminService.Retrieve(Account.EntityLogicalName, acc.Id, new ColumnSet(true)).ToEntity<Account>();
            Assert.AreEqual(AccountState.Inactive, retrieved.StateCode);
            Assert.AreEqual(Account_StatusCode.Inactive, retrieved.StatusCode);

            acc.StateCode = null;
            acc.StatusCode = Account_StatusCode.Active;
            orgAdminService.Update(acc);

            retrieved = orgAdminService.Retrieve(Account.EntityLogicalName, acc.Id, new ColumnSet(true)).ToEntity<Account>();
            Assert.AreEqual(AccountState.Active, retrieved.StateCode);
            Assert.AreEqual(Account_StatusCode.Active, retrieved.StatusCode);
        }
    }
}
