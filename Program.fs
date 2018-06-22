open System
open TLSharp.Core
open TLSharp.Core.Network
open TeleSharp.TL

module ClientFactory =
    open Starksoft.Aspen.Proxy

    let proxy = Environment.GetEnvironmentVariable "PROXY_HOST"
    let auth = Environment.GetEnvironmentVariable "PROXY_AUTH"

    let make host port = 
        let proxyClient = Socks5ProxyClient(
                              proxy.Split(":").[0], proxy.Split(":").[1] |> int, 
                              auth.Split(":").[0], auth.Split(":").[1])
        proxyClient.CreateConnection(host, port)

[<EntryPoint>]
let main _ =
    async {
        use client = new TelegramClient(
                         int <| Environment.GetEnvironmentVariable "TELEGRAM_APIID", 
                         Environment.GetEnvironmentVariable "TELEGRAM_APIHASH", 
                         handler = TcpClientConnectionHandler(ClientFactory.make))
        do! client.ConnectAsync() |> Async.AwaitTask

        if not <| client.IsUserAuthorized() then
            let number = Environment.GetEnvironmentVariable "USER_PHONENUMBER"
            let! hash = client.SendCodeRequestAsync(number) |> Async.AwaitTask

            printfn "Enter code:"
            let code = Console.ReadLine()

            let! user = client.MakeAuthAsync(number, hash, code) |> Async.AwaitTask
            printfn "User = %O" user.FirstName

        let! contacts = client.GetContactsAsync() |> Async.AwaitTask
        contacts.Users 
        |> Seq.map (fun x -> (x :?> TLUser).FirstName)
        |> Seq.toList
        |> printfn "Users: %A"
    } |> Async.RunSynchronously
    0