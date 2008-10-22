using System;
using System.IO;
using System.Net;
using EVEMon.Common.Net;
using NUnit.Framework;
using TypeMock;

namespace Tests.EVEMon.Common.Net
{
    [TestFixture]
    [VerifyMocks]
    public class EVEMonWebRequestTests
    {
        private string _url;
        private string _accept;
        private Stream _responseStream ;
        private EVEMonWebClientState _webClientState;

        [SetUp]
        public void Setup()
        {
            _webClientState = new EVEMonWebClientState();
            _url = "http://battleclinic.com/dummyurl.php";
            _accept = "text/xml";
            _responseStream = new MemoryStream();
        }

        [TearDown]
        public void TearDown()
        {
            _responseStream.Close();
        }

        #region Core response exception tests
        [Test]
        [VerifyMocks]
        public void GetResponseWebExceptionTest()
        {
            EVEMonWebRequest request = new EVEMonWebRequest(_webClientState);
            Mock<HttpWebRequest> webRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            webRequest.ExpectAndThrow("GetResponse", new WebException());
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown");
            }
            catch (EVEMonWebException ex)
            {
                Assert.AreEqual(EVEMonWebExceptionStatus.WebException, ex.Status, "Incorrect exception type");
            }
        }

        [Test]
        [VerifyMocks]
        public void GetResponseRedirectsExceededTest()
        {
            EVEMonWebRequest request = new EVEMonWebRequest(_webClientState);
            Mock<HttpWebResponse> webResponse = MockManager.MockAll<HttpWebResponse>(Constructor.NotMocked);
            webResponse.ExpectGetAlways("StatusCode", HttpStatusCode.Redirect);
            webResponse.ExpectAndReturn("GetResponseHeader", _url, _webClientState.MaxRedirects + 1);
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown");
            }
            catch (EVEMonWebException ex)
            {
                Assert.AreEqual(EVEMonWebExceptionStatus.RedirectsExceeded, ex.Status, "Incorrect exception type");
            }
        }

        [Test]
        [VerifyMocks]
        public void ProxyTests()
        {
            EVEMonWebRequest request = new EVEMonWebRequest(_webClientState);
            _webClientState.DisableOnProxyAuthenticationFailure = true;
            MockObject<HttpWebResponse> webResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            webResponse.ExpectGetAlways("StatusCode", HttpStatusCode.ProxyAuthenticationRequired);
            Mock<HttpWebRequest> webRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            webRequest.ExpectAndThrow("GetResponse", new WebException("", new Exception(), WebExceptionStatus.ProtocolError, webResponse.Object));
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown for Authentication Failure");
            }
            catch(EVEMonWebException ex)
            {
                Assert.AreEqual(EVEMonWebExceptionStatus.ProxyError, ex.Status, "Incorrect exception type for Authentication Failure");
            }
            Assert.IsTrue(_webClientState.RequestsDisabled, "Web Requests not disabled");
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown");
            }
            catch(EVEMonWebException ex)
            {
                Assert.AreEqual(EVEMonWebExceptionStatus.RequestsDisabled, ex.Status, "Incorrect exception type");
            }
            _webClientState.RequestsDisabled = false;
        }

        #endregion

    }
}
