module SlackExample

open System
open System.Diagnostics
open System.Web
open SlackAPI

let clientId = "PUT_CLIENT_ID_FROM_SLACK_APPLICATION_REGISTATION_HERE";
let clientSecret = "PUT_CLIENT_SECRET_FROM_SLACK_APPLICATION_REGISTATION_HERE";
let redirectUri = "PUT_REDIRECT_FROM_SLACK_APPLICATION_REGISTATION_HERE";

let test () =
    let state = Guid.NewGuid().ToString()
    let uri = SlackClient.GetAuthorizeUri(clientId, SlackScope.Identify + SlackScope.Read + SlackScope.Post, redirectUri, state, "socialsaleslounge")
    Console.WriteLine("Directing to: " + (string uri))
    Process.Start("open", uri.ToString()) |> ignore

    Console.WriteLine("Paste in the URL of the authentication result...")
    let mutable asString = Console.ReadLine()
    let index = asString.IndexOf('?')
    if index <> -1 then
        asString <- asString.Substring(index + 1)

    let qs = HttpUtility.ParseQueryString(asString)
    let code = qs.["code"]
    let newState = qs.["state"]

    if state <> newState then
        raise <| new InvalidOperationException("State mismatch.")

    Console.WriteLine("Requesting access token...")
    SlackClient.GetAccessToken(
        Action<_>(fun response -> 
            let accessToken = response.access_token;
            Console.WriteLine("Got access token '{0}'...", accessToken);

            // post...
            let client = new SlackClient(accessToken);
            client.PostMessage(null, "#registrations", "Test", "Jo the Robot");
            ()), 
        clientId, clientSecret, redirectUri, code)
    Console.WriteLine("Done.");

let foo () = ()