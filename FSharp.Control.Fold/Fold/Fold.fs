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

    let zip (fold1 : Fold<'a,'b>) (fold2 : Fold<'a,'c>) : Fold<'a,'b * 'c> =
        let zipImpl =
            { new FoldCont<_,_,_> with
                member _.Apply step1 initial1 extract1 =
                    { new FoldCont<_,_,_> with
                        member _.Apply step2 initial2 extract2 =
                            let step (struct (state1, state2)) a = struct (step1 state1 a, step2 state2 a)
                            let initial = struct (initial1, initial2)
                            let extract (struct (state1, state2)) = (extract1 state1, extract2 state2)
                            makeFold step initial extract
                    }
            }
        fold2.Elim (fold1.Elim zipImpl)

    let apply (foldF : Fold<'a,'b -> 'c>) (foldB : Fold<'a,'b>) = map (fun (f, b) -> f b) (zip foldF foldB)

    let map2 (f : 'b -> 'c -> 'd) fold1 fold2 = map (fun (b, c) -> f b c) (zip fold1 fold2)
    // TODO: Experiment (benchmark) with InlineIfLambda here and in similar cases.

    let zip3 (fold1 : Fold<'a,'b>) (fold2 : Fold<'a,'c>) (fold3 : Fold<'a,'d>) : Fold<'a,'b * 'c * 'd> =
        let zip3Impl =
            { new FoldCont<_,_,_> with
                member _.Apply step1 initial1 extract1 =
                    { new FoldCont<_,_,_> with
                        member _.Apply step2 initial2 extract2 =
                            { new FoldCont<_,_,_> with
                                member _.Apply step3 initial3 extract3 =
                                    let step (struct (state1, state2, state3)) a =
                                        struct (step1 state1 a, step2 state2 a, step3 state3 a)
                                    let initial = struct (initial1, initial2, initial3)
                                    let extract (struct (state1, state2, state3)) =
                                        (extract1 state1, extract2 state2, extract3 state3)
                                    makeFold step initial extract
                            }
                    }
            }
        fold3.Elim (fold2.Elim (fold1.Elim zip3Impl))

    let map3 (f : 'b -> 'c -> 'd -> 'e) fold1 fold2 fold3 = map (fun (b, c, d) -> f b c d) (zip3 fold1 fold2 fold3)

    let zip4 (fold1 : Fold<'a,'b>) (fold2 : Fold<'a,'c>) (fold3 : Fold<'a,'d>) (fold4 : Fold<'a,'e>)
        : Fold<'a,'b * 'c * 'd * 'e> =
        let zip4Impl =
            { new FoldCont<_,_,_> with
                member _.Apply step1 initial1 extract1 =
                    { new FoldCont<_,_,_> with
                        member _.Apply step2 initial2 extract2 =
                            { new FoldCont<_,_,_> with
                                member _.Apply step3 initial3 extract3 =
                                    { new FoldCont<_,_,_> with
                                        member _.Apply step4 initial4 extract4 =
                                            let step (struct (state1, state2, state3, state4)) a =
                                                struct (step1 state1 a, step2 state2 a, step3 state3 a, step4 state4 a)
                                            let initial = struct (initial1, initial2, initial3, initial4)
                                            let extract (struct (state1, state2, state3, state4)) =
                                                (extract1 state1, extract2 state2, extract3 state3, extract4 state4)
                                            makeFold step initial extract
                                    }
                            }
                    }
            }
        fold4.Elim (fold3.Elim (fold2.Elim (fold1.Elim zip4Impl)))

    let map4 (f : 'b -> 'c -> 'd -> 'e -> 'f) fold1 fold2 fold3 fold4 =
        map (fun (b, c, d, e) -> f b c d e) (zip4 fold1 fold2 fold3 fold4)

[<AutoOpen>]
module CE =

    type FoldBuilder () =
        member _.BindReturn (fold, f) = Fold.map f fold
        // TODO: Define BindNReturn?
        member _.MergeSources (fold1, fold2) = Fold.zip fold1 fold2
        member _.MergeSources3 (fold1, fold2, fold3) = Fold.zip3 fold1 fold2 fold3
        member _.MergeSources4 (fold1, fold2, fold3, fold4) = Fold.zip4 fold1 fold2 fold3 fold4

    let fold = FoldBuilder ()