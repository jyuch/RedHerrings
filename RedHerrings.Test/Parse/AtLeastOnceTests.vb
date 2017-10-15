Imports Xunit

Public Class AtLeastOnceTests

    <Fact>
    Public Sub AtLeastOnce_return_failture_when_passed_not_matched_pattern()
        Dim p = Parse.PString("Hoge").AtLeastOnce()
        Dim expectedRemainder = New Input("Hello World")
        Dim actual = p.TryParse("Hello World")

        Assert.False(actual.WasSuccessful)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

    <Fact>
    Public Sub AtLeastOnce_return_one_count_result()
        Dim p = Parse.PString("Hoge").AtLeastOnce()
        Dim expectedResult As IEnumerable(Of String) = {"Hoge"}
        Dim expectedRemainder = New Input("HogeFuga", 4)
        Dim actual = p.TryParse("HogeFuga")

        Assert.Equal(expectedResult, actual.Value)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

    <Fact>
    Public Sub AtLeastOnce_return_two_count_result()
        Dim p = Parse.PString("Hoge").AtLeastOnce()
        Dim expectedResult As IEnumerable(Of String) = {"Hoge", "Hoge"}
        Dim expectedRemainder = New Input("HogeHogeFuga", 8)
        Dim actual = p.TryParse("HogeHogeFuga")

        Assert.Equal(expectedResult, actual.Value)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

End Class
