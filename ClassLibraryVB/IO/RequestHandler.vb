Imports System
Imports System.IO
Imports System.Reflection
Imports System.ComponentModel
Imports ClassLibraryCS.Collections
Imports ClassLibraryCS.FunctionalEnumerableOperations
Imports ClassLibraryFS
Imports ClassLibraryFS.ConsoleUI

Namespace IO
    ''' <summary>
    ''' Exception thrown to indicate the program should exit to the menu.
    ''' </summary>
    Public Class ExitToMenuException
        Inherits Exception
    End Class

    ''' <summary>
    ''' Exception thrown to indicate the program should exit completely.
    ''' </summary>
    Public Class ExitProgramException
        Inherits Exception
    End Class

    ''' <summary>
    ''' Specifies a style for the request message in the Context.Request* methods.
    ''' </summary>
    Public Enum RequestStyle As Integer
        ''' <summary>
        ''' A request message Is placed in the previous line.
        '''
        ''' <code>Request message
        ''' > |
        ''' </code>
        ''' </summary>
        [Default] = 0

        ''' <summary>
        ''' The request message Is placed on the same line.
        '''
        ''' <code>Request message: |</code>
        ''' </summary>
        Inline = 1

        ''' <summary>
        ''' Just a regular <see cref="Console.ReadLine"/>.
        ''' </summary>
        Bare = 2
    End Enum

    ''' <summary>
    ''' Represents the context for program I/O operations. It includes methods to read And write data from/to the user.
    ''' </summary>
    ''' <param name="Reader">The input data stream.</param>
    ''' <param name="Writer">The output data stream.</param>
    ''' <param name="TalkToUser">Indicates whether to communicate with the user through stderr.</param>
    Public Class Context
        Implements IContext
        Public ReadOnly Property Reader As TextReader Implements IContext.Reader
        Public ReadOnly Property Writer As TextWriter Implements IContext.Writer
        Public ReadOnly Property TalkToUser As Boolean Implements IContext.TalkToUser
        Public ReadOnly Property GlobalTheme As Coloring.Theme Implements IContext.GlobalTheme
        Public ReadOnly Property HelpMenu As String Implements IContext.HelpMenu

        Public Sub New(reader As TextReader, writer As TextWriter, talkToUser As Boolean, globalTheme As Coloring.Theme)
            Me.Reader = reader
            Me.Writer = writer
            Me.TalkToUser = talkToUser
            Me.GlobalTheme = globalTheme
            Me.HelpMenu = generateTable(
                "Available Commands",
                {
                    Tuple.Create("menu", "return to the menu"),
                    Tuple.Create("exit", "exit the program"),
                    Tuple.Create("help", "show this table"),
                    Tuple.Create("clear", "clear the console")
                },
                globalTheme)
            PrintLine(Me.HelpMenu)
        End Sub

        ''' <summary>
        ''' Prints a message to the console without a newline, using the specified color.
        ''' If the TalkToUser variable is set to True,
        ''' the message will be written to the standard error stream (Console.Error).
        ''' </summary>
        ''' <param name="message">The message to print.</param>
        ''' <param name="color">(Optional) The text color. Default is white.</param>
        Public Sub Print(message As Object, Optional color As ConsoleColor? = Nothing)
            If color Is Nothing Then color = GlobalTheme.Other
            If TalkToUser Then
                Dim oldColor = Console.ForegroundColor
                Console.ForegroundColor = color
                Console.Error.Write(message)
                Console.ForegroundColor = oldColor
            End If
        End Sub

        ''' <summary>
        ''' Prints a message to the console with the specified color.
        ''' If the TalkToUser variable is set to True,
        ''' the message will be written to the standard error stream (Console.Error).
        ''' </summary>
        ''' <param name="message">The message to print.</param>
        ''' <param name="color">(Optional) The text color. Default is white.</param>
        Public Sub PrintLine(message As Object, Optional color As ConsoleColor? = Nothing)
            If color Is Nothing Then color = GlobalTheme.Other
            If TalkToUser Then
                Dim oldColor = Console.ForegroundColor
                Console.ForegroundColor = color
                Console.Error.WriteLine(message)
                Console.ForegroundColor = oldColor
            End If
        End Sub

        ''' <summary>
        ''' Writes a message to the program output.
        ''' </summary>
        ''' <param name="message">The message to be written.</param>
        Public Sub Write(message As Object)
            If TalkToUser Then Writer.Write(message)
        End Sub

        ''' <summary>
        ''' Writes a message to the program output, appending a New line at the end.
        ''' </summary>
        ''' <param name="message">The message to be written.</param>
        Public Sub WriteLine(message As Object)
            If TalkToUser Then Writer.WriteLine(message)
        End Sub

        ''' <summary>
        ''' Prints an error message to the user, appending a New line at the end.
        ''' </summary>
        ''' <param name="message">The error message to be printed.</param>
        Public Sub [Error](message As String)
            PrintLine("Error! " & If(message, ""), ConsoleColor.DarkRed)
        End Sub

        ''' <summary>
        ''' Handles command...
        ''' </summary>
        ''' <param name="input">An input from the user to be checked</param>
        ''' <returns>True if command was handled</returns>
        Private Function HandleCommand(input As String) As Boolean
            If input Is Nothing Then Throw New ExitProgramException()

            Select Case input.Trim().ToLower()
                Case "menu"
                    Throw New ExitToMenuException()
                Case "exit"
                    Throw New ExitProgramException()
                Case "clear"
                    Console.Clear()
                    Return True
                Case "help"
                    PrintLine(HelpMenu)
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>
        ''' Requests input from the user, attempting to convert it to a specific type.
        ''' If an exception occurs, an error message Is shown And the request Is retried.
        ''' </summary>
        ''' <typeparam name="T">The type of the requested value.</typeparam>
        ''' <param name="converter">A function that converts the input string into the requested type.</param>
        ''' <param name="message">A message that guides the user on what to input.</param>
        ''' <param name="style">The style of the request message.</param>
        ''' <returns>The user input converted to the specified type.</returns>
        ''' <exception cref="ExitToMenuException">Thrown when the user chooses to return to the menu.</exception>
        ''' <exception cref="ExitProgramException">Thrown when the user chooses to exit the program.</exception>
        Public Function Request(Of T)(converter As Converter(Of String, T), Optional message As String = Nothing, Optional style As RequestStyle = RequestStyle.Default) As T
            While True
                Try
                    Return converter(Request(message, style))
                Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                    [Error](e.Message)
                End Try
            End While
        End Function

        ''' <summary>
        ''' Requests input from the user, attempting to convert it to a specified type that implements IParsable.
        ''' If an exception occurs, an error message Is shown And the request Is retried.
        ''' </summary>
        ''' <typeparam name="T">The type of the requested value, constrained to be IParsable.</typeparam>
        ''' <param name="message">A message that guides the user on what to input.</param>
        ''' <param name="style">The style of the request message.</param>
        ''' <returns>The user input converted to the specified type.</returns>
        ''' <exception cref="ExitToMenuException">Thrown when the user chooses to return to the menu.</exception>
        ''' <exception cref="ExitProgramException">Thrown when the user chooses to exit the program.</exception>
        Public Function Request(Of T As {IParsable(Of T), New})(Optional message As String = Nothing, Optional style As RequestStyle = RequestStyle.[Default]) As T
            Return Request(AddressOf Parse(Of T), message, style)
        End Function

        ''' <summary>
        ''' Requests a string input from the user And returns it.
        ''' </summary>
        ''' <param name="message">A message that guides the user on what to input.</param>
        ''' <param name="style">The style of the request message.</param>
        ''' <returns>The user input as a string.</returns>
        ''' <exception cref="ExitToMenuException">Thrown when the user chooses to return to the menu.</exception>
        ''' <exception cref="ExitProgramException">Thrown when the user chooses to exit the program.</exception>
        Public Function Request(Optional message As String = Nothing, Optional style As RequestStyle = RequestStyle.[Default]) As String
            Dim input As String
            Do
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

                input = Reader.ReadLine()
            Loop While HandleCommand(input)
            Return input
        End Function

        ''' <summary>
        ''' Requests the number of elements from the user And generates an array of random elements.
        ''' </summary>
        ''' <typeparam name="T">The type of the elements in the array.</typeparam>
        ''' <param name="getRandomItem">A function that generates random elements of type T.</param>
        ''' <returns>An array of randomly generated elements of type T.</returns>
        Public Function ReadArrayRandom(Of T)(getRandomItem As Func(Of T)) As DynArray(Of T)
            Dim size As Integer = Request(Function(input) SizeInt(input), "Input number of elements")
            Dim generatedArray As DynArray(Of T) = Generator.GetRandomDynArray(New Range(size, size), getRandomItem)

            PrintLine(String.Format("Generated array: {0}", generatedArray))
            Return generatedArray
        End Function

        ''' <summary>
        ''' Requests elements from the user And collects them into an array.
        ''' </summary>
        ''' <typeparam name="T">The type of the elements in the array.</typeparam>
        ''' <param name="converter">A function that converts each input string into the requested type.</param>
        ''' <returns>An array of elements of type T entered by the user.</returns>
        Public Function ReadArrayInline(Of T)(converter As Converter(Of String, T)) As DynArray(Of T)
            Dim input As String
            Do
                input = Reader.ReadLine()
            Loop While HandleCommand(input)

            Return input _
                .Split() _
                .Map(Function(inp) converter(inp)) _
                .ToDynArray()
        End Function

        ''' <summary>
        ''' Requests elements from the user And collects them into an array.
        ''' </summary>
        ''' <typeparam name="T">The type of the elements in the array, constrained to be IParsable.</typeparam>
        ''' <returns>An array of elements of type T entered by the user.</returns>
        Public Function ReadArrayInline(Of T As {IParsable(Of T), New})() As DynArray(Of T)
            Return Reader _
                .ReadLine() _
                .Split() _
                .Map(AddressOf Parse(Of T)) _
                .ToDynArray()
        End Function

        ''' <summary>
        ''' Requests the user to specify the input method for entering an array.
        ''' </summary>
        ''' <typeparam name="T">The type of the elements in the array, constrained to be IParsable.</typeparam>
        ''' <param name="getRandomItem">A function that generates random elements of type T.</param>
        ''' <returns>An array of elements of type T entered by the user.</returns>
        Public Function RequestArray(Of T As {IParsable(Of T), New})(getRandomItem As Func(Of T)) As DynArray(Of T)
            Return RequestArray(AddressOf Parse(Of T), getRandomItem)
        End Function

        ''' <summary>
        ''' Requests the user to specify the input method for entering an array.
        ''' </summary>
        ''' <typeparam name="T">The type of the elements in the array.</typeparam>
        ''' <param name="converter">A function that converts each input string into the requested type.</param>
        ''' <param name="getRandomItem">A function that generates random elements of type T.</param>
        ''' <returns>An array of elements of type T entered by the user.</returns>
        Public Function RequestArray(Of T)(converter As Converter(Of String, T), getRandomItem As Func(Of T)) As DynArray(Of T)
            Dim isRandom As Boolean = ChooseInputMethod()

            If isRandom Then
                Return ReadArrayRandom(getRandomItem)
            Else
                Do
                    Try
                        PrintLine("Enter the array separated by spaces:")
                        Return ReadArrayInline(converter)
                    Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                        [Error](e.Message)
                    End Try
                Loop
            End If
        End Function

        ''' <summary>
        ''' Requests a two-dimensional matrix of elements of type T, where T implements IParsable.
        ''' </summary>
        ''' <param name="getRandomItem">A function that generates random items of type T.</param>
        ''' <typeparam name="T">The type of the elements in the matrix, constrained to be IParsable.</typeparam>
        ''' <returns>A dynamically sized matrix (array of arrays) with elements of type T.</returns>
        Public Function RequestMatrix(Of T As {IParsable(Of T), New})(getRandomItem As Func(Of T)) As DynArray(Of DynArray(Of T))
            Return RequestMatrix(AddressOf Parse(Of T), getRandomItem)
        End Function

        ''' <summary>
        ''' Requests a matrix of elements, using a specified converter And random item generator.
        ''' </summary>
        ''' <typeparam name="T">The type of elements in the matrix, constrained to be parsable.</typeparam>
        ''' <param name="converter">A function that converts each input string into the requested type.</param>
        ''' <param name="getRandomItem">A function that generates random elements of type T.</param>
        ''' <returns>A dynamically sized matrix (array of arrays) with parsed Or randomly generated elements.</returns>
        Public Function RequestMatrix(Of T)(converter As Converter(Of String, T), getRandomItem As Func(Of T)) As DynArray(Of DynArray(Of T))
            Dim size As Integer = Request(AddressOf SizeInt, "Input number Of Sub-arrays")
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

        ''' <summary>
        ''' Reads And returns a matrix as a dynamic array of rows, each row being a dynamic array of elements of type T.
        ''' </summary>
        ''' <typeparam name="T">The type of elements in the matrix.</typeparam>
        ''' <param name="size">The number of rows in the matrix.</param>
        ''' <param name="converter">A function that converts input strings to the desired type T.</param>
        ''' <param name="show">Outputs a flag indicating if any error occurred during input processing.</param>
        ''' <returns>A dynamic array of dynamic arrays representing the matrix.</returns>
        Private Function RequestMatrixInline(Of T)(size As Integer, converter As Converter(Of String, T), ByRef show As Boolean) As DynArray(Of DynArray(Of T))
            Dim typed As New DynArray(Of DynArray(Of T))(size, Function() Nothing)
            show = False

            For i As Integer = 0 To size - 1
                Try
                    Print(i + 1 & ": ")
                    typed(i) = ReadArrayInline(Function(input) converter(input))
                Catch e As Exception When TypeOf e Is FormatException OrElse TypeOf e Is OverflowException
                    [Error](e.Message)
                    i -= 1
                    show = True
                End Try
            Next

            Return typed
        End Function

        ''' <summary>
        ''' Reads And returns a matrix of random elements based on the given size And generator function.
        ''' </summary>
        ''' <typeparam name="T">The type of elements in the matrix.</typeparam>
        ''' <param name="rows">The number of rows in the matrix.</param>
        ''' <param name="getRandomItem">A function that generates a random item of type <typeparamref name="T"/>.</param>
        ''' <returns>A dynamic array of dynamic arrays representing the randomly generated matrix.</returns>
        Private Function RequestMatrixRandom(Of T)(rows As Integer, getRandomItem As Func(Of T)) As DynArray(Of DynArray(Of T))
            Dim maxColumnSize As Integer = Request(AddressOf SizeInt, "Input max number of elements")
            Return New DynArray(Of DynArray(Of T)) _
                (rows, Function() Generator.GetRandomDynArray(New Range(1, maxColumnSize), getRandomItem))
        End Function

        ''' <summary>
        ''' Prompts the user to choose between inputting elements manually Or generating them randomly.
        ''' </summary>
        ''' <returns>A boolean indicating whether the random input method was chosen.</returns>
        Private Function ChooseInputMethod() As Boolean
            Const message As String =
                "Select input method:" & vbCrLf &
                "    1. Random" & vbCrLf &
                "    2. Manual"

            Do
                Select Case Request(message).Trim()
                    Case "1" : Return True
                    Case "2" : Return False
                End Select
                Error ("Unknown option")
            Loop
        End Function

        ''' <summary>
        ''' Parses <paramref name="input"/> as int And ensures it's positive.
        ''' </summary>
        ''' <param name="input">The user input as a string.</param>
        ''' <returns>A valid array size.</returns>
        Private Function SizeInt(input As String) As Integer
            Dim size As Integer = Integer.Parse(input)

            If size < 1 Then
                Throw New FormatException($"Value {size} is not in valid range (1..{Integer.MaxValue})")
            End If

            Return size
        End Function

        Private Function Parse(Of T As {IParsable(Of T), New})(input As String) As T
            Dim method = GetType(T).GetMethod("Parse", {GetType(String), GetType(IFormatProvider)})

            Try
                Try
                    Return method.Invoke(Nothing, {input, Nothing})
                Catch ex As TargetInvocationException
                    If TypeOf ex.InnerException Is FormatException Then
                        Throw New FormatException($"Format error: '{input}' cannot be converted to {GetType(T).Name}.", ex.InnerException)
                    ElseIf TypeOf ex.InnerException Is OverflowException Then
                        Throw New OverflowException($"Overflow: '{input}' is too large or small for {GetType(T).Name}.", ex.InnerException)
                    End If
                    Throw
                End Try
            Catch ex As Exception
                Throw
            End Try
        End Function
    End Class
End Namespace
