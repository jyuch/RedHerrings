Imports Xunit

Public Class RegexTests

    <Fact>
    Public Sub Regex_matched_pattern()
        Dim p = Parse.Regex("[1-9]+")
        Dim expectedResult = "123456789"
        Dim expectedRemainder = New Input("123456789", 9)
        Dim actual = p.TryParse("123456789")

        Assert.True(actual.WasSuccessful)
        Assert.Equal(expectedResult, actual.Value)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

    <Fact>
    Public Sub Regex_not_matched_pattern()
        Dim p = Parse.Regex("[1-9]+")
        Dim expectedRemainder = New Input("0123456789")
        Dim actual = p.TryParse("0123456789")

        Assert.False(actual.WasSuccessful)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

End Class
