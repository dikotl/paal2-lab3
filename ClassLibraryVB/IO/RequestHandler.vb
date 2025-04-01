Imports System
Imports System.IO
Imports System.ComponentModel
Imports ClassLibrary.Collections
Imports ClassLibrary.FunctionalEnumerableOperations


Namespace IO
    Public Class ExitToMenuException
        Inherits Exception
    End Class

    Public Class ExitProgramException
        Inherits Exception
    End Class


    Public Enum RequestStyle As Integer
        [Default] = 0
        Inline = 1
        Bare = 2
    End Enum


    Public Class Context
        Public ReadOnly Property Reader As TextReader
        Public ReadOnly Property Writer As TextWriter
        Public ReadOnly Property TalkToUser As Boolean

        Public Sub New(reader As TextReader, writer As TextWriter, talkToUser As Boolean)
            Me.Reader = reader
            Me.Writer = writer
            Me.TalkToUser = talkToUser
        End Sub

        Public Sub Print(message As Object)
            If TalkToUser Then Console.Error.Write(message)
        End Sub

        Public Sub PrintLine(message As Object)
            If TalkToUser Then Console.Error.WriteLine(message)
        End Sub

        Public Sub Write(message As Object)
            If TalkToUser Then Writer.Write(message)
        End Sub

        Public Sub WriteLine(message As Object)
            If TalkToUser Then Writer.WriteLine(message)
        End Sub

        Public Sub [Error](message As String)
            Dim oldColor As ConsoleColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Red

            PrintLine("Error! " & If(message, ""))

            Console.ForegroundColor = oldColor
        End Sub

        Public Function Request(Of T)(converter As Converter(Of String, T), Optional message As String = Nothing, Optional style As RequestStyle = RequestStyle.Default) As T
            While True
                Try
                    Return converter(Request(message, style))
                Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                    [Error](e.Message)
                End Try
            End While
        End Function

        Public Function Request(Of T As {IParsable(Of T), New})(Optional message As String = Nothing, Optional style As RequestStyle = RequestStyle.[Default]) As T
            Return Request(Function(input)
                               Dim method = GetType(T).GetMethod("Parse", {GetType(String), GetType(IFormatProvider)})
                               If method IsNot Nothing Then
                                   Return CType(method.Invoke(Nothing, {input, Nothing}), T)
                               End If
                           End Function, message, style)
        End Function



        Public Function Request(Optional message As String = Nothing, Optional style As RequestStyle = RequestStyle.[Default]) As String
            Select Case style
                Case RequestStyle.[Default]
                    If message IsNot Nothing Then Print(message)
                    Print(vbLf & "> ")
                Case RequestStyle.Inline
                    If message IsNot Nothing Then Print(message)
                    Print(": ")
                Case RequestStyle.Bare
                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(style), CType(style, Integer), GetType(RequestStyle))
            End Select

            Dim input As String = If(Reader.ReadLine(), "")
            Dim trimmed = input.Trim().ToLower()

            If trimmed = "menu" Then Throw New ExitToMenuException()
            If trimmed = "exit" Then Throw New ExitProgramException()

            Return input

        End Function

        Public Function ReadArrayRandom(Of T)(getRandomItem As Func(Of T)) As DynArray(Of T)
            Dim size As Integer = Request(Function(input) SizeInt(input), "Input number of elements")
            Dim generatedArray As DynArray(Of T) = Generator.GetRandomDynArray(New Range(size, size), getRandomItem)

            PrintLine(String.Format("Generated array: {0}", generatedArray))
            Return generatedArray
        End Function

        Public Function ReadArrayInline(Of T)(converter As Converter(Of String, T)) As DynArray(Of T)
            Return Reader _
                .ReadLine() _
                .Split() _
                .Map(Function(input) converter(input)) _
                .ToDynArray()
        End Function

        Private Function ParseInput(Of T As {IParsable(Of T), New})(input As String) As T
            Dim method = GetType(T).GetMethod("Parse", {GetType(String), GetType(IFormatProvider)})
            If method IsNot Nothing Then
                Return CType(method.Invoke(Nothing, {input, Nothing}), T)
            End If
        End Function


        Public Function ReadArrayInline(Of T As {IParsable(Of T), New})() As DynArray(Of T)
            Return Reader _
                .ReadLine() _
                .Split() _
                .Map(Function(input) ParseInput(Of T)(input)) _
                .ToDynArray()
        End Function

        Public Function RequestArray(Of T As {IParsable(Of T), New})(getRandomItem As Func(Of T)) As DynArray(Of T)
            Return RequestArray(Function(input)
                                    Dim method = GetType(T).GetMethod("Parse", {GetType(String), GetType(IFormatProvider)})
                                    If method IsNot Nothing Then
                                        Return CType(method.Invoke(Nothing, {input, Nothing}), T)
                                    End If
                                End Function, getRandomItem)
        End Function


        Public Function RequestArray(Of T)(converter As Converter(Of String, T), getRandomItem As Func(Of T)) As DynArray(Of T)
            Dim isRandom As Boolean = ChooseInputMethod()

            If isRandom Then
                Return ReadArrayRandom(getRandomItem)
            Else
                Do
                    Try
                        Return ReadArrayInline(converter)
                    Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                        Error (e.Message)
                    End Try
                Loop
            End If
        End Function

        Public Function RequestMatrix(Of T As {IParsable(Of T), New})(getRandomItem As Func(Of T)) As DynArray(Of DynArray(Of T))
            Return RequestMatrix(Function(input)
                                     Dim method = GetType(T).GetMethod("Parse", {GetType(String), GetType(IFormatProvider)})
                                     If method IsNot Nothing Then
                                         Return CType(method.Invoke(Nothing, {input, Nothing}), T)
                                     End If
                                 End Function, getRandomItem)
        End Function


        Public Function RequestMatrix(Of T)(converter As Converter(Of String, T), getRandomItem As Func(Of T)) As DynArray(Of DynArray(Of T))
            Dim size As Integer = Request(AddressOf SizeInt, "Input number of sub-arrays")
            Dim isRandomOrWasError As Boolean = ChooseInputMethod()

            Dim result As DynArray(Of DynArray(Of T)) =
                If(isRandomOrWasError, RequestMatrixRandom(size, getRandomItem), RequestMatrixInline(size, converter, isRandomOrWasError))

            If isRandomOrWasError Then
                PrintLine("Gotten matrix:")
                For Each item As DynArray(Of T) In result
                    WriteLine(item)
                Next
            End If

            Return result
        End Function

        Private Function RequestMatrixInline(Of T)(size As Integer, converter As Converter(Of String, T), ByRef show As Boolean) As DynArray(Of DynArray(Of T))
            Dim typed As New DynArray(Of DynArray(Of T))(size)
            show = False

            For i As Integer = 0 To size - 1
                Try
                    typed(i) = ReadArrayInline(converter)
                Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                    Error (e.Message)
                    i -= 1
                    show = True
                End Try
            Next

            Return typed
        End Function

        Private Function RequestMatrixRandom(Of T)(rows As Integer, getRandomItem As Func(Of T)) As DynArray(Of DynArray(Of T))
            Dim maxColumnSize As Integer = Request(AddressOf SizeInt, "Input max number of elements")
            Return New DynArray(Of DynArray(Of T)) _
                (rows, Function() Generator.GetRandomDynArray(New Range(1, maxColumnSize), getRandomItem))
        End Function

        Private Function ChooseInputMethod() As Boolean
            Const message As String =
                "Select input method:" & vbCrLf &
                "    1. Random" & vbCrLf &
                "    2. Line by line"

            Do
                Select Case Request(message).Trim()
                    Case "1" : Return True
                    Case "2" : Return False
                End Select
                Error ("Unknown option")
            Loop
        End Function

        Public Function SizeInt(input As String) As Integer
            Dim size As Integer = Integer.Parse(input)

            If size < 1 Then
                Throw New FormatException($"Value {size} is not in valid range (1..{Integer.MaxValue})")
            End If

            Return size
        End Function


    End Class

End Namespace
