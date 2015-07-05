#I "../packages" 
#r "Streams/lib/net45/Streams.Core.dll"
#r "MBrace.Flow/lib/net45/MBrace.Flow.dll"
#load "MBrace.Azure.Standalone/MBrace.Azure.fsx"

namespace global

[<AutoOpen>]
module ConnectionStrings = 

    open MBrace.Core
    open MBrace.Azure
    open MBrace.Azure.Client
    open MBrace.Azure.Runtime

    let myStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=???;AccountKey=???"
    let myServiceBusConnectionString = "Endpoint=sb://???/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=???"

    let config =
        { Configuration.Default with
            StorageConnectionString = myStorageConnectionString
            ServiceBusConnectionString = myServiceBusConnectionString }
