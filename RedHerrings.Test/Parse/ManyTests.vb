Imports Xunit

Public Class ManyTests

    <Fact>
    Public Sub Meny_return_empty_result()
        Dim p = Parse.PString("Hoge").Many()
        Dim expectedResult As IEnumerable(Of String) = {}
        Dim expectedRemainder = New Input("Hello World")
        Dim actual = p.TryParse("Hello World")

        Assert.Equal(expectedResult, actual.Value)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

    Public Sub Meny_return_one_count_result()
        Dim p = Parse.PString("Hoge").Many()
        Dim expectedResult As IEnumerable(Of String) = {"Hoge"}
        Dim expectedRemainder = new Input("HogeFuga", 4)
        Dim actual = p.TryParse("HogeFuga")

        Assert.Equal(expectedResult, actual.Value)
        Assert.Equal(expectedRemainder, actual.Remainder)
    End Sub

End Class
