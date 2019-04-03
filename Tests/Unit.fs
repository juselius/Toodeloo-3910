namespace Toodeloo.Tests
open Shared

module UnitTests =

  open System
  open Expecto
  open Toodeloo.API

  [<Tests>]
  let tests =
    testList "example success" [
      testCase "universe exists" <| fun _ ->
        let subject = true
        Expect.isTrue subject "I compute, therefore I am."

      testCase "I'm skipped (should skip)" <| fun _ ->
        Tests.skiptest "Yup, waiting for a sunny day..."

      testCase "contains things" <| fun _ ->
        Expect.containsAll [| 2; 3; 4 |] [| 2; 4 |]
          "This is the case; {2,3,4} contains {2,4}"

    ]

  // [<Tests>]
  let failures =
    testList "example failure" [
      testCase "when true is not (should fail)" <| fun _ ->
        let subject = false
        Expect.isTrue subject "I should fail because the subject is false"

      testCase "I'm always fail (should fail)" <| fun _ ->
        Tests.failtest "This was expected..."

      testCase "contains things (should fail)" <| fun _ ->
        Expect.containsAll [| 2; 3; 4 |] [| 2; 4; 1 |]
         "Expecting we have one (1) in there"

      testCase "UTF" <| fun _ ->
        Expect.equal "abcdëf" "abcdef" "These should equal"

      test "I am (should fail)" {
        "computation expression" |> Expect.equal true false
      }
    ]

  [<Tests>]
  let serverTests =
    testList "API tests" [
      testCase "add todo" <| fun _ ->
        let result = addTodo Shared.defaultTodo
        Expect.isOk result "Add Ok"

      testCase "load todos" <| fun _ ->
        let result = getTodos ()
        Expect.isOk result "Get Ok"
        match result with 
        | Ok x -> 
          let n = Seq.length x 
          Expect.isTrue (n > 0) "Todo list has n > 0 elements"
        | Error _ -> Tests.failtest "This should not be possible!"
    ]