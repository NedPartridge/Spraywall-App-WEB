using System.Security.Cryptography;

namespace SpraywallAppWeb.Helpers;

// A class containing methods and data relating to encrypting/decrypting passwords
public static class PasswordEncryption
{
    // Public key is public, because it can be viewed by anyone
    // The UserController endpoint makes it available to everyone, without auth
    public const string PublicKeyXML = "<RSAKeyValue><Modulus>lJvs6ObPnF99IPLtdjmZgyLcunIe4Ho4UBXaj3/GtWRUslMT0zQBf2dejlF4a9Xe5YzZMWc28DIV91W8hMBF2nszMQfngLueV8oy+ivLWlR34vqLpMVgyW2TZbfMx1/EId+9zrOpWIuAZ8cwIYulVX9fJ2RuyPyLSqszeu1vdCqs4u6S7ruq+p6aNIgUJPDq9bJelR+DHGiRv0vQ/rAIRigv38ymjledBAPfv3erNM/2OEFfw6aYOCVsLXa2aDGnGu/Cb1JZxHZVA7vKYnJ9vnKgQykS3+xq8Q0QXPFBJnEdD361Jt2ohDHj8A0LOqBO8vVWyKUOftkhK3xfQmqXhQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    
    // The private key cannot be accessed by anyone - only available within this class.
    private const string PrivateKeyXML = "<RSAKeyValue><Modulus>lJvs6ObPnF99IPLtdjmZgyLcunIe4Ho4UBXaj3/GtWRUslMT0zQBf2dejlF4a9Xe5YzZMWc28DIV91W8hMBF2nszMQfngLueV8oy+ivLWlR34vqLpMVgyW2TZbfMx1/EId+9zrOpWIuAZ8cwIYulVX9fJ2RuyPyLSqszeu1vdCqs4u6S7ruq+p6aNIgUJPDq9bJelR+DHGiRv0vQ/rAIRigv38ymjledBAPfv3erNM/2OEFfw6aYOCVsLXa2aDGnGu/Cb1JZxHZVA7vKYnJ9vnKgQykS3+xq8Q0QXPFBJnEdD361Jt2ohDHj8A0LOqBO8vVWyKUOftkhK3xfQmqXhQ==</Modulus><Exponent>AQAB</Exponent><P>wxmhhkwaIx031OXcLWAzFlk8017EbaSg/ttycbsk8yo0zXfDZwzUQeCsPPgqK8LjCD+GX1C22dcISlsIResaKHC2XeX7Lk+1P114CUmasnk4Mj4HJFwtqHkv7bZUJ0DtNmYd1s7DFRS6byxsIP4lKWEOH5w/8kE6A2l+Z8XUqBM=</P><Q>wv83E1OqYSBEMNZyYaiOXwUOLzEAdtqx2Zd7NbEbMgFpjOExa8Op9aMC6d1ghYUxsDAxJiaS9n4Kx3e4V6kbb4pxHr+9zLiqDA87EmfHgtIykEoZm+vfKGMSJLxmHCkednzEskGDS40CLqObEBwY7mPTalWd8+86ft1ey63b5Qc=</Q><DP>jhn/EgggCW9gmQqK4enti9uXeATQYWPFd5tyse+zLAGT0AvpIQexGgKAsS+314PtRLawMoBhji0W9sFzNynUNZxS1/WIi+S2iN6VbX5U1KWCDuYq4YraHpmWFTf191lM7ZBXL3wNjy83032xjB5QBuGQHen6kYJ8tLuDvCXVjE0=</DP><DQ>AII/94nxWN2HsATvOXgCHxdZSSA8mxLJAgNWK/axVnkQeHCh+2EAUYbCXC/ar56AWbBWgH37Ri9gAdM7JOdpDepzjxv698zHjM6BXfPtXlcEEMJQvMppw6Y2hugC8DHfRXnf3YM0P383sT12Cx23pu+VvqJIjjlikrArLibe+wM=</DQ><InverseQ>GgckMzY1ZhPfX1D46552E6hm9Wc1wM0goUl1aAugKKt2AbWI8boHNGzvl+j7F1J54SXmiIB4HW3h/B9xdInMx0IUwiETLM7I4ngKKv18R4oiiFSR1CZBAfQISPx5IjjCRogdhGpH04DN2SXKgOpGEFUd4K7nDD2rJFSLShFcnSk=</InverseQ><D>QyUMbtLSnRm75KY+SnuN8+VNi+RCNTSjuvOU099svcp+RjApNfU6dAEVhQIkgbyZQGhaPesftHk82Yrgaa6iISwnHhEvNUgQLCRM0iH1eHPP0ztW2vTfXUNoJL5eBPstGA0Kqpkce8hUFQ+3nICHDFFHgfnr4F3S5cLiuLkM8ZQiVY6GHXqoAy7bnFKI5QpAfE8UEcs2/Uyyc2HcMLDjELuUQZ/EqPPP26LsBa2v4plbOf+nJXeKTPue3XuHTkiF4NtRRNxKJHvYNmiS3ae4xFiag4YoIzzBVoG9JhUAoh3HCDDr7q0dqqFINPHP82/M9MG3fJdxi7IE5NFSLs56JQ==</D></RSAKeyValue>";


    // Decrypt and return hashed data
    internal static byte[] Decrypt(byte[] data)
    {
        using (RSA rsaPrivate = RSA.Create())
        {
            // Import the private key
            rsaPrivate.FromXmlString(PrivateKeyXML);

            return rsaPrivate.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }
}
