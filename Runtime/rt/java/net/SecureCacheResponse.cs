//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.net
{
    public abstract partial class SecureCacheResponse : CacheResponse
    {
        public SecureCacheResponse(){}
        public global::java.lang.String  CipherSuite { get; private set;}
        public global::java.util.List<global::java.security.cert.Certificate>  LocalCertificateChain { get; private set;}
        public global::java.security.Principal  LocalPrincipal { get; private set;}
        public global::java.security.Principal  PeerPrincipal { get; private set;}
        public global::java.util.List<global::java.security.cert.Certificate>  ServerCertificateChain { get; private set;}
    }
}