Imports Xunit

Public Class EndTests

    <Fact>
    Public Sub End_success_when_reach_end_of_source()
        Dim p = Parse.PString("Hello").End()
        Dim actual = p.TryParse("Hello")
        Assert.True(actual.WasSuccessful)
    End Sub

    <Fact>
    Public Sub End_failure_when__not_reach_end_of_source()
        Dim p = Parse.PString("Hello").End()
        Dim actual = p.TryParse("Hello World")
        Assert.False(actual.WasSuccessful)
    End Sub

End Class
