module Ploeh.Samples.UserManagement.UserManagementTests

open System
open System.Collections.Generic
open Ploeh.Samples.UserManagement.UserManagementModel
open Xunit
open Hedgehog
open Swensen.Unquote

module Gen =
    let user = gen {
        let! id = Range.linear 1 1_000_000 |> Gen.int
        return { UserId = id; ConnectedUsers = [] } }

    let withOtherId user = gen {
        let! i = Range.linear 1 1000 |> Gen.int
        return { user with UserId = user.UserId + i } }

type FakeDB () =
    let users = Dictionary<int, User> ()

    member val IsDirty = false with get, set

    member this.AddUser user =
        this.IsDirty <- true
        users.Add (user.UserId, user)

    member this.TryFind i =
        match users.TryGetValue i with
        | false, _ -> None
        | true,  u -> Some u

    member this.LookupUser s =
        match Int32.TryParse s with
        | false, _ -> Error InvalidId
        | true, i ->
            match users.TryGetValue i with
            | false, _ -> Error NotFound
            | true, u -> Ok u

    member this.UpdateUser u =
        this.IsDirty <- true
        users.[u.UserId] <- u

let isIdInvalid s = Int32.TryParse s |> fst |> not

[<Fact>]
let ``Users successfully connect`` () = Property.check <| property {
    let! user = Gen.user
    let! otherUser = Gen.withOtherId user
    let db = FakeDB ()
    db.AddUser user
    db.AddUser otherUser

    let actual = post db.LookupUser db.UpdateUser (string user.UserId) (string otherUser.UserId)

    test <@ db.TryFind user.UserId
            |> Option.exists
                (fun u -> u.ConnectedUsers |> List.contains otherUser) @>
    test <@ isOK actual @> }

[<Fact>]
let ``Users don't connect when user doesn't exist`` () = Property.check <| property {
    let! i = Range.linear 1 1_000_000 |> Gen.int
    let! otherUser = Gen.user
    let db = FakeDB ()
    db.AddUser otherUser
    db.IsDirty <- false
    let uniqueUserId = string (otherUser.UserId + i)

    let actual = post db.LookupUser db.UpdateUser uniqueUserId (string otherUser.UserId)

    test <@ not db.IsDirty @>
    test <@ isBadRequest actual @> }

[<Fact>]
let ``Users don't connect when other user doesn't exist`` () = Property.check <| property {
    let! i = Range.linear 1 1_000_000 |> Gen.int
    let! user = Gen.user
    let db = FakeDB ()
    db.AddUser user
    db.IsDirty <- false
    let uniqueOtherUserId = string (user.UserId + i)

    let actual = post db.LookupUser db.UpdateUser (string user.UserId) uniqueOtherUserId 

    test <@ not db.IsDirty @>
    test <@ isBadRequest actual @> }

[<Fact>]
let ``Users don't connect when user Id is invalid`` () = Property.check <| property {
    let! s = Gen.alphaNum |> Gen.string (Range.linear 0 100) |> Gen.filter isIdInvalid
    let! otherUser = Gen.user
    let db = FakeDB ()
    db.AddUser otherUser
    db.IsDirty <- false

    let actual = post db.LookupUser db.UpdateUser s (string otherUser.UserId)

    test <@ not db.IsDirty @>
    test <@ isBadRequest actual @> }

[<Fact>]
let ``Users don't connect when other user Id is invalid`` () = Property.check <| property {
    let! s = Gen.alphaNum |> Gen.string (Range.linear 0 100) |> Gen.filter isIdInvalid
    let! user = Gen.user
    let db = FakeDB ()
    db.AddUser user
    db.IsDirty <- false

    let actual = post db.LookupUser db.UpdateUser (string user.UserId) s

    test <@ not db.IsDirty @>
    test <@ isBadRequest actual @> }