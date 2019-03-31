module Tests

open System
open Expecto
open FsCheck

[<Tests>]
let tests =
  testList "samples" [
    testCase "universe exists (╭ರᴥ•́)" <| fun _ ->
      let subject = true
      Expect.isTrue subject "I compute, therefore I am."

    testCase "when true is not (should fail)" <| fun _ ->
      let subject = false
      Expect.isTrue subject "I should fail because the subject is false"

    testCase "I'm skipped (should skip)" <| fun _ ->
      Tests.skiptest "Yup, waiting for a sunny day..."

    testCase "I'm always fail (should fail)" <| fun _ ->
      Tests.failtest "This was expected..."

    testCase "contains things" <| fun _ ->
      Expect.containsAll [| 2; 3; 4 |] [| 2; 4 |]
                         "This is the case; {2,3,4} contains {2,4}"

    testCase "contains things (should fail)" <| fun _ ->
      Expect.containsAll [| 2; 3; 4 |] [| 2; 4; 1 |]
                         "Expecting we have one (1) in there"

    testCase "Sometimes I want to ༼ノಠل͟ಠ༽ノ ︵ ┻━┻" <| fun _ ->
      Expect.equal "abcdëf" "abcdef" "These should equal"

    test "I am (should fail)" {
      "╰〳 ಠ 益 ಠೃ 〵╯" |> Expect.equal true false
    }
  ]

let config = { FsCheckConfig.defaultConfig with maxTest = 10000 }

let properties =
  testList "FsCheck samples" [
    testProperty "Addition is commutative" <| fun a b ->
      a + b = b + a

    testProperty "Reverse of reverse of a list is the original list" <|
      fun (xs:list<int>) -> List.rev (List.rev xs) = xs

    // you can also override the FsCheck config
    testPropertyWithConfig config "Product is distributive over addition" <|
      fun a b c ->
        a * (b + c) = a * b + a * c
  ]

Tests.runTests defaultConfig properties

open FsCheck

type User = {
    Id : int
    FirstName : string
    LastName : string
}

type UserGen() =
   static member User() : Arbitrary<User> =
       let genFirsName = Gen.elements ["Don"; "Henrik"; null]
       let genLastName = Gen.elements ["Syme"; "Feldt"; null]
       let createUser id firstName lastName =
           {Id = id; FirstName = firstName ; LastName = lastName}
       let getId = Gen.choose(0,1000)
       let genUser =
           createUser <!> getId <*> genFirsName <*> genLastName
       genUser |> Arb.fromGen

let config' = { FsCheckConfig.defaultConfig with arbitrary = [typeof<UserGen>] }

let properties' =
  testList "FsCheck samples" [

    // you can also override the FsCheck config
    testPropertyWithConfig config "User with generated User data" <|
      fun x ->
        Expect.isNotNull x.FirstName "First Name should not be null"
  ]

Tests.runTests defaultConfig properties

module Gen =
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
    let private config = Gen.addToConfig FsCheckConfig.defaultConfig
    let testProp name = testPropertyWithConfig config name
    let ptestProp name = ptestPropertyWithConfig config name
    let ftestProp name = ftestPropertyWithConfig config name
    let etestProp stdgen name = etestPropertyWithConfig stdgen config name

module Tests =
    let topicTests =
        testList "topic" [
            testProp "float between 0 and 1" (fun (Gen.Float01 f) ->
                () // test
            )
            testProp "list of 100 things" (fun (Gen.ListOf100 l) ->
                () // test
            )
            testProp "list of at least 2 things" (fun (Gen.ListOfAtLeast2 l) ->
                () // test
            )
            testProp "list of at least 2 things without gen" (fun h1 h2 t ->
                let l = h1::h2::t
                () // test
            )
        ]