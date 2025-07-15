using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDatabase;

public static class DatabaseConfig
{

    public static string ConnectionString { get; set; } =
        "Host=ep-steep-tooth-a2jheg6u-pooler.eu-central-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_QtfWkqSJhZ58; SSL Mode=VerifyFull; Channel Binding=Require;";


    public static class ConnectionStrings
    {
        public static string Development =>
            "Host=ep-steep-tooth-a2jheg6u-pooler.eu-central-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_QtfWkqSJhZ58; SSL Mode=VerifyFull; Channel Binding=Require;";

        public static string Production =>
            "Host=ep-steep-tooth-a2jheg6u-pooler.eu-central-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_QtfWkqSJhZ58; SSL Mode=VerifyFull; Channel Binding=Require;";

        public static string LocalWithCredentials =>
            "Host=ep-steep-tooth-a2jheg6u-pooler.eu-central-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_QtfWkqSJhZ58; SSL Mode=VerifyFull; Channel Binding=Require;";
    }


    public static class BulkInsertSettings
    {

        public static int BatchSize => 1000;

        public static int RecommendBulkInsertThreshold => 1000;
    }
}
