namespace Azure.ServiceBus.Demo.Common
{
    public static class AccessData
    {
        public static string ServiceBusNamespace = "demobustest";

        public static string SasTestKeyName = "TestKey";
        public static string SasTestKeyValue = "j5dX2Vs55R03cqB9UQMPJasVh4jXXJq28yEM0Q8jHl8=";

        public static string SasRootKeyName = "RootManageSharedAccessKey";
        public static string SasRootKeyValue = "iXiHWoEK3gD/2aJ1qELDhwCZO+A9SIv2LxuBNprc93k=";

        public static string ServiceBusRootKeyConnectionString 
            = "Endpoint=sb://demobustest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iXiHWoEK3gD/2aJ1qELDhwCZO+A9SIv2LxuBNprc93k=";
        public static string ServiceBusTestKeyConnectionString 
            = "Endpoint=sb://demobustest.servicebus.windows.net/;SharedAccessKeyName=TestKey;SharedAccessKey=j5dX2Vs55R03cqB9UQMPJasVh4jXXJq28yEM0Q8jHl8=";

        public static string ServiceBusConfigConnectionStringName = "Azure.ServiceBus.ConnectionString";
    }
}
