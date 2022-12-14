namespace FSharp.Control.Fold

type FoldCont<'a,'b,'r> =
    abstract Apply : ('state -> 'a -> 'state) -> 'state -> ('state -> 'b) -> 'r

type Fold<'a,'b> =
    abstract Elim : FoldCont<'a,'b,'r> -> 'r