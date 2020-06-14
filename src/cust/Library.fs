namespace ClusterSharding.Node
open System
open System.Collections.Generic
open System.Text
open Akka.Persistence
open Akka.Cluster.Sharding
open Akka.Actor

type ShardEnvelope (entityId:string, payload:obj) =
    member val EntityId = entityId with get, set
    member val Payload = payload with get, set

type MessageExtractor (i:int) =
    inherit HashCodeMessageExtractor(i)

    override this.EntityId(message:obj) = 
        (message :?> ShardEnvelope).EntityId

    override this.EntityMessage(message:obj) = 
        (message :?> ShardEnvelope).Payload

type PurchaseItem () =
    member val ItemName = Unchecked.defaultof<string> with get, set
    member this.PurchaseItem(itemName:string) =
        this.ItemName <- itemName
    new (itm:string) as this =
        PurchaseItem ()
        then
            this.PurchaseItem(itm)


type ItemPurchased () =
    member val ItemName = Unchecked.defaultof<string> with get, set
    member this.ItemPurchased(itemName:string) =
        this.ItemName <- itemName                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
    new (itm:string) as this =
        ItemPurchased ()
        then
            this.ItemPurchased(itm)

type Customer (trivialFlag:bool) = 
    inherit ReceivePersistentActor ()
    let mutable purchasedItems = new List<string>() :> ICollection<string>

    override val PersistenceId = ReceivePersistentActor.Context.Parent.Path.Name + "/" + ReceivePersistentActor.Context.Self.Path.Name with get
    member this.self : IActorRef = base.Self
    member this._purchasedItems 
        with get () = purchasedItems
        and set (value) = purchasedItems <- value

    new () as this =
        Customer (false)
        then
            base.SetReceiveTimeout(new Nullable<TimeSpan>(TimeSpan.FromSeconds(60.0)))
            base.Recover<ItemPurchased>(fun (purchased:ItemPurchased) -> this._purchasedItems.Add(purchased.ItemName))
            let act0 = 
                new Action<ItemPurchased>(
                    fun purchased ->
                        this._purchasedItems.Add(purchased.ItemName)
                        let name = Uri.UnescapeDataString(this.self.Path.Name)
                        printfn "%s purchased '%s'.\nAll items: [%s]\n--------------------------" name purchased.ItemName (String.Join(", ", this._purchasedItems))
                    )
            let act = 
                new Action<PurchaseItem>(
                    fun (purchase:PurchaseItem) ->
                        let ip = new ItemPurchased(purchase.ItemName)
                        this.Persist(ip, act0)
                    )
            this.Command<PurchaseItem>(act)        

type orz (trivialFlag:bool) =
    [<DefaultValue>] val mutable i : int

    new () as this =
        orz (false)
        then 
            this.i <- 123
            printfn "123"

//let oo = orz()
//oo.i