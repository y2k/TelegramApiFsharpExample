[<EntryPoint>]
let main _ =
    SlackExample.test |> Async.RunSynchronously
    0