Imports Xunit

Public Class OnceTests

    <Fact>
    Public Sub Once_return_result()
        Dim p = Parse.PString("Hoge").Once()
        Dim expected As IEnumerable(Of String) = {"Hoge"}
        Dim actual = p.Parse("Hoge")

        Assert.Equal(expected, actual)
    End Sub

End Class
