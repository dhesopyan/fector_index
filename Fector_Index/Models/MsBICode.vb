Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsBICode
    <Key> _
    <DisplayName("BI Code")> _
    <Required(ErrorMessage:="Please input BI Code")> _
    <StringLength(20)> _
    Public Property BICode As String
    
    <ForeignKey("SubjectStatus")> _
    <DisplayName("Status")> _
    <Required(ErrorMessage:="Please select subject status")> _
    Public Property SubjectStatusId As String
    Public Overridable Property SubjectStatus As MsSubjectStatus

    <DisplayName("Company Name")> _
    <Required(ErrorMessage:="Please input company name")> _
    <StringLength(100)> _
    Public Property SubjectName As String

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetBICode(ByVal BICode As String, ByVal db As FectorEntities) As MsBICode
        Return db.BICodes.Where(Function(f) f.BICode = BICode).SingleOrDefault
    End Function
End Class

Public Class MsBICodeMatching
    Public Property bicode As MsBICode
    Public Property NametoMatch As String
    Public ReadOnly Property MatchPercentage As Decimal
        Get
            Return CalculateSimilarity(Me.bicode.SubjectName, Me.NametoMatch) * 100
        End Get
    End Property

    Private Function ComputeLevenshteinDistance(source As String, target As String) As Integer
        If (source Is Nothing) OrElse (target Is Nothing) Then
            Return 0
        End If
        If (source.Length = 0) OrElse (target.Length = 0) Then
            Return 0
        End If
        If source = target Then
            Return source.Length
        End If

        Dim sourceWordCount As Integer = source.Length
        Dim targetWordCount As Integer = target.Length

        ' Step 1
        If sourceWordCount = 0 Then
            Return targetWordCount
        End If

        If targetWordCount = 0 Then
            Return sourceWordCount
        End If

        Dim distance As Integer(,) = New Integer(sourceWordCount, targetWordCount) {}

        ' Step 2
        Dim i As Integer = 0
        While i <= sourceWordCount


            distance(i, 0) = System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
        End While
        Dim j As Integer = 0
        While j <= targetWordCount


            distance(0, j) = System.Math.Max(System.Threading.Interlocked.Increment(j), j - 1)
        End While

        For i = 1 To sourceWordCount
            For j = 1 To targetWordCount
                ' Step 3
                Dim cost As Integer = If((target(j - 1) = source(i - 1)), 0, 1)

                ' Step 4
                distance(i, j) = Math.Min(Math.Min(distance(i - 1, j) + 1, distance(i, j - 1) + 1), distance(i - 1, j - 1) + cost)
            Next
        Next

        Return distance(sourceWordCount, targetWordCount)
    End Function

    Private Function CalculateSimilarity(source As String, target As String) As Double
        If (source Is Nothing) OrElse (target Is Nothing) Then
            Return 0.0
        End If
        If (source.Length = 0) OrElse (target.Length = 0) Then
            Return 0.0
        End If
        If source = target Then
            Return 1.0
        End If

        Dim stepsToSame As Integer = ComputeLevenshteinDistance(source, target)
        Return (1.0 - (CDbl(stepsToSame) / CDbl(Math.Max(source.Length, target.Length))))
    End Function
End Class