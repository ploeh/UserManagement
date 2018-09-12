module Ploeh.Samples.UserManagement.UserManagementModel

type HttpResponse<'a> = OK of 'a | BadRequest of string

let isOK = function OK _ -> true | _ -> false

let isBadRequest = function BadRequest _ -> true | _ -> false

type User = { UserId : int; ConnectedUsers : User list }

let addConnection user otherUser =
    { user with ConnectedUsers = otherUser :: user.ConnectedUsers }

type UserLookupError = InvalidId | NotFound

type ResultBuilder () =
    member this.Bind (r, f) = Result.bind f r
    member this.Return x = Ok x
    member this.ReturnFrom x = x

let result = ResultBuilder ()

let post lookupUser updateUser userId otherUserId =
    let userRes =
        lookupUser userId |> Result.mapError (function
            | InvalidId -> "Invalid user ID."
            | NotFound  -> "User not found.")
    let otherUserRes =
        lookupUser otherUserId |> Result.mapError (function
            | InvalidId -> "Invalid ID for other user."
            | NotFound  -> "Other user not found.")

    let connect = result {
        let! user = userRes
        let! otherUser = otherUserRes
        addConnection user otherUser |> updateUser
        return otherUser }

    match connect with Ok u -> OK u | Error msg -> BadRequest msg