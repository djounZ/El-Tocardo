using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

namespace ElTocardo.ServiceDefaults;

public static  class DbCommandExtensions
{

    public static X509Certificate2? LoadFromDataBaseX509Certificate2(this  DbCommand command, string certificateName)
    {
        command.CommandText = "SELECT * FROM \"PersistentCertificates\" WHERE \"Name\" = @name";
        var param = command.CreateParameter();
        param.ParameterName = "@name";
        param.Value = certificateName;
        command.Parameters.Add(param);
        using var reader = command.ExecuteReader();
        X509Certificate2? cert = null;
        if (reader.Read())
        {
            // Map fields manually, e.g.:
            var pfxBytes = (byte[])reader["PfxBytes"];
            var password = (string)reader["Password"];
            cert = X509CertificateLoader.LoadPkcs12(pfxBytes, password);
        }
        return cert;
    }
}
