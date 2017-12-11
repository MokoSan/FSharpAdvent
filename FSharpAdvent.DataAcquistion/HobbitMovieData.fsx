#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"
#load "MovieCommon.fsx"

open System
open System.IO

open FSharp.Data

(* Entire Series - Overall Numbers *)
[<Literal>]
let hobbitFilmSeries              = @"https://en.wikipedia.org/wiki/The_Hobbit_(film_series)"
type HobbitFilmSeriesTypeProvider = HtmlProvider< hobbitFilmSeries >
let hobbitFilmSeriesProvider      = HobbitFilmSeriesTypeProvider.Load( hobbitFilmSeries )

(** Table of Budget, Box Office and Running Time **)
let overallFirstTablesRows = hobbitFilmSeriesProvider.Tables.Table1.Rows

// Running Time
let overallRuntime          = overallFirstTablesRows.[ 12 ].``The Hobbit 2``
let overallRuntimeInMinutes = int( overallRuntime.Split(' ').[ 0 ])

// Budget
let overallBudget                     = overallFirstTablesRows.[ 15 ].``The Hobbit 2``
let overallBudgetInMillions           = int ( overallBudget.Split(' ').[ 0 ].Replace( "$", "" ))

// Box Office Revenue
let overallBoxOfficeRevenue           = overallFirstTablesRows.[ 16 ].``The Hobbit 2``
let overallBoxOfficeRevenueInMillions = float( overallBoxOfficeRevenue.Split(' ').[ 0 ].Replace( "$", "" )) * 1000.

(** List of Academy Award Nominations vs. Wins  **)
// The Unexpected Journey 
let ujAcademyAwardsData  = hobbitFilmSeriesProvider.Lists.``Academy Awards``.Values.[ 0 ]
let ujNominationsAndWins = ujAcademyAwardsData.Split('—').[ 1 ].Split(':')
let ujNominations        = int ( ujNominationsAndWins.[ 1 ].Split(';').[ 0 ].Replace(" ", ""))
let ujWins               = int ( ujNominationsAndWins.[ 1 ].Split(';').[ 1 ].Split(' ').[ 1 ] )
let ujRottenTomatoes     = int ( hobbitFilmSeriesProvider.Tables.``Public and critical response``.Rows.[ 0 ].``Rotten Tomatoes``.Split(' ').[0].Replace("%", ""))

// The Desolation of Smaug
let dsAcademyAwardsData  = hobbitFilmSeriesProvider.Lists.``Academy Awards``.Values.[ 1 ]
let dsNominationsAndWins = dsAcademyAwardsData.Split('—').[ 1 ].Split(':')
let dsNominations        = int ( dsNominationsAndWins.[ 1 ].Split(';').[ 0 ].Replace(" ", ""))
let dsWins               = 0
let dsRottenTomatoes     = int ( hobbitFilmSeriesProvider.Tables.``Public and critical response``.Rows.[ 1 ].``Rotten Tomatoes``.Split(' ').[0].Replace("%", ""))

// The Battle of the Five Armies
let bfaAcademyAwardsData  = hobbitFilmSeriesProvider.Lists.``Academy Awards``.Values.[ 2 ]
let bfaNominationsAndWins = bfaAcademyAwardsData.Split('-').[ 1 ].Split(':')
let bfaNominations        = int ( bfaNominationsAndWins.[ 1 ].Split(';').[ 0 ].Replace(" ", ""))
let bfaWins               = 0
let bfaRottenTomatoes     = int ( hobbitFilmSeriesProvider.Tables.``Public and critical response``.Rows.[ 2 ].``Rotten Tomatoes``.Split(' ').[0].Replace("%", ""))

// Overall 
let overallNominations         = ujNominations    + dsNominations    + bfaNominations
let overallWins                = ujWins           + dsWins           + bfaWins
let overallRottenTomatoesScore = ( float( ujRottenTomatoes + dsRottenTomatoes + bfaRottenTomatoes )) / 3.

let overallMovieInfo = { Name                       = "The Hobbit Series";
                         RuntimeInMinutes           = overallRuntimeInMinutes;
                         BudgetInMillions           = overallBudgetInMillions;
                         BoxOfficeRevenueInMillions = overallBoxOfficeRevenueInMillions;
                         AcademyAwardNominations    = overallNominations;
                         AcademyAwardWins           = overallWins; 
                         RottenTomatoesScore        = overallRottenTomatoesScore; }

(* The Unexpected Journey Specific Data *)

[<Literal>]
let unexpectedJourney          = @"https://en.wikipedia.org/wiki/The_Hobbit:_An_Unexpected_Journey"

type UnexpectedJourneyProvider = HtmlProvider< unexpectedJourney > 
let unexpectedJourneyProvider  = UnexpectedJourneyProvider.Load( unexpectedJourney )

(** Running Time, Budget and Box Office Revenue **)
let ujFirstTableRows   = unexpectedJourneyProvider.Tables.Table1.Rows

let ujRuntime          = ujFirstTableRows.[ 12 ].``The Hobbit: An Unexpected Journey 2``.Split(' ').[ 0 ]
let ujRuntimeInMinutes = int ( ujRuntime )

let ujBudget           = ujFirstTableRows.[ 15 ].``The Hobbit: An Unexpected Journey 2``.Split('-').[ 0 ].Replace("$", "")
let ujBudgetInMillions = int ( ujBudget )

let ujBoxOfficeRevenue           = ujFirstTableRows.[ 16 ].``The Hobbit: An Unexpected Journey 2``.Split(' ').[ 0 ].Replace("$", "")
let ujBoxOfficeRevenueInMillions = float ( ujBoxOfficeRevenue ) * 1000.

let ujMovieInfo =  { Name                       = "The Unexpected Journey";
                     BudgetInMillions           = ujBudgetInMillions;
                     BoxOfficeRevenueInMillions = ujBoxOfficeRevenueInMillions;
                     RuntimeInMinutes           = ujRuntimeInMinutes;
                     AcademyAwardNominations    = ujNominations;
                     AcademyAwardWins           = ujWins; 
                     RottenTomatoesScore        = float( ujRottenTomatoes ); }

(* The Desolation of Smaug Specific Data *)
[<Literal>]
let desolationOfSmaug          = @"https://en.wikipedia.org/wiki/The_Hobbit:_The_Desolation_of_Smaug"

type DesolationOfSmaugProvider = HtmlProvider< desolationOfSmaug > 
let desolationOfSmaugProvider  = DesolationOfSmaugProvider.Load( desolationOfSmaug )

(** Running Time, Budget and Box Office Revenue **)
let dsFirstTableRows   = desolationOfSmaugProvider.Tables.Table1.Rows
let dsRuntimeInMinutes = 161 // Weird the Type Provider didn't have this even though it exists on the Wikipage

let dsBudget           = dsFirstTableRows.[ 15 ].``The Hobbit: The Desolation of Smaug 2``.Split('-').[ 0 ].Replace("$", "").Split(' ').[ 0 ]
let dsBudgetInMillions = int ( dsBudget )

let dsBoxOfficeRevenue           = dsFirstTableRows.[ 16 ].``The Hobbit: The Desolation of Smaug 2``.Split(' ').[ 0 ].Replace("$", "")
let dsBoxOfficeRevenueInMillions = float ( dsBoxOfficeRevenue )

let dsMovieInfo =  { Name                       = "The Desolation of Smaug";
                     BudgetInMillions           = dsBudgetInMillions;
                     BoxOfficeRevenueInMillions = dsBoxOfficeRevenueInMillions;
                     RuntimeInMinutes           = dsRuntimeInMinutes;
                     AcademyAwardNominations    = dsNominations;
                     AcademyAwardWins           = dsWins; 
                     RottenTomatoesScore        = float( dsRottenTomatoes ); }

(* The Battle of the Five Armies Specific Data *)
[<Literal>]
let battleOfFiveArmies = @"https://en.wikipedia.org/wiki/The_Hobbit:_The_Battle_of_the_Five_Armies"

type BattleOfFiveArmiesProvider = HtmlProvider< battleOfFiveArmies > 
let battleOfFiveArmiesProvider  = BattleOfFiveArmiesProvider.Load( battleOfFiveArmies )

(** Running Time, Budget and Box Office Revenue **)
let bfaFirstTableRows   = battleOfFiveArmiesProvider.Tables.Table1.Rows
let bfaRuntimeInMinutes = 144 // Weird the Type Provider didn't have this even though it exists on the Wikipage

let bfaBudget           = bfaFirstTableRows.[ 15 ].``The Hobbit: The Battle of the Five Armies 2``.Replace("$", "").Split(' ').[ 0 ]
let bfaBudgetInMillions = int ( bfaBudget )

let bfaBoxOfficeRevenue           = bfaFirstTableRows.[ 16 ].``The Hobbit: The Battle of the Five Armies 2``.Split(' ').[ 0 ].Replace("$", "")
let bfaBoxOfficeRevenueInMillions = float ( bfaBoxOfficeRevenue )

let bfaMovieInfo =  { Name                       = "The Battle of the Five Armies";
                      BudgetInMillions           = bfaBudgetInMillions;
                      BoxOfficeRevenueInMillions = bfaBoxOfficeRevenueInMillions;
                      RuntimeInMinutes           = bfaRuntimeInMinutes;
                      AcademyAwardNominations    = bfaNominations;
                      AcademyAwardWins           = bfaWins; 
                      RottenTomatoesScore        = float( bfaRottenTomatoes ); }
let allMoviesInfo  =
    [
        overallMovieInfo;
        ujMovieInfo;
        dsMovieInfo;
        bfaMovieInfo
    ]
  
let allMoviesCsv = 
    allMoviesInfo
    |> List.map( MovieInfo.ToCsv )

appendCsvLineToMovieOutput( allMoviesCsv )