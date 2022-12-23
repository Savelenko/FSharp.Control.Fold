open System
open FSharp.Control.Fold

open Fold

[<EntryPoint>]
let main args =
    // Example foldable data structures
    let list = [1 .. 1_000]

    // Example 1
    let length = makeFold (fun count _ -> count + 1) 0 id
    printfn $"Example 1: {List.foldl length list}"

    // Example 2
    let lengthAndSum = fold {
        let! length = length // Reuse an existing Fold
        and! sum = makeFold (+) 0 id
        return (length, sum)
    }
    printfn $"Example 2: {List.foldl lengthAndSum list}"

    0