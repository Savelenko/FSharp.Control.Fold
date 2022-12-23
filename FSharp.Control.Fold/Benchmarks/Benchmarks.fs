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

[<EntryPoint>]
let main args =
    let _ = BenchmarkRunner.Run<SeqAndFoldSimple> ()
    0