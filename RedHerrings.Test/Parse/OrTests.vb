Imports Xunit

Public Class OrTests

    <Fact>
    Public Sub Or_matched_first_parser()
        Dim hello = Parse.PString("hello")
        Dim world = Parse.PString("world")
        Dim p = hello.Or(world)

        Dim expected = "hello"
        Dim actual = p.TryParse("hello")

        Assert.True(actual.WasSuccessful)
        Assert.Equal(expected, actual.Value)
    End Sub

    <Fact>
    Public Sub Or_matched_second_parser()
        Dim hello = Parse.PString("hello")
        Dim world = Parse.PString("world")
        Dim p = hello.Or(world)

        Dim expected = "world"
        Dim actual = p.TryParse("world")

        Assert.True(actual.WasSuccessful)
        Assert.Equal(expected, actual.Value)
    End Sub

    <Fact>
    Public Sub Or_not_matched_first_and_second()
        Dim hello = Parse.PString("hello")
        Dim world = Parse.PString("world")
        Dim p = hello.Or(world)
        
        Dim actual = p.TryParse("hoge")

        Assert.False(actual.WasSuccessful)
    End Sub

End Class
