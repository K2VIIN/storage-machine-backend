/// Data access implementation of the Stock component.
module StorageMachine.Stock.Stock

open System.Runtime.InteropServices.JavaScript
open StorageMachine
open Bin
open Stock
open StorageMachine.SimulatedDatabase

/// Data access operations of the Stock component implemented using the simulated in-memory DB.
let stockPersistence =
    { new IStockDataAccess with

        member this.RetrieveAllBins() =
            SimulatedDatabase.retrieveBins ()
            |> Set.map (fun binIdentifier ->
                { Identifier = binIdentifier
                  Content = SimulatedDatabase.retrieveStock () |> Map.tryFind binIdentifier })
            |> Set.toList

        member this.newBin(bin) =
            let answer = SimulatedDatabase.storeBin bin

            match answer with
            | Ok _ -> Some(bin)
            | Error _ -> None

    }
