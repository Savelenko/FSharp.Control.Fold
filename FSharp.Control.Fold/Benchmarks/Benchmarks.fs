open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Configs
open FSharp.Control.Fold

open Fold

[<CategoriesColumn; GroupBenchmarksBy (BenchmarkLogicalGroupRule.ByCategory)>]
type SeqAndFoldSimple () =

    let integers = seq { 1 .. 10_000 }

    [<Benchmark (Description = "Seq.sum"); BenchmarkCategory ("Sum")>]
    member _.SeqSum () =
        Seq.sum integers

    [<Benchmark (Baseline = true, Description = "Seq.fold manual sum"); BenchmarkCategory ("Sum")>]
    member _.SeqFoldManualSum () =
        Seq.fold (fun sum a -> sum + a) 0 integers

    [<Benchmark (Description = "Fold manual sum"); BenchmarkCategory ("Sum")>]
    member _.FoldManualSum () =
        Seq.foldl (makeFold (fun sum a -> sum + a) 0 id) integers

    [<Benchmark (Baseline = true, Description = "Seq.fold constant function"); BenchmarkCategory ("Constant")>]
    member _.SeqFoldConstant () =
        Seq.fold (fun c _ -> c) "Constant result" integers

    [<Benchmark (Description = "Fold.ret"); BenchmarkCategory ("Constant")>]
    member _.FoldConstant () =
        Seq.foldl (Fold.ret "Constant result") integers

    [<Benchmark (Description = "Fold return CE"); BenchmarkCategory ("Constant")>]
    member _.FoldConstantCE () =
        integers
        |> Seq.foldl (fold {
            return "Constant result"
        })

[<CategoriesColumn; GroupBenchmarksBy (BenchmarkLogicalGroupRule.ByCategory)>]
type SeqAndFoldComposed () =

    let integers = seq { 1 .. 10_000 }

    [<Benchmark (Baseline = true, Description = "Seq.fold manual sum and max"); BenchmarkCategory ("SumMax")>]
    member _.SeqFoldManualSumMax () =
        Seq.fold (fun (s, m) a -> (s + a, max m a)) (0, Int32.MinValue) integers

    [<Benchmark (Description = "Fold sum and max CE"); BenchmarkCategory ("SumMax")>]
    member _.FoldSumMax () =
        integers
        |> Seq.foldl (fold {
            let! sum = makeFold (fun s a -> s + a) 0 id
            and! max = makeFold (fun m a -> max m a) Int32.MinValue id
            return (sum, max)
        })

[<EntryPoint>]
let main args =
    let _ = BenchmarkRunner.Run<SeqAndFoldSimple> ()
    let _ = BenchmarkRunner.Run<SeqAndFoldComposed> ()
    0