namespace Toodeloo.Tests
open Toodleloo.Tests

module Main =
    open Expecto

    [<EntryPoint>]
    let main argv =
        UITests.testClient ()
        Tests.runTestsInAssembly defaultConfig argv

