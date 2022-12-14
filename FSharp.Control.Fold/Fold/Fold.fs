namespace FSharp.Control.Fold

type FoldCont<'a,'b,'r> =
    abstract Apply : ('state -> 'a -> 'state) -> 'state -> ('state -> 'b) -> 'r

type Fold<'a,'b> =
    abstract Elim : FoldCont<'a,'b,'r> -> 'r

module Fold =

    let makeFold (step : 'state -> 'a -> 'state) (initial : 'state) (extract : 'state -> 'b) : Fold<'a,'b> =
        { new Fold<_,_> with
              member _.Elim cont = cont.Apply step initial extract
        }

    let ret a = makeFold (fun () _ -> ()) () (fun _ -> a)

    let map (f : 'b -> 'c) (fold : Fold<'a,'b>) : Fold<'a,'c> =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = makeFold step initial (extract >> f)
        }

    let contraMap (f : 'c -> 'a) (fold : Fold<'a,'b>) : Fold<'c,'b> =
        fold.Elim { new FoldCont<_,_,_> with
            member _.Apply step initial extract = makeFold (fun s a -> step s (f a)) initial extract
        }
