namespace SpraywallAppWeb.Helpers;

// Class to store authentication-related data
public static class AuthSettings
{
    // Private key - not to be shared, used to decrypt authentication tokens.
    // May be stored as a public variable because the server is hosted locally,
    // the only access is through the API endpoints, which are controlled.
    public static string PrivateKey { get; set; } = "C'est-LA__Pr1AtE,KEy!,Unst,Unst,W#L0Ve,5eCu(ity!unst+unst2u3jyg0d372&&2y197y9*u~~`|}{|][][yeahYEAH!";
}