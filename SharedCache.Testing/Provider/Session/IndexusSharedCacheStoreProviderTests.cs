using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Moq;
using NUnit.Framework;
using SharedCache.WinServiceCommon.Provider.Session;

namespace SharedCache.Testing.Provider.Session
{
    [TestFixture]
    public class IndexusSharedCacheStoreProviderTests
    {
        private Mock<SharedCache.WinServiceCommon.Provider.Cache.IndexusSharedCacheProvider> _cacheImpl;
        private IndexusSharedCacheStoreProvider _provider;
        private HttpContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new HttpContext(new HttpRequest(null, "http://bla/", null), new HttpResponse(new StringWriter()));
            _cacheImpl = new Mock<SharedCache.WinServiceCommon.Provider.Cache.IndexusSharedCacheProvider>();
            _provider = new IndexusSharedCacheStoreProvider();
            _provider.Cache = _cacheImpl.Object;
            _provider.AppID = "UnitTest";
        }

        [Test]
        public void CanCreateNewStoreDataWithNullContext()
        {
            SessionStateStoreData data = _provider.CreateNewStoreData(null, 20);

            Assert.IsNotNull(data);
            Assert.AreEqual(20, data.Timeout);
            Assert.IsEmpty(data.StaticObjects);
            Assert.IsEmpty(data.Items);
        }

        [Test]
        public void CanCreateNewStoreDataWithContext()
        {
            SessionStateStoreData data = _provider.CreateNewStoreData(_context, 40);

            Assert.IsNotNull(data);
            Assert.AreEqual(40, data.Timeout);
            Assert.AreEqual(_context.Application.GetType().GetProperty("SessionStaticObjects", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_context.Application, null), data.StaticObjects);
            Assert.IsEmpty(data.Items);
        }

        [Test]
        public void CreateUninitializedItemSavesToCache()
        {
            _cacheImpl.Setup(cache => cache.Add("Session(THIS_IS_MY_ID, UnitTest)", It.IsAny<object>(), It.IsInRange(DateTime.Now.AddMinutes(39), DateTime.Now.AddMinutes(41), Range.Inclusive)));

            _provider.CreateUninitializedItem(_context, "THIS_IS_MY_ID", 40);

            _cacheImpl.VerifyAll();
        }

        [Test]
        public void RemoveItemDeletesFromCache()
        {
            _cacheImpl.Setup(cache => cache.Remove("Session(THIS_IS_MY_ID, UnitTest)"));

            _provider.RemoveItem(_context, "THIS_IS_MY_ID", null, null);

            _cacheImpl.VerifyAll();
        }

        [Test]
        public void SetAndReleaseItemExclusiveReleasesLockAndUpdatesTimeout()
        {
            _cacheImpl.Setup(cache => cache.Add("Session(THIS_IS_MY_ID, UnitTest)", It.IsAny<object>(), It.IsInRange(DateTime.Now.AddMinutes(59), DateTime.Now.AddMinutes(61), Range.Inclusive)));

            _provider.SetAndReleaseItemExclusive(_context, "THIS_IS_MY_ID", new SessionStateStoreData(null, null, 60), null, true);

            _cacheImpl.VerifyAll();
        }

        [Test]
        public void ReleaseItemExclusiveGetsAndResavesObject()
        {
            _cacheImpl.Setup(cache => cache.Get<byte[]>("Session(THIS_IS_MY_ID, UnitTest)")).Returns(new byte[] { 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF });
            _cacheImpl.Setup(cache => cache.Add("Session(THIS_IS_MY_ID, UnitTest)", It.IsAny<object>(), It.IsInRange(DateTime.Now.AddMinutes(19), DateTime.Now.AddMinutes(21), Range.Inclusive)));

            _provider.ReleaseItemExclusive(_context, "THIS_IS_MY_ID", null);

            _cacheImpl.VerifyAll();
        }

        [Test]
        public void ResetItemTimeoutExtendsTTl()
        {
            _cacheImpl.Setup(cache => cache.ExtendTtl("Session(THIS_IS_MY_ID, UnitTest)", It.IsInRange(DateTime.Now.AddMinutes(19), DateTime.Now.AddMinutes(21), Range.Inclusive)));

            _provider.CreateNewStoreData(_context, 20); //needed to cache timeout
            _provider.ResetItemTimeout(_context, "THIS_IS_MY_ID");

            _cacheImpl.VerifyAll();
        }

        [Test]
        public void GetNonexistentItemReturnsNull()
        {
            _cacheImpl.Setup(cache => cache.Get<byte[]>("Session(THIS_IS_MY_ID, UnitTest)")).Returns((byte[])null);

            bool locked;
            TimeSpan lockAge;
            object lockId;
            SessionStateActions actions;

            SessionStateStoreData data = _provider.GetItem(_context, "THIS_IS_MY_ID", out locked, out lockAge, out lockId, out actions);

            _cacheImpl.VerifyAll();

            Assert.IsNull(data);
            Assert.IsFalse(locked);
            Assert.AreEqual(TimeSpan.Zero, lockAge);
            Assert.AreEqual(SessionStateActions.None, actions);
        }

        [Test]
        public void GetItemCreatesNoLock()
        {
            _cacheImpl.Setup(cache => cache.Get<byte[]>("Session(THIS_IS_MY_ID, UnitTest)")).Returns(new byte[] { 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF });

            bool locked;
            TimeSpan lockAge;
            object lockId;
            SessionStateActions actions;

            SessionStateStoreData data = _provider.GetItem(_context, "THIS_IS_MY_ID", out locked, out lockAge, out lockId, out actions);

            _cacheImpl.VerifyAll();

            Assert.IsNotNull(data);
            Assert.IsFalse(locked);
            Assert.AreEqual(TimeSpan.Zero, lockAge);
            Assert.AreEqual(SessionStateActions.None, actions);
        }

        [Test]
        public void GetLockedItemReturnsNull()
        {
            _cacheImpl.Setup(cache => cache.Get<byte[]>("Session(THIS_IS_MY_ID, UnitTest)")).Returns(new byte[] { 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF });

            bool locked;
            TimeSpan lockAge;
            object lockId;
            SessionStateActions actions;

            SessionStateStoreData data = _provider.GetItem(_context, "THIS_IS_MY_ID", out locked, out lockAge, out lockId, out actions);

            _cacheImpl.VerifyAll();

            Assert.IsNull(data);
            Assert.IsTrue(locked);
            Assert.That(DateTime.Now.Subtract(new DateTime()) - lockAge < TimeSpan.FromMilliseconds(100));
            Assert.AreEqual(SessionStateActions.None, actions);
        }

        [Test]
        public void GetItemExclusiveCreatesLock()
        {
            _cacheImpl.Setup(cache => cache.Get<byte[]>("Session(THIS_IS_MY_ID, UnitTest)")).Returns(new byte[] { 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF });
            _cacheImpl.Setup(cache => cache.Add("Session(THIS_IS_MY_ID, UnitTest)", (object) It.Is<byte[]>(bytes => bytes[6] == 0x01), It.IsInRange(DateTime.Now.AddMinutes(19), DateTime.Now.AddMinutes(21), Range.Inclusive)));

            bool locked;
            TimeSpan lockAge;
            object lockId;
            SessionStateActions actions;

            SessionStateStoreData data = _provider.GetItemExclusive(_context, "THIS_IS_MY_ID", out locked, out lockAge, out lockId, out actions);

            _cacheImpl.VerifyAll();

            Assert.IsNotNull(data);
            Assert.IsFalse(locked);
            Assert.AreEqual(TimeSpan.Zero, lockAge);
            Assert.AreEqual(SessionStateActions.None, actions);
        }
    }
}
