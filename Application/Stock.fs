/// Provides functionality (use-cases) related to stock (mostly bins) stored in the Storage Machine.
module StorageMachine.Stock.Stock

open StorageMachine
open Bin
open Stock

/// Defines data access operations for stock functionality.
type IStockDataAccess =

    /// Retrieve all bins currently stored in the Storage Machine.
    abstract RetrieveAllBins: unit -> List<Bin>

    abstract newBin: Bin -> Option<Bin> // KATJA COMMENT: Nieuwe methode in the interface die ervoor kan zorgen dat zooi in de database kan. Heeft wat te maken dat in Onion structuur moet. Vandaar interface, vraag anders maar keer aan mij voor meer details

/// An overview of all bins currently stored in the Storage Machine.
let binOverview (dataAccess: IStockDataAccess) : List<Bin> =
    // Trivially
    dataAccess.RetrieveAllBins()

/// An overview of actual stock currently stored in the Storage Machine. Actual stock is defined as all non-empty bins.
let stockOverview (dataAccess: IStockDataAccess) : List<Bin> =
    // Perform I/O
    let allBins = dataAccess.RetrieveAllBins()
    // Use the model which provides the definition of a bin being (non-)empty
    let actualStock = allBins |> List.filter Bin.isNotEmpty
    actualStock

/// An overview of all products in stock consists of all unique products and their total quantity stored in the Storage
/// Machine.
type ProductsOverview = Set<Product * Quantity>

/// An overview of all products stored in the Storage Machine, regardless what bins contain them.
let productsInStock (dataAccess: IStockDataAccess) : ProductsOverview =
    let allBins = dataAccess.RetrieveAllBins()
    let products = Stock.allProducts allBins
    products |> Stock.totalQuantity |> Map.toSeq |> Set.ofSeq // KATJA COMMENT: Dit zijn antwoorden van Exercise 0. Dit is gewoon het ophalen uit de 'database' en je wilt een Set returnen. Kan je gewoon door die ingebouwde functies van map naar seq omzetten en daarna terug naar set

let newBin (dataAccess: IStockDataAccess) (bin: Bin) : Option<Bin> = dataAccess.newBin bin
