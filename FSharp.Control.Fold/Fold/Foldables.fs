namespace FSharp.Control.Fold

[<RequireQualifiedAccess>]
module Seq =

    let foldl (fold : Fold<'a,'b>) (sequence : seq<'a>) : 'b =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = extract (Seq.fold step initial sequence)
        }

[<RequireQualifiedAccess>]
module List =

    let foldl (fold : Fold<'a,'b>) (list : List<'a>) : 'b =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = extract (List.fold step initial list)
        }

[<RequireQualifiedAccess>]
module Set =

    let foldl (fold : Fold<'a,'b>) (set : Set<'a>) : 'b =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = extract (Set.fold step initial set)
        }

[<RequireQualifiedAccess>]
module Map =

    let foldl (fold : Fold<_,'b>) (map : Map<'k,'v>) : 'b =
        let untuple f state key value = f state (key, value)
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = extract (Map.fold (untuple step) initial map)
        }

[<RequireQualifiedAccess>]
module Option =

    let foldl (fold : Fold<'a,'b>) (option : Option<'a>) : 'b =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = extract (Option.fold step initial option)
        }

[<RequireQualifiedAccess>]
module ValueOption =

    let foldl (fold : Fold<'a,'b>) (option : ValueOption<'a>) : 'b =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = extract (ValueOption.fold step initial option)
        }

// TODO: Support for Result<'a,'b>.
