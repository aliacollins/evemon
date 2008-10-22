namespace EVEMon.Common.Net
{
    /// <summary>
    /// Status types for an EVEMonWebException
    /// </summary>
    public enum EVEMonWebExceptionStatus
    {
        Exception,
        WebException,
        RedirectsExceeded,
        RequestsDisabled,
        ServerError,
        ProxyError,
        NameResolutionFailure,
        ConnectFailure,
        Timeout,
        XmlException,
        ImageException,
        FileError
    }
}