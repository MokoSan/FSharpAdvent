#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"

open FSharp.Data

type Race = 
    | Hobbit
    | Elf
    | Dwarf
    | Human 
    | NotFound

type CharacterInfo = { Name : string; Url : string; Race : Race }

let createBaseCharacterInfoFromData ( name : string ) 
                                    ( url : string  ) : CharacterInfo =
    { Name = name; Url = url; Race = NotFound }

[<Literal>]
let baseWikiURL  = @"http://lotr.wikia.com/" 

[<Literal>]
let characterURLPage1 = @"http://lotr.wikia.com/wiki/Category:Characters" 
[<Literal>]
let characterURLPage2 = @"http://lotr.wikia.com/wiki/Category:Characters?page=2" 
[<Literal>]
let characterURLPage3 = @"http://lotr.wikia.com/wiki/Category:Characters?page=3" 
[<Literal>]
let characterURLPage4 = @"http://lotr.wikia.com/wiki/Category:Characters?page=4" 
[<Literal>]
let characterURLPage5 = @"http://lotr.wikia.com/wiki/Category:Characters?page=5" 

type LotrCharacterProviderPage1 = HtmlProvider< characterURLPage1 >
let lotrCharacterProviderPage1  = LotrCharacterProviderPage1.Load( characterURLPage1 )

type LotrCharacterProviderPage2 = HtmlProvider< characterURLPage2 >
let lotrCharacterProviderPage2  = LotrCharacterProviderPage2.Load( characterURLPage2 )

type LotrCharacterProviderPage3 = HtmlProvider< characterURLPage3 >
let lotrCharacterProviderPage3  = LotrCharacterProviderPage3.Load( characterURLPage3 )

type LotrCharacterProviderPage4 = HtmlProvider< characterURLPage4 >
let lotrCharacterProviderPage4  = LotrCharacterProviderPage4.Load( characterURLPage4 )

type LotrCharacterProviderPage5 = HtmlProvider< characterURLPage5 >
let lotrCharacterProviderPage5  = LotrCharacterProviderPage5.Load( characterURLPage5 )

let getListOfListOfLinks ( lists : HtmlNode list ) : seq< seq< CharacterInfo >> = 
    lists
    |> Seq.map( fun l -> 
        ( l.Descendants[ "a" ]  )
        |> Seq.map( fun ll -> 
            let characterName = ll.TryGetAttribute("title").Value.Value()
            let url           = baseWikiURL + ll.TryGetAttribute("href").Value.Value() 
            createBaseCharacterInfoFromData characterName url  ))

let validCharacterLists =
    [
        // Page 1
        lotrCharacterProviderPage1.Lists.A.Html;
        lotrCharacterProviderPage1.Lists.``A cont.``.Html;
        lotrCharacterProviderPage1.Lists.B.Html;
        lotrCharacterProviderPage1.Lists.``B cont.``.Html;

        // Page 2
        lotrCharacterProviderPage2.Lists.B.Html;
        lotrCharacterProviderPage2.Lists.C2.Html;
        lotrCharacterProviderPage2.Lists.``C cont. 2``.Html;
        lotrCharacterProviderPage2.Lists.D.Html;
        lotrCharacterProviderPage2.Lists.E.Html;
        lotrCharacterProviderPage2.Lists.``E cont.``.Html;
        lotrCharacterProviderPage2.Lists.F.Html;

        // Page 3
        lotrCharacterProviderPage3.Lists.F.Html;
        lotrCharacterProviderPage3.Lists.G.Html;
        lotrCharacterProviderPage3.Lists.``G cont.``.Html;
        lotrCharacterProviderPage3.Lists.H.Html;
        lotrCharacterProviderPage3.Lists.``H cont.``.Html;
        lotrCharacterProviderPage3.Lists.I.Html;
        // Missing K on Page 3

        // Page 4
        // Missing K on Page 4
        lotrCharacterProviderPage4.Lists.L.Html;
        lotrCharacterProviderPage4.Lists.M2.Html;
        lotrCharacterProviderPage4.Lists.``M cont.``.Html;
        lotrCharacterProviderPage4.Lists.N.Html;
        lotrCharacterProviderPage4.Lists.O.Html;
        lotrCharacterProviderPage4.Lists.P2.Html;
        lotrCharacterProviderPage4.Lists.R.Html;
        lotrCharacterProviderPage4.Lists.``R cont.``.Html;
        lotrCharacterProviderPage4.Lists.S2.Html;
        lotrCharacterProviderPage4.Lists.T2.Html;

        // Page 5
        lotrCharacterProviderPage5.Lists.T2.Html;
        lotrCharacterProviderPage5.Lists.``T cont.``.Html;
        lotrCharacterProviderPage5.Lists.U.Html;
        lotrCharacterProviderPage5.Lists.V2.Html;
        lotrCharacterProviderPage5.Lists.W.Html;
        lotrCharacterProviderPage5.Lists.Y.Html;
        // Missing Z on Page 5
        // Missing Æ on Page 5
        lotrCharacterProviderPage5.Lists.É.Html;
        lotrCharacterProviderPage5.Lists.Í.Html;
        // Missing Ó on Page 5
        lotrCharacterProviderPage5.Lists.Ó.Html;
    ]

let incompleteCharacters = Seq.concat ( getListOfListOfLinks ( validCharacterLists ))

let grab2 = incompleteCharacters |> Seq.take 2

let getRaceFromURL( c : CharacterInfo ) : CharacterInfo =
        try
            let getHtmlDocAsync = async { return! HtmlDocument.AsyncLoad( c.Url )}

            let doc = getHtmlDocAsync |> Async.RunSynchronously

            let isHuman : bool = 
                let themeIsMan             = doc.CssSelect( "aside.pi-theme-Men" ) 
                let themeIsManFromGondor   = doc.CssSelect( "aside.pi-theme-Men-Gondor" ) 
                let themeIsManFromRohan    = doc.CssSelect( "aside.pi-theme-Men-Rohan" )
                let themeIsManFromLaketown = doc.CssSelect( "aside.pi-theme-Men-of-Dale-Laketown" ) 

                printfn "Man: %A Gondor: %A Rohan: %A" themeIsMan.IsEmpty themeIsManFromGondor.IsEmpty themeIsManFromRohan.IsEmpty

                not ( themeIsMan.IsEmpty && themeIsManFromGondor.IsEmpty && themeIsManFromRohan.IsEmpty )

            let hobbitRace = doc.CssSelect("a[title|=Hobbit]")
            let isHobbit = hobbitRace.Length > 0

            let elfRace    = doc.CssSelect("a[title|=Elves]")
            let isElf = elfRace.Length > 0

            let dwarfRace = doc.CssSelect("a[title|=Dwarves]")
            let isDwarf = dwarfRace.Length > 0

            let getRace() = 
                if   isHuman  then Human 
                elif isHobbit then Hobbit
                elif isElf    then Elf 
                elif isDwarf  then Dwarf 
                else NotFound

            let race = getRace()

            printfn "Processed: Name: %A; Url: %A Race: %A" c.Name c.Url race
            { c with Race = race }
        // In the case of an exception - just simply skip to the next character. 
        with
           | :? System.Exception -> 
            printfn "Skipping: Name: %A; Url: %A" c.Name c.Url 
            c

grab2 |> Seq.map( getRaceFromURL )

let listOfCompleteCharacters : seq< CharacterInfo > = 
    incompleteCharacters |> Seq.map( getRaceFromURL )

listOfCompleteCharacters |> Seq.take 2 
