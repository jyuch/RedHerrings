Imports Xunit

Public Class SelectManyTests

    <Fact>
    Public Sub SelectMany_can_use_query()
        Dim p = From a In Parse.PString("Hello")
                From b In Parse.PString(" ")
                From c In Parse.PString("World")
                Select Tuple.Create(a, c)
        Dim expected = Tuple.Create("Hello", "World")
        Dim actual = p.Parse("Hello World")

        Assert.Equal(expected, actual)
    End Sub

End Class
