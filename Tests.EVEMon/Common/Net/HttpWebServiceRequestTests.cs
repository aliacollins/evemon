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
    public class HttpWebServiceRequestTests
    {
        private string _url;
        private string _accept;
        private Stream _responseStream ;
        private HttpWebServiceState _webServiceState;

        [SetUp]
        public void Setup()
        {
            _webServiceState = new HttpWebServiceState();
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
            HttpWebServiceRequest request = new HttpWebServiceRequest(_webServiceState);
            Mock<HttpWebRequest> webRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            webRequest.ExpectAndThrow("GetResponse", new WebException());
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown");
            }
            catch (HttpWebServiceException ex)
            {
                Assert.AreEqual(HttpWebServiceExceptionStatus.WebException, ex.Status, "Incorrect exception type");
            }
        }

        [Test]
        [VerifyMocks]
        public void GetResponseRedirectsExceededTest()
        {
            HttpWebServiceRequest request = new HttpWebServiceRequest(_webServiceState);
            Mock<HttpWebResponse> webResponse = MockManager.MockAll<HttpWebResponse>(Constructor.NotMocked);
            webResponse.ExpectGetAlways("StatusCode", HttpStatusCode.Redirect);
            webResponse.ExpectAndReturn("GetResponseHeader", _url, _webServiceState.MaxRedirects + 1);
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown");
            }
            catch (HttpWebServiceException ex)
            {
                Assert.AreEqual(HttpWebServiceExceptionStatus.RedirectsExceeded, ex.Status, "Incorrect exception type");
            }
        }

        [Test]
        [VerifyMocks]
        public void ProxyTests()
        {
            HttpWebServiceRequest request = new HttpWebServiceRequest(_webServiceState);
            _webServiceState.DisableOnProxyAuthenticationFailure = true;
            MockObject<HttpWebResponse> webResponse = MockManager.MockObject<HttpWebResponse>(Constructor.Mocked);
            webResponse.ExpectGetAlways("StatusCode", HttpStatusCode.ProxyAuthenticationRequired);
            Mock<HttpWebRequest> webRequest = MockManager.Mock<HttpWebRequest>(Constructor.NotMocked);
            webRequest.ExpectAndThrow("GetResponse", new WebException("", new Exception(), WebExceptionStatus.ProtocolError, webResponse.Object));
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown for Authentication Failure");
            }
            catch(HttpWebServiceException ex)
            {
                Assert.AreEqual(HttpWebServiceExceptionStatus.ProxyError, ex.Status, "Incorrect exception type for Authentication Failure");
            }
            Assert.IsTrue(_webServiceState.RequestsDisabled, "Web Requests not disabled");
            try
            {
                request.GetResponse(_url, _responseStream, _accept);
                Assert.Fail("No exception thrown");
            }
            catch(HttpWebServiceException ex)
            {
                Assert.AreEqual(HttpWebServiceExceptionStatus.RequestsDisabled, ex.Status, "Incorrect exception type");
            }
            _webServiceState.RequestsDisabled = false;
        }

        #endregion

    }
}
