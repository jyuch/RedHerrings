Imports System.Runtime.CompilerServices
Imports RedHerrings.Internal

Namespace Global.RedHerrings

    Public Interface IInput
        Inherits IEquatable(Of IInput)
        ReadOnly Property Source As String
        ReadOnly Property AtEnd As Boolean
        ReadOnly Property Position As Integer
    End Interface

    Public Class Input
        Implements IInput

        Private ReadOnly _source As String
        Private ReadOnly _position As Integer

        Public Sub New(source As String)
            Me.New(source, 0)
        End Sub

        Public Sub New(source As String, position As Integer)
            _source = source
            _position = position
        End Sub

        Public ReadOnly Property Source As String Implements IInput.Source
            Get
                Return _source
            End Get
        End Property

        Public ReadOnly Property AtEnd As Boolean Implements IInput.AtEnd
            Get
                Return _source.Length = _position
            End Get
        End Property

        Public ReadOnly Property Position As Integer Implements IInput.Position
            Get
                Return _position
            End Get
        End Property

        Public Overrides Function GetHashCode() As Integer
            Return _source.GetHashCode() Xor _position
        End Function

        Public Overrides Function ToString() As String
            Return _position.ToString()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Return Equals(TryCast(obj, IInput))
        End Function

        Public Overloads Function Equals(other As IInput) As Boolean Implements IEquatable(Of IInput).Equals
            If other Is Nothing Then Return False
            If other Is Me Then Return True
            If _source = other.Source AndAlso _position = other.Position Then Return True
            Return False
        End Function

        Public Shared Operator =(left As Input, rigth As Input) As Boolean
            Return Equals(left, rigth)
        End Operator

        Public Shared Operator <>(left As Input, rigth As Input) As Boolean
            Return Not Equals(left, rigth)
        End Operator

    End Class

    Public Interface IResult(Of Out T)
        ReadOnly Property Value As T
        ReadOnly Property WasSuccessful As Boolean
        ReadOnly Property Remainder As IInput
    End Interface

    Friend Class Result(Of T)
        Implements IResult(Of T)

        Private ReadOnly _value As T
        Private ReadOnly _remainder As IInput
        Private ReadOnly _wasSuccessful As Boolean

        Public Sub New(value As T, remainder As IInput)
            _value = value
            _remainder = remainder
            _wasSuccessful = True
        End Sub

        Public Sub New(remainder As IInput)
            _value = Nothing
            _remainder = remainder
            _wasSuccessful = False
        End Sub

        Public ReadOnly Property Value As T Implements IResult(Of T).Value
            Get
                Return _value
            End Get
        End Property

        Public ReadOnly Property WasSuccessful As Boolean Implements IResult(Of T).WasSuccessful
            Get
                Return _wasSuccessful
            End Get
        End Property

        Public ReadOnly Property Remainder As IInput Implements IResult(Of T).Remainder
            Get
                Return _remainder
            End Get
        End Property

        Public Overrides Function ToString() As String
            If _wasSuccessful Then
                Return String.Format("true, {0}", _value)
            Else
                Return String.Format("false, {0}", _value)
            End If
        End Function
    End Class

    Public Class Result
        Public Shared Function Success(Of T)(value As T, remainder As IInput) As IResult(Of T)
            Return New Result(Of T)(value, remainder)
        End Function

        Public Shared Function Failure(Of T)(remainder As IInput) As IResult(Of T)
            Return New Result(Of T)(remainder)
        End Function
    End Class

    Public Delegate Function Parser(Of T)(input As IInput) As IResult(Of T)

    Public Module ParseModule
        <Extension>
        Public Function [Then](Of T, U)(first As Parser(Of T), second As Func(Of T, Parser(Of U))) As Parser(Of U)
            Return Function(i) first(i).IfSuccess(Function(s) second(s.Value)(s.Remainder))
        End Function
    End Module

    Public Class Parse
        Public Shared Function Text(t As String) As Parser(Of String)
            Return Function(input As IInput) As IResult(Of String)
                       If input.AtEnd Then
                           Return Result.Failure(Of String)(input)
                       End If

                       Dim x = StringHelper.Substring(input.Source, input.Position, t.Length)
                       If x = t Then
                           Return Result.Success(x, New Input(input.Source, input.Position + t.Length))
                       Else
                           Return Result.Failure(Of String)(input)
                       End If
                   End Function
        End Function
    End Class

End Namespace

Namespace Global.RedHerrings.Internal

    Friend Module ResultHelper
        <Extension>
        Public Function IfSuccess(Of T, U)(this As IResult(Of T), [next] As Func(Of IResult(Of T), IResult(Of U))) As IResult(Of U)
            If this.WasSuccessful Then
                Return [next](this)
            End If

            Return Result.Failure(Of U)(this.Remainder)
        End Function

        <Extension>
        Public Function IfFailure(Of T)(this As IResult(Of T), [next] As Func(Of IResult(Of T), IResult(Of T))) As IResult(Of T)
            Return If(this.WasSuccessful, this, [next](this))
        End Function
    End Module

    Friend Class StringHelper
        Friend Shared Function Substring(text As String, position As Integer, length As Integer) As String
            If text.Length > position + length Then
                Return text.Substring(position, length)
            ElseIf text.Length > position Then
                Return text.Substring(position)
            Else
                Return ""
            End If
        End Function
    End Class

End Namespace
