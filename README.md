# FSharp.Control.Fold

Using this F# library ([NuGet](https://www.nuget.org/packages/FSharp.Control.Fold/)) you define folding of data structures or streams as _values_ of type `Fold<'a,'b>` and combine smaller folds into larger folds using F# applicative computation expressions.

One benefit of this is that the programmer does not have to define combined folder functions manually, which gets tedious quickly as more folds are to be combined. Instead, combining folds is declarative and can reuse predefined fold values. Another major benefit is that combining explicit folds-as-values guarantees that the resulting `Fold<'a,'b>` will go through the data structure (or stream) exactly once, folding individual constituent values _at the same time_ and producing a `'b`. This is especially important and useful when evaluating, say, `seq<'a>` is effectful, for example when `'a` values are retrieved from a database.

`FSharp.Control.Fold` is directly based on the Haskell library [foldl](https://hackage.haskell.org/package/foldl), although the definition of the main `Fold<'a,'b>` type is different as one-to-one port is impossible.

## Supported foldable types

`FSharp.Control.Fold` includes support for folding standard F# types `seq<'a>`, `List<'a>`, `Set<'a>`, `Map<'k,'v>`, `Option<'a>` and `ValueOption<'a>`. The naming convention is that the folding function is named `foldl` ("fold left"). For example, the library provides

```fsharp
Seq.foldl (fold : Fold<'a,'b>) (sequence : seq<'a>) : 'b = ...
```

and similarly `List.foldl`, `Set.foldl` and so on.

## Basic usage

1. Open namespace `FSharp.Control.Fold`
2. Define your folding computation using function `Fold.makeFold`
3. Apply your fold using function `foldl`

Example:

```fsharp
open FSharp.Control.Fold

let length = Fold.makeFold (fun count _ -> count + 1) 0 id
List.foldl length [1;2;3;4]
```

Function `makeFold` is the main "constructor" for folds-as-values. Regular `fold` functions from the `FSharp.Core` library accept two fold "ingredients": a folder function and initial state. In contrast, `makeFold` accepts three ingredients: a folder function, initial state and a function which _transforms final state to the final result_. Therefore, in `Fold<'a,'b>` type parameter `'a` is the type of elements and `'b` is the final result, while the type of state _gets hidden_ by `makeFold` and does not occur in `Fold<'a,'b>`. This is one of the main points to understand about this library.

## Combining folds

Two or more folds can be combined into a single fold in an applicative computation expression. For example, the following expression is a fold which computes the length (count) of the data structure or stream and the sum of its elements (note the type of the final result):

```fsharp
let lengthAndSum : Fold<int, int * int> = fold {
    let! length = length // Reuse an existing Fold, see above
    and! sum = Fold.makeFold (+) 0 id
    return (length, sum)
}
```

It is of course possible to use more than two folds in a computation expression. Just add more `and!` bindings.

## Making your data types compatible with the library

If you have your own foldable data type, for example `Tree<'a>`, then you can make it compatible with `FSharp.Control.Fold` by implementing a `foldl` function for it by yourself. Just copy one of the definitions from `Foldables.fs` and adjust it by inserting the "native" `Tree.fold` function at the self-explanatory place. Do not focus on implementation details; just copy, adjust and place somewhere in your solution. For example:

```fsharp
let foldl (fold : Fold<'a,'b>) (tree : Tree<'a>) : 'b =
    fold.Elim { new FoldCont<_,_,_> with
        member _.Apply step initial extract = extract (Tree.fold step initial tree)
    }
```

## Status of the library

Although there are no tests yet, the library is certainly usable. Future releases will add the following:

- tests
- documentation comments
- more complete API description for module `Fold` in this file
- predefined folds like in the original [foldl](https://hackage.haskell.org/package/foldl) library

You can see some examples and benchmarks in (executable) projects `Examples` and `Benchmarks` respectively. These will also be extended in the future.
