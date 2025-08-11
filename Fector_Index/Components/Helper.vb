Imports System
Imports System.IO
Imports System.Globalization

Namespace Helper

    Public Class DateTimeHelper

        Public Shared Function FromDMY(s As String) As DateTime
            Dim dmy() As String = s.Split("-")
            Return New DateTime(dmy(2), dmy(1), dmy(0))
        End Function

        Public Shared Function ToDMY(d As DateTime) As String
            Return d.ToString("ddMMyyyy")
        End Function
    End Class

End Namespace