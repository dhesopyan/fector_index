Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsSetting
    <Key> _
    <Column(Order:=1)> _
    Public Property Key1 As String
    <Key> _
    <Column(Order:=2)> _
    Public Property Key2 As String
    <Key> _
    <Column(Order:=3)> _
    Public Property Key3 As String

    Public Property Value1 As String
    Public Property Value2 As String
    Public Property Value3 As String

    Public Shared Function GetSetting(key1 As String, key2 As String, key3 As String, valueindex As Integer) As String
        Dim db As New FectorEntities()

        Dim setting = db.Settings.Where(Function(s) s.Key1 = key1 And s.Key2 = key2 And s.Key3 = key3).SingleOrDefault
        If Not IsNothing(setting) Then
            If valueindex = 1 Then
                Return setting.Value1
            ElseIf valueindex = 2 Then
                Return setting.Value2
            ElseIf valueindex = 3 Then
                Return setting.Value3
            End If
        End If
        Return ""
    End Function
End Class
