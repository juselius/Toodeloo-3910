namespace Toodeloo.Tests

open System
open Expecto
open FsCheck

module Simple =
    let config = { FsCheckConfig.defaultConfig with maxTest = 10000 }

    [<Tests>]
    let properties =
      testList "FsCheck samples" [
        testProperty "Addition is commutative" <| fun a b ->
          a + b = b + a

        testProperty "Reverse of reverse of a list is the original list" <|
          fun (xs : int list) -> List.rev (List.rev xs) = xs

        // you can also override the FsCheck config
        testPropertyWithConfig config "Product is distributive over addition" <|
          fun a b c ->
            a * (b + c) = a * b + a * c
      ]


module UserDefined =
    type User = {
        Id : int
        FirstName : string
        LastName : string
    }

    type UserGen() =
       static member User() : Arbitrary<User> =
        //    let genFirsName = Gen.elements ["Don"; "Henrik"; null]
        //    let genLastName = Gen.elements ["Syme"; "Feldt"; null]
           let genFirsName = Gen.elements ["Don"; "Henrik"]
           let genLastName = Gen.elements ["Syme"; "Feldt"]
           let createUser id firstName lastName =
               {Id = id; FirstName = firstName ; LastName = lastName}
           let getId = Gen.choose(0,1000)
           let genUser =
               createUser <!> getId <*> genFirsName <*> genLastName
           genUser |> Arb.fromGen

    let config = { 
        FsCheckConfig.defaultConfig with 
            arbitrary = [typeof<UserGen>] 
        }

    [<Tests>]
    let properties =
      testList "FsCheck samples" [

        // you can also override the FsCheck config
        testPropertyWithConfig config "User with generated User data" <|
          fun x ->
            Expect.isNotNull x.FirstName "First Name should not be null"
      ]

module MyGen =
    type Float01 = Float01 of float

    let float01Arb =
        let maxValue = float UInt64.MaxValue
        Arb.convert
            (fun (DoNotSize a) -> float a / maxValue |> Float01)
            (fun (Float01 f) -> f * maxValue + 0.5 |> uint64 |> DoNotSize)
            Arb.from
    
    type 'a ListOf100 = ListOf100 of 'a list
    
    let listOf100Arb() =
        Gen.listOfLength 100 Arb.generate
        |> Arb.fromGen
        |> Arb.convert ListOf100 (fun (ListOf100 l) -> l)

    type 'a ListOfAtLeast2 = ListOfAtLeast2 of 'a list

    let listOfAtLeast2Arb() =
        Arb.convert
            (fun (h1,h2,t) -> ListOfAtLeast2 (h1::h2::t))
            (function
                | ListOfAtLeast2 (h1::h2::t) -> h1,h2,t
                | e -> failwithf "not possible in listOfAtLeast2Arb: %A" e)
            Arb.from

    let addToConfig config =
        {config with arbitrary = typeof<Float01>.DeclaringType::config.arbitrary}

[<AutoOpen>]
module Auto =
    let private config = MyGen.addToConfig FsCheckConfig.defaultConfig
    let testProp name = testPropertyWithConfig config name
    let ptestProp name = ptestPropertyWithConfig config name
    let ftestProp name = ftestPropertyWithConfig config name
    let etestProp stdgen name = etestPropertyWithConfig stdgen config name

module PropertyTests =
    open MyGen

    [<Tests>]
    let topicTests =
        testList "topic" [
            testProp "float between 0 and 1" (fun (Float01 f) ->
                () // test
            )
            testProp "list of 100 things" (fun (ListOf100 l) ->
                () // test
            )
            testProp "list of at least 2 things" (fun (ListOfAtLeast2 l) ->
                () // test
            )
            testProp "list of at least 2 things without gen" (fun h1 h2 t ->
                let l = h1::h2::t
                () // test
            )
        ]