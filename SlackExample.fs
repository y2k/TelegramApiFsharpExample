module SlackExample

open System
open SlackAPI

let userId = Environment.GetEnvironmentVariable "SLACK_API_USERID"
let password = Environment.GetEnvironmentVariable "SLACK_API_PASSWORD"

let test =
    async {
        let! response =
            SlackTaskClient.AuthSignin (userId, "T09229ZC6", password)
            |> Async.AwaitTask

        printfn 
            "team = %O | token = %O | user = %O | error = %O | ok = %O" 
            response.team response.token response.user response.error response.ok

        let client = SlackTaskClient response.token

        let! channels = client.GetChannelListAsync () |> Async.AwaitTask

        channels.channels 
        |> Array.sortByDescending (fun x -> x.num_members) 
        |> Array.map (fun x -> sprintf "%s (%i)" x.name x.num_members)
        |> printfn "Channels = %A"

    }