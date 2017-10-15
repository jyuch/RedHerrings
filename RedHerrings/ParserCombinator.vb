'﻿The MIT License (MIT)
'
'Copyright (c) 2017 jyuch
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in
'all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
'THE SOFTWARE.

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports RedHerrings.Internal

Namespace Global.RedHerrings

    Public Class ParseException
        Inherits Exception
    End Class

    Public Interface IInput
        Inherits IEquatable(Of IInput)
        ReadOnly Property Source As String
        ReadOnly Property AtEnd As Boolean
        ReadOnly Property Position As Integer
        Function Advance() As IInput
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

        Public Function Advance() As IInput Implements IInput.Advance
            If AtEnd Then Throw New InvalidOperationException("The input is already at the end of the source.")
            Return New Input(Source, Position + 1)
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

        <Extension>
        Public Function [Select](Of T, U)(parser As Parser(Of T), convert As Func(Of T, U)) As Parser(Of U)
            If parser Is Nothing Then Throw New ArgumentNullException("parser")
            If convert Is Nothing Then Throw New ArgumentNullException("convert")

            Return parser.Then(Function(it) Parse.[Return](convert(it)))
        End Function

        <Extension>
        Public Function SelectMany(Of T, U, V)(parser As Parser(Of T),
                                               selecter As Func(Of T, Parser(Of U)),
                                               projecter As Func(Of T, U, V)) As Parser(Of V)
            Return parser.Then(Function(it) selecter(it).Select(Function(ut) projecter(it, ut)))
        End Function

        <Extension>
        Public Function [Or](Of T)(first As Parser(Of T), second As Parser(Of T)) As Parser(Of T)
            If first Is Nothing Then Throw New ArgumentNullException("")
            If second Is Nothing Then Throw New ArgumentNullException("")

            Return Function(it As IInput) As IResult(Of T)
                       Dim fr = first(it)

                       If Not fr.WasSuccessful Then
                           Return second(it)
                       End If

                       Return fr
                   End Function
        End Function

        <Extension>
        Public Function Once(Of T)(parser As Parser(Of T)) As Parser(Of IEnumerable(Of T))
            Throw New NotImplementedException()
        End Function

        <Extension>
        Public Function Meny(Of T)(parser As Parser(Of T)) As Parser(Of IEnumerable(Of T))
            Throw New NotImplementedException()
        End Function

        <Extension>
        Public Function AtLeastOnce(Of T)(parser As Parser(Of T)) As Parser(Of IEnumerable(Of T))
            Throw New NotImplementedException()
        End Function
    End Module

    Public Class Parse
        Public Shared Function PString(t As String) As Parser(Of String)
            Return Function(input As IInput) As IResult(Of String)
                       If input.AtEnd Then
                           Return Result.Failure(Of String)(input)
                       End If

                       Dim x = StringHelper.Substring(input.Source, input.Position, t.Length)
                       If x = t Then
                           Dim remainder = input
                           For i = 1 To t.Length
                               remainder = remainder.Advance()
                           Next
                           Return Result.Success(x, remainder)
                       Else
                           Return Result.Failure(Of String)(input)
                       End If
                   End Function
        End Function

        Public Shared Function [Return](Of T)(value As T) As Parser(Of T)
            Return Function(it) Result.Success(value, it)
        End Function

        Public Shared Function Regex(r As String) As Parser(Of String)
            Return Regex(New Regex(r))
        End Function

        Public Shared Function Regex(r As Regex) As Parser(Of String)
            Return RegexMatch(r).Then(Function(it) [Return](it.Value))
        End Function

        Public Shared Function RegexMatch(r As String) As Parser(Of Match)
            If r Is Nothing Then Throw New ArgumentNullException("r")
            Return RegexMatch(r)
        End Function

        Public Shared Function RegexMatch(r As Regex) As Parser(Of Match)
            If r Is Nothing Then Throw New ArgumentNullException("r")

            r = OptimizeRegex(r)

            Return Function(i As IInput) As IResult(Of Match)
                       If Not i.AtEnd Then
                           Dim input = i.Source.Substring(i.Position)
                           Dim match = r.Match(input)
                           Dim remainder = i

                           If match.Success Then
                               For j = 1 To match.Length
                                   remainder = remainder.Advance()
                               Next
                               Return Result.Success(match, remainder)
                           Else
                               Return Result.Failure(Of Match)(i)
                           End If
                       Else
                           Return Result.Failure(Of Match)(i)
                       End If
                   End Function
        End Function

        Private Shared Function OptimizeRegex(r As Regex) As Regex
            Return New Regex(String.Format("^(?:{0})", r), r.Options)
        End Function
    End Class

    Public Module ParserExtensions
        <Extension>
        Public Function TryParse(Of T)(parser As Parser(Of T), input As String) As IResult(Of T)
            If parser Is Nothing Then Throw New ArgumentNullException("parser")
            If input Is Nothing Then Throw New ArgumentNullException("input")

            Return parser(New Input(input))
        End Function

        <Extension>
        Public Function Parse(Of T)(parser As Parser(Of T), input As String) As T
            If parser Is Nothing Then Throw New ArgumentNullException("parser")
            If input Is Nothing Then Throw New ArgumentNullException("input")

            Dim result = TryParse(parser, input)

            If result.WasSuccessful Then
                Return result.Value
            End If

            Throw New ParseException()
        End Function
    End Module

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
