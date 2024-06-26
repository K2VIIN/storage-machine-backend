﻿/// This module exposes use-cases of the Stock component as an HTTP Web service using Giraffe.
module StorageMachine.Stock.Stock

open Giraffe
open Microsoft.AspNetCore.Http
open Thoth.Json.Giraffe
open Stock

/// An overview of all bins currently stored in the Storage Machine.
let binOverview (next: HttpFunc) (ctx: HttpContext) =
    task {
        let dataAccess = ctx.GetService<IStockDataAccess>()
        let bins = Stock.binOverview dataAccess
        return! ThothSerializer.RespondJsonSeq bins Serialization.encoderBin next ctx
    }

/// An overview of actual stock currently stored in the Storage Machine. Actual stock is defined as all non-empty bins.
let stockOverview (next: HttpFunc) (ctx: HttpContext) =
    task {
        let dataAccess = ctx.GetService<IStockDataAccess>()
        let bins = Stock.stockOverview dataAccess
        return! ThothSerializer.RespondJsonSeq bins Serialization.encoderBin next ctx
    }

/// An overview of all products stored in the Storage Machine, regardless what bins contain them.
let productsInStock (next: HttpFunc) (ctx: HttpContext) =
    task {
        let dataAccess = ctx.GetService<IStockDataAccess>()
        let productsOverview = Stock.productsInStock dataAccess
        return! ThothSerializer.RespondJson productsOverview Serialization.encoderProductsOverview next ctx
    }

/// KATJA COMMENT: Dit is een nieuwe functie die aangemaakt is om een nieuwe Bin in de 'database' te zetten
/// zeg maar de eerste line is copy paste. Tweede line is het lezen van de POST request, nou zie comment lol
/// Daarna omdat binData een Result<> teruggeeft kan het OK of ERROR zijn. Afhankelijk van welke ga je de goede path in
/// Zoals je kan zien gaan we bij een OK het echt in de database zetten. Dit geeft ook weer iets zoals een Result<> terug,
/// maar deze keer een Option<> wat basically hetzelfde is maar andere woorden.
let newBin (next: HttpFunc) (ctx: HttpContext) =
    task {
        let dataAccess = ctx.GetService<IStockDataAccess>()
        let! binData = ThothSerializer.ReadBody ctx Serialization.decodeBin // Read the data from the raw body JSON

        match binData with
        | Ok bin ->
            let newBin = Stock.newBin dataAccess bin

            match newBin with
            | Some newBin -> return! ThothSerializer.RespondJson newBin Serialization.encoderBin next ctx
            | None -> return! RequestErrors.badRequest (text "POST sad :( - 1") earlyReturn ctx
        | Error _ -> return! RequestErrors.badRequest (text "POST sad :( - 2") earlyReturn ctx
    }

/// Defines URLs for functionality of the Stock component and dispatches HTTP requests to those URLs.
let handlers: HttpHandler =
    choose
        [ GET >=> route "/bins" >=> binOverview
          GET >=> route "/stock" >=> stockOverview
          GET >=> route "/stock/products" >=> productsInStock
          POST >=> route "/bin" >=> newBin ]
