Imports Xunit

Public Class InputTests

    <Fact>
    Public Sub Equals_returns_true_when_passed_same_value_object()
        Dim obj1 = New Input("Hello", 0)
        Dim obj2 = New Input("Hello", 0)

        Assert.Equal(True, obj1.Equals(obj2))
    End Sub

    <Fact>
    Public Sub Equals_returns_false_when_passed_not_same_value_object()
        Dim obj1 = New Input("Hello", 0)
        Dim obj2 = New Input("World", 0)

        Assert.Equal(False, obj1.Equals(obj2))
    End Sub

    <Fact>
    Public Sub operator_equals_returns_true_when_passed_same_value_object()
        Dim obj1 = New Input("Hello", 0)
        Dim obj2 = New Input("Hello", 0)

        Assert.Equal(True, obj1 = obj2)
    End Sub

    <Fact>
    Public Sub GetHashCode_returns_same_value_when_passed_same_value_object()
        Dim obj1 = New Input("Hello", 0)
        Dim obj2 = New Input("Hello", 0)

        Assert.Equal(obj1.GetHashCode(), obj2.GetHashCode())
    End Sub

End Class
