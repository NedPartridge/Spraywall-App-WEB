using System.Security.Cryptography;

namespace SpraywallAppWeb.Helpers;

// A class containing methods and data relating to encrypting/decrypting passwords
public static class PasswordEncryption
{
    // Public key is public, because it can be viewed by anyone
    // The UserController endpoint makes it available to everyone, without auth
    public const string PublicKeyXML = "<RSAKeyValue><Modulus>wEhcXFdSmJG3GZAaUq0kZ6LZQOqM0d0DYeia20p0jWON3FMtNSY+bcD6+cEEzrXYP26ILZ1QUxUAWmplw5WSyLYy496nAWWsobcjid4YMnn0oJu4hSMnasLjLRpug/ymyzYvUjPk35OnRmZQkrY/Mg9pM9DPvidjjPAWa/Hg1Y9WTjyrgKzGjf9ykt7V84uGnYdJHi6eqPZ2D3HmJleOHSGDLpd/qIIt4PPqSrTzUAAQt2dxMMachautEW40lk6k3I/Zl6BDY4Iyu9BdAll32Xa2n61NXyUBPKmsWk4bWOATt5snJHjnOYLUtkjAnGyYls79JbvrbT2UwKqgBydXCQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    // The private key cannot be accessed by anyone - only available within this class.
    private const string PrivateKeyXML = "<RSAKeyValue><Modulus>wEhcXFdSmJG3GZAaUq0kZ6LZQOqM0d0DYeia20p0jWON3FMtNSY+bcD6+cEEzrXYP26ILZ1QUxUAWmplw5WSyLYy496nAWWsobcjid4YMnn0oJu4hSMnasLjLRpug/ymyzYvUjPk35OnRmZQkrY/Mg9pM9DPvidjjPAWa/Hg1Y9WTjyrgKzGjf9ykt7V84uGnYdJHi6eqPZ2D3HmJleOHSGDLpd/qIIt4PPqSrTzUAAQt2dxMMachautEW40lk6k3I/Zl6BDY4Iyu9BdAll32Xa2n61NXyUBPKmsWk4bWOATt5snJHjnOYLUtkjAnGyYls79JbvrbT2UwKqgBydXCQ==</Modulus><Exponent>AQAB</Exponent><P>9fozj/J+/WHIKKvBiV7iuc3bCjLTaax8dRl/GGJIfrUf0nMRsW0XM3SH3hPwoPz5hanKD2wAo9AHT0+3S6mcYyta0ay8aHz8z/vtjYHDXPbJUfCzjcTDLgR6JrRZjuZsV6QTnEQc6xuBSJ0mS0XXAXC79JfRkX0jcklZb8Wtg2c=</P><Q>yB4RcKJ6Tt/59H9M5XOhZpBtP2osGW3/Mhu98dpPn0MCSp3bEn1HkxLC3xXln9/94r9PBI1wFp7igixmdy2zT1GZHJ1SkaP8e8jNzRRrVeOIU8y0Ba+twUtfoO/xlZpiaM7E4XGfL9AT8LlVYi5FU4Cy2zik2g9umLvelCV7vA8=</Q><DP>r9M/FEAkof5JUzkiyIz4yBPr5KhcEodnF1U4Uje+1kOmEYqLNSZjlTZRAF+Ee68YtYCenNN4yZ+j+f9jaEQ/M2wEKSiuLIpbNFhCgAonmcYWNeo2jrn9QmGU5yG3erwe6a6xDdxqR2yLx1n9y4SKoc7xq27yMGsg8UqZEZpd4zU=</DP><DQ>C0hetVoDyVSpdBaFUF5/mBfGgQ/MYran77xOrkcfvBv8EhbqVhWIJoulPwyuAKCRYOpmWxxaWHwmyy4TrN5/wJYaKtnX4Ow8/QGqUMi48IO+mPLup29rPX2xa5J7rXKh38TgptFQJ1L0/NqGLN3s/LNB4khESyMZmjTlbLbcThM=</DQ><InverseQ>NvoIGtlry4o9OC1rx/k2hpBn1UJofwr0NSDvS/eEkowJ1wf6zAz/obnhRy79/kuS17kHJpxmUOStTAx1qZeXY1GuKl7AUhUJRwWzJ0fL26C+yKt16o+5qrIfm7plag6064uKzK3ehIsZSwTxuSXPAocZOVsVAEQogAbEJVBrwlg=</InverseQ><D>mt2tBfKO47L49e3KRayOFapjsJU4tg58Gu9tShG9reLw7vNPBe+3eE5l1aJQbOypLu2nYJXDjCbuEeR/8fcWMfINKtq5R7bMQ7NtPC1U79Kp5HGHPDQEG0i8ECepu0RSUnlhw6NoQ2p434qoeoGuhHNFGCamK1EuLOVyKZ62p0lTvqOrVgtIBP+hOevwI/oBsA7dWv7GAwuGbtkEBk/Z0vKhHGPWsl1xcBWB82dp8pyWcftLuLXDUvDwGmKgOmS8caTy75aEmsIIxBh3ywbng1gnMRdXlmFRzpnia3YXug66MjdWn4oQQvOMoKxbMazzzs6XtIY1aI7Ab+jvMtyDwQ==</D></RSAKeyValue>";


    // Decrypt and return hashed data
    internal static byte[] Decrypt(byte[] data)
    {
        using (RSA rsaPrivate = RSA.Create())
        {
            rsaPrivate.FromXmlString(PrivateKeyXML);
            return rsaPrivate.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }
    }
}
