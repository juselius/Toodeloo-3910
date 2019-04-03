namespace Toodeloo.Tests
open Toodleloo.Tests

module Main =
    open Expecto

    [<EntryPoint>]
    let main argv =
        let argv' =
            if Array.exists ((=) "--canopy") argv then
                UITests.testClient ()
                Array.filter ((=) "--canopy" >> not) argv
            else argv
        Tests.runTestsInAssembly defaultConfig argv'

