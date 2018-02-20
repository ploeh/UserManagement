#r @"packages/FAKE.4.64.5/tools/FakeLib.dll"

open Fake

Target "Build" <| fun _ ->
    !! "**/UserManagement.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore


RunTargetOrDefault "Build"
