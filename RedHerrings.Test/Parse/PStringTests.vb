Imports Xunit

Public Class PStringTests

    <Fact>
    Public Sub PString_can_consume_input_text_and_return_string()
        Dim p = Parse.PString("Hello")

        Dim expectedResult = "Hello"
        Dim expectedRemain = New Input("Hello World", 5)

        Dim result = p.TryParse("Hello World")
        Dim remain = result.Remainder

        Assert.Equal(expectedResult, result.Value)
        Assert.Equal(expectedRemain, remain)
    End Sub

    <Fact>
    Public sub PString_return_failure_when_passed_unmatch_pattern()
        Dim p = Parse.PString("Hoge")

        Dim expectedRemain = New Input("Hello World")

        Dim result = p.TryParse("Hello World")
        Dim remain = result.Remainder

        Assert.False(result.WasSuccessful)
        Assert.Equal(expectedRemain, remain)
    End sub

End Class
